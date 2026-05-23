using System.Net;
using maze_runner.Client;
using maze_runner.Model.Core;
using maze_runner.Model.Core.Logger;
using maze_runner.Model.Dungeon.Themes.Cave;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace maze_runner;
using maze_runner.Server;

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

        Task serverTask = Task.CompletedTask, clientTask = Task.CompletedTask;
        if (startServer)
        {
            var config = ConfigLoader.Load("config.json");
            
            var server = new ServerEngine(config, serverPort);
            server.LoadLevel(new CaveTheme(), itemsCount: 15, enemiesCount: 10);
            serverTask = Task.Run(server.StartServerAsync);
        }
        if (startClient)
        {
            var client = new ClientEngine();
            clientTask = Task.Run(() => client.ConnectAndRunAsync(clientIp, clientPort));
        }
        
        if (!startClient && !startServer) Console.WriteLine("Run either as client or server");
        await Task.WhenAll(serverTask, clientTask);
    }

    private static bool TryParseEndpoint(string input, out string ip, out int port)
    {
        ip = null;
        port = 0;

        var parts = input.Split(':');
        if (parts.Length != 2) return false;
        
        if (IPAddress.TryParse(parts[0], out var parsedIp) && int.TryParse(parts[1], out var parsedPort) && parsedPort > 0 && parsedPort <= 1 << 16)
        {
            ip = parsedIp.ToString();
            port = parsedPort;
            return true;
        }
        
        return false;
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
            string arg = args[i].ToLower();
            switch (arg)
            {
                case "--client":
                {
                    startClient = true;
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("--")) continue;
                    var val = args[i + 1];
                    i++;

                    if (!TryParseEndpoint(val, out var ip, out var port)) continue;
                    clientIp = ip;
                    clientPort = port;
                    break;
                }
                case "--server":
                {
                    startServer = true;
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("--")) continue;
                    var val = args[i + 1];
                    i++;

                    if (int.TryParse(val, out var port) && port > 0 && port <= 1 << 16) continue;
                    serverPort = port;
                    break;
                }
                case "--help":
                {
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
                }
            }
        }
        
    }
}