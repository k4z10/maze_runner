using System.Net;
using maze_runner.Client;
using maze_runner.Model.Core;
using maze_runner.Model.Dungeon.Themes.Cave;

namespace maze_runner;
using Server;

static class Program
{
    static async Task Main(string[] args)
    {
        ParseProgramArgs(args,
            out var startClient,
            out var startServer,
            out var clientIp,
            out var clientPort,
            out var serverPort);

        if (!startClient && !startServer) Console.WriteLine("Run either as client or server");

        await ApplicationRunner.RunAsync(startClient, startServer, clientIp, clientPort, serverPort);
    }
    
    private static void ParseProgramArgs(
        string[] args,
        out bool startClient,
        out bool startServer,
        out string clientIp,
        out int clientPort,
        out int serverPort)
    {
        startClient = false;
        startServer = false;
        clientIp = "127.0.0.1";
        clientPort = 5555;
        serverPort = 5555;

        for (int i = 0; i < args.Length; i++)
        {
            string token = args[i].ToLowerInvariant();

            switch (token)
            {
                case "--client":
                    startClient = true;
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        string endpoint = args[++i];
                        
                        var parts = endpoint.Split(':');
                        if (parts.Length == 2 && 
                            IPAddress.TryParse(parts[0], out var ip) && 
                            int.TryParse(parts[1], out int port) && 
                            port is > 0 and <= 65535)
                        {
                            clientIp = ip.ToString();
                            clientPort = port;
                        }
                        else
                        {
                            Console.WriteLine($"[Błąd Krytyczny] Nieprawidłowy adres dla --client: '{endpoint}'. Oczekiwany format: IP:PORT (np. 127.0.0.1:5555).");
                            Environment.Exit(1);
                        }
                    }
                    break;

                case "--server":
                    startServer = true;
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        string portStr = args[++i];
                        
                        if (int.TryParse(portStr, out int port) && port is > 0 and <= 65535)
                        {
                            serverPort = port;
                        }
                        else
                        {
                            Console.WriteLine($"[Błąd Krytyczny] Nieprawidłowy port dla --server: '{portStr}'. Oczekiwany zakres: 1-65535.");
                            Environment.Exit(1);
                        }
                    }
                    break;

                case "--help":
                    Console.WriteLine("""
                                      This is a game written for the sake of ProjObs.

                                      Usage:
                                        maze_runner [options]

                                      Options:
                                        --client [ip:port]   Starts the application in Client mode. 
                                                             Attempts to connect to the specified IP address and port.
                                                             If omitted, defaults will be used.
                                                             (Default: 127.0.0.1:5555)

                                        --server [port]      Starts the application in Server mode.
                                                             Listens for incoming connections on the specified port.
                                                             If omitted, default port will be used.
                                                             (Default: 5555)

                                        --help               Shows this help message and exits.
                                      """);
                    Environment.Exit(0);
                    break;
                    
                default:
                    Console.WriteLine($"[Błąd] Nieznany argument: '{token}'.");
                    Environment.Exit(1);
                    break;
            }
        }
    }

    private static class ApplicationRunner
    {
        public static async Task RunAsync(bool startClient, bool startServer, string clientIp, int clientPort, int serverPort)
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            if (startClient && startServer) await RunHybridModeAsync(clientIp, clientPort, serverPort, cts.Token);
            else if (startServer)           await RunDedicatedServerAsync(serverPort, cts.Token);
            else if (startClient)           await RunDedicatedClientAsync(clientIp, clientPort, cts.Token);
        }

        private static async Task RunHybridModeAsync(string clientIp, int clientPort, int serverPort, CancellationToken token)
        {
            var config = ConfigLoader.Load("config.json");
            var server = new ServerEngine(config, serverPort);
            server.LoadLevel(new CaveTheme(), itemsCount: 15, enemiesCount: 10);

            _ = Task.Run(() => server.StartServerAsync(token), token);
            await Task.Delay(150, token);

            var client = new ClientEngine();
            await client.ConnectAndRunAsync(clientIp, clientPort, token);
            
            server.StopServer();
        }

        private static async Task RunDedicatedServerAsync(int serverPort, CancellationToken token)
        {
            var config = ConfigLoader.Load("config.json");
            var server = new ServerEngine(config, serverPort);
            
            server.LoadLevel(new CaveTheme(), itemsCount: 15, enemiesCount: 10);
            
            token.Register(server.StopServer);
            await server.StartServerAsync(token); 
        }

        private static async Task RunDedicatedClientAsync(string clientIp, int clientPort, CancellationToken token)
        {
            var client = new ClientEngine();
            await client.ConnectAndRunAsync(clientIp, clientPort, token);
        }
    }
}