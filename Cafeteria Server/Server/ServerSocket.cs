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
        //private readonly AuthenticationController _authenticationManager;
        private readonly CommandDispatcher _dispatcher;

        public ServerSocket(int port, CommandDispatcher dispatcher)
        {
            this.port = port;
            //_authenticationManager = authenticationManager;
            _dispatcher = dispatcher;
        }

        public async Task Start()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Server started. Listening on port {port}...");

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected.");

                    Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                //// Example: Send a welcome message to the client
                //string message = "Welcome to the server!";
                //byte[] data = Encoding.ASCII.GetBytes(message);
                //await stream.WriteAsync(data, 0, data.Length);

                // Handle client requests and responses here...
                byte[] data = new byte[256];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                //string authRequest = Encoding.ASCII.GetString(data, 0, bytesRead);


                string request = Encoding.ASCII.GetString(data, 0, bytesRead);

                var requestObject = JsonConvert.DeserializeObject<RequestObject>(request);
                string commandName = requestObject.CommandName;
                string requestData = requestObject.RequestData;

                string jsonResponse = await _dispatcher.Dispatch(commandName, requestData);
                byte[] responseData = Encoding.ASCII.GetBytes(jsonResponse);
                await stream.WriteAsync(responseData, 0, responseData.Length);

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
