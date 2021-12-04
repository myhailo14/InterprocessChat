using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SocketChat.Client
{
    public class ChatClient : IChat
    {
        private bool _isActive;

        public Socket Socket { get; set; }
        public Thread Thread { get; set; }
        public Dispatcher Dispatcher { get; set; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnIsActiveChanged(EventArgs.Empty);
            }
        }

        public IPAddress IPAddress { get; set; }
        public ushort Port { get; set; }
        public IPEndPoint IPEndPoint => new IPEndPoint(IPAddress, Port);
        public int ClientIdCounter { get; set; }
        public string SourceUsername { get; set; }

        public BindingList<Client> ClientList { get; set; }
        public BindingList<string> ChatList { get; set; }

        public EventHandler IsActiveChanged { get; set; }

        public ChatClient()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ClientList = new BindingList<Client>();
            ChatList = new BindingList<string>();
            
            SourceUsername = "Client" + new Random().Next(0, 99); // random username
        }

        public async void StartConnection()
        {
            if (IsActive)
            {
                return;
            }

            if (Socket == null)
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            await Socket.ConnectAsync(IPEndPoint);

            Auth(SourceUsername);

            Thread = new Thread(ReceiveMessages);
            Thread.Start();

            IsActive = true;
        }

        private void Auth(string username)
        {
            string cmd = $"auth {username}";

            Socket.Send(Encoding.Unicode.GetBytes(cmd));
        }

        public void ReceiveMessages()
        {
            byte[] inf = new byte[1024];
            while (true)
            {
                if (!ReceiveMessage(inf))
                {
                    return;
                }
            }
        }

        private bool ReceiveMessage(byte[] inf)
        {
            try
            {
                if (!IsSocketConnected(Socket))
                {
                    Dispatcher.Invoke(StopConnection);
                    return false;
                }

                for (int i = 0; i < inf.Length; i++)
                {
                    inf[i] = 0;
                }

                int x = Socket.Receive(inf);

                if (x > 0)
                {
                    string strMessage = Encoding.Unicode.GetString(inf);

                    Console.WriteLine(strMessage);

                    if (strMessage.Contains("update"))
                    {
                        HandleUpdate(strMessage);
                        return true;
                    }
                    if (strMessage.Contains("message"))
                    {
                        HandleMessage(strMessage);
                    }
                }
            }
            catch (SocketException ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StopConnection();

                    // Concurrently closing a listener that is accepting at the time causes exception 10004.
                    Debug.WriteLineIf(ex.ErrorCode != 10004, $"*EXCEPTION* {ex.ErrorCode}: {ex.Message}");
                    if (ex.ErrorCode == 10054)
                    {
                        MessageBox.Show(
                            ex.Message + "\nApplication will be closed.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        Environment.Exit(0);
                    }

                    if (ex.ErrorCode != 10004)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });

                return false;
            }

            return true;
        }

        private void HandleMessage(string strMessage)
        {
            MatchCollection matches = Regex
                .Matches(
                    strMessage.Replace("message ", ""),
                    "[a-zA-Zа-яА-ЯїЇґҐ0-9]+"
                );
            string message = "";
            foreach (Match match in matches)
            {
                message += match.Value + " ";
            }

            Dispatcher.Invoke(() => { ChatList.Add(message); });
        }

        private void HandleUpdate(string strMessage)
        {
            Dispatcher.Invoke(() =>
            {
                ClientList.Clear();
                MatchCollection matches = Regex
                    .Matches(
                        strMessage.Replace("update ", ""),
                        "Client[0-9]{1,2}"
                    );
                foreach (Match match in matches)
                {
                    ClientList.Add(new Client {Username = match.Value});
                }
            });
        }

        public static bool IsSocketConnected(Socket socket)
        {
            if (!socket.Connected)
            {
                return false;
            }

            if (socket.Available != 0) return true;
            return !socket.Poll(1000, SelectMode.SelectRead);
        }

        public void StopConnection()
        {
            if (Socket != null && Thread != null)
            {
                Thread.Abort();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Disconnect(true);
            }

            ChatList.Clear();
            ClientList.Clear();
            IsActive = false;
        }

        public void OnIsActiveChanged(EventArgs e)
        {
            IsActiveChanged?.Invoke(this, e);
        }
    }
}