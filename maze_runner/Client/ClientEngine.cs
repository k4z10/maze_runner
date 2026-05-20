using System.Net.Sockets;
using System.Security.Authentication;
using System.Text.Json;
using maze_runner.Network.DTOs.Actions;
using maze_runner.Network.DTOs.GameState;
using maze_runner.View;
using maze_runner.View.TerminalGui;

namespace maze_runner.Client;

public class ClientEngine
{
    private TcpClient _tcpClient = null!;
    private StreamReader _reader = null!;
    private StreamWriter _writer = null!;

    private IGameFrontend _frontend = null!;
    private GameStateSnapshotDto? _latestSnapshot;
    private LevelMetaDto? _currentLevelMeta;
    private int _myPlayerId;

    public async Task ConnectAndRunAsync(string ip, int port)
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(ip, port);
        
        var stream = _tcpClient.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream) { AutoFlush = true };

        // HANDSHAKE: Odbieramy swoje przydzielone ID z serwera jako pierwszą wiadomość
        string? handshake = await _reader.ReadLineAsync();
        if (string.IsNullOrEmpty(handshake) || !int.TryParse(handshake, out _myPlayerId)) throw new AuthenticationException("Server did not respond");

        // Inicjalizacja pasywnego widoku
        _frontend = new TerminalFrontend(_myPlayerId);
        _frontend.OnKeyPressed += HandleInput;

        // Uruchamiamy pętlę czytającą w tle, aby nie zablokować UI
        _ = Task.Run(ReceiveLoopAsync);

        // Uruchamiamy interfejs (to wywołanie blokuje główny wątek klienta aż do wyjścia z gry)
        _frontend.Run();
    }

    private async Task ReceiveLoopAsync()
    {
        try
        {
            while (_tcpClient.Connected)
            {
                string? json = await _reader.ReadLineAsync();
                if (string.IsNullOrEmpty(json)) break; // Serwer zerwał połączenie

                var snapshot = JsonSerializer.Deserialize<GameStateSnapshotDto>(json);
                if (snapshot != null)
                {
                    _latestSnapshot = snapshot;
                    _frontend.RenderSnapshot(snapshot);
                }
            }
        }
        catch (Exception ex)
        {
            // W środowisku Terminal.Gui wyprintowanie tutaj zepsuje render, 
            // najlepiej zalogować do pliku lub pokazać MessageDialog.
            Terminal.Gui.Application.Invoke(() => Terminal.Gui.MessageBox.ErrorQuery("Błąd Sieci", ex.Message, "OK"));
        }
        finally
        {
            Terminal.Gui.Application.RequestStop(); // Zamknij UI w przypadku utraty połączenia
        }
    }

    private void HandleInput(char key)
    {
        if (!_tcpClient.Connected) return;

        string? commandId = null;
        var args = new Dictionary<string, string>();

        var dynamicBinding = _latestSnapshot?.LevelMeta.Commands.FirstOrDefault(c => c.Key == key);
        if (dynamicBinding != null)
        {
            commandId = dynamicBinding.CommandId;
        }

        if (commandId != null)
        {
            var request = new ActionRequestDto
            {
                PlayerId = _myPlayerId,
                CommandId = commandId,
            };

            // Serializacja i wysyłka
            string jsonPayload = JsonSerializer.Serialize(request);
            _writer.WriteLine(jsonPayload);
        }
    }
}