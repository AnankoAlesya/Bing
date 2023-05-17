using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kurs2
{
    public class Server
    {
        private TcpListener _listener;
        private Dictionary<int, Clients> _clients;
        private List<Books> _books;

        public Server()
        {
            var data = Serialization.Deserialize();
            _clients = data.Item1;
            _books = data.Item2;
        }

        public async Task StartAsync(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                HandleClientAsync(client);
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();

            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    break;
                }

                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received request: " + request);
                var jsonRequest = JsonConvert.DeserializeObject<Request>(request);
                string response = Handler.CommandHandler(jsonRequest);
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }

            client.Close();
        }
    }
}