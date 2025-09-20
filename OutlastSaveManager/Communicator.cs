using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OutlastSaveManager
{
    internal class Communicator
    {
        private TcpListener _listener;
        private readonly List<TcpClient> tcpClients = new List<TcpClient>();
        private readonly Object _monitor = new();
        public event Action<String>? MessageReceived;

        private void OnMessageReceived(String message) => MessageReceived?.Invoke(message.Trim());

        public Communicator(int port) => _listener = new TcpListener(IPAddress.Loopback, port);

        public async Task AcceptConnects()
        {
            _listener.Start();
            try
            {
                while (true)
                {
                    var connection = await _listener.AcceptTcpClientAsync();
                    AddClient(connection);
                    _ = ReadLoop(connection);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            _listener.Stop();

            lock (_monitor)
            {
                tcpClients.ForEach(t => t.Close());
                tcpClients.Clear();
            }
        }

        public async Task SendMessage(String message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");
            List<TcpClient> snapShot;

            lock (_monitor)
            {
                snapShot = tcpClients.Where(t => t.Connected).ToList();
            }

            foreach (var t in snapShot)
            {
                var stream = t.GetStream();
                try
                {
                    await stream.WriteAsync(bytes);
                    await stream.FlushAsync();
                }
                catch (Exception)
                {
                    RemoveClient(t);
                }
            }
        }

        private async Task ReadLoop(TcpClient client)
        {
            using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);

            try
            {
                while (client.Connected)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                    {
                        RemoveClient(client);
                        break;
                    }

                    OnMessageReceived(line);
                }
            }
            catch (Exception)
            {
                RemoveClient(client);
            }
        }

        private void AddClient(TcpClient client)
        {
            lock (_monitor)
            {
                tcpClients.Add(client);
            }
        }

        private void RemoveClient(TcpClient client)
        {
            lock (_monitor)
            {
                tcpClients.Remove(client);
            }
            client.Close();
        }


    }
}
