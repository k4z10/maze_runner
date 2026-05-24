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
    private int _myPlayerId;

    public async Task ConnectAndRunAsync(string ip, int port, CancellationToken token = default)
    {
        try
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ip, port, token);

            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };

            var handshake = await _reader.ReadLineAsync(token);
            if (string.IsNullOrEmpty(handshake) || !int.TryParse(handshake, out _myPlayerId))
                throw new AuthenticationException("Server did not respond");

            _frontend = new TerminalFrontend(_myPlayerId);
            _frontend.OnKeyPressed += HandleInput;

            _ = Task.Run(() => ReceiveLoopAsync(token), token);

            _frontend.Run();
        }
        catch (OperationCanceledException) {}
        finally
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
        }
        
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        try
        {
            while (_tcpClient.Connected)
            {
                string? json = await _reader.ReadLineAsync(token);
                if (string.IsNullOrEmpty(json)) break;

                var snapshot = JsonSerializer.Deserialize<GameStateSnapshotDto>(json);
                if (snapshot != null)
                {
                    _latestSnapshot = snapshot;
                    _frontend.RenderSnapshot(snapshot);
                }
            }
        }
        catch (OperationCanceledException) {}
        catch (Exception ex)
        {
            if (!_tcpClient.Connected) return;
            Terminal.Gui.Application.Invoke(() => Terminal.Gui.MessageBox.ErrorQuery("Błąd Sieci", ex.Message, "OK"));
        }
        finally
        {
            Terminal.Gui.Application.RequestStop();
        }
    }

    private void HandleInput(char key)
    {
        if (!_tcpClient.Connected) return;

        string? commandId = null;
        var args = new Dictionary<string, string>();

        var dynamicBinding = _latestSnapshot?.LevelMeta?.Commands.FirstOrDefault(c => c.Key == key);
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

            string jsonPayload = JsonSerializer.Serialize(request);
            _writer.WriteLine(jsonPayload);
        }
    }
}