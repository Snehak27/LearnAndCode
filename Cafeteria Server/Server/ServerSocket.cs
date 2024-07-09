using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using CafeteriaServer.DTO;

namespace CafeteriaServer.Server
{
    public class ServerSocket
    {
        private readonly int port;
        private TcpListener listener;
        private readonly CommandDispatcher _dispatcher;

        public ServerSocket(int port, CommandDispatcher dispatcher)
        {
            this.port = port;
            _dispatcher = dispatcher;
        }

        public async Task Start()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("192.168.1.3"), port);
                listener.Start();
                Console.WriteLine($"Server started. Listening on port {port}...");

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected.");

                    // Create a new thread to handle the client connection
                    Thread clientThread = new Thread(() => HandleClientAsync(client));
                    clientThread.Start();

                    //Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096]; 

                while (client.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received request: {request}");

                    var requestObject = JsonConvert.DeserializeObject<RequestObject>(request);
                    if (requestObject != null)
                    {
                        string commandName = requestObject.CommandName;
                        string requestData = requestObject.RequestData;

                        string jsonResponse = await _dispatcher.Dispatch(commandName, requestData);
                        byte[] responseData = Encoding.ASCII.GetBytes(jsonResponse);
                        await stream.WriteAsync(responseData, 0, responseData.Length);
                        await stream.FlushAsync(); 
                    }
                }
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}
