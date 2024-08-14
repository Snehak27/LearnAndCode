using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace CafeteriaClient
{
    public class ClientSocket
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public ClientSocket(string serverIp, int port)
        {
            _client = new TcpClient(serverIp, port);
            _stream = _client.GetStream();
        }

        public async Task<string> SendRequest(RequestObject request)
        {
            string requestJson = JsonConvert.SerializeObject(request);
            byte[] requestBytes = Encoding.ASCII.GetBytes(requestJson);

            await _stream.WriteAsync(requestBytes, 0, requestBytes.Length);
            await _stream.FlushAsync(); 

            byte[] responseBytes = new byte[32768];
            int bytesRead = await _stream.ReadAsync(responseBytes, 0, responseBytes.Length);
            string responseJson = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

            return responseJson;
        }
        
        public void Close()
        {
            _stream.Close();
            _client.Close();
        }
    }
}
