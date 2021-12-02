using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketChat.Client
{
    public class Client : IDisposable, INotifyPropertyChanged
    {
        //private int _id;
        private bool _isDisposed;
        private string _username;

        public event PropertyChangedEventHandler PropertyChanged;

        public Socket Socket { get; set; }

        public Thread Thread { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                NotifyPropertyChanged("Username");
            }
        }

        public Client()
        {
            _isDisposed = false;
        }

        public static bool IsSocketConnected(Socket s)
        {
            if (!s.Connected)
            {
                return false;
            }

            if (s.Available != 0) return true;
            return !s.Poll(1000, SelectMode.SelectRead);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            if (Socket != null)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Disconnect(true);
            }

            Thread?.Abort();

            _isDisposed = true;
        }

        public bool IsSocketConnected()
        {
            return IsSocketConnected(Socket);
        }

        public void SendMessage(string message)
        {
            try
            {
                Socket.Send(Encoding.Unicode.GetBytes(message));
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}