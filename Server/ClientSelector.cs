using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SocketChat
{
    public class ClientSelector : INotifyPropertyChanged
    {
        private IChat _chatInterface;
        private string messageContent;

        public ClientSelector()
        {
            StartConnectionCMD = new RelayCommand(StartConnection);
            SendMessageCMD = new RelayCommand(() => SendMessage(MessageContent));
            SelectClient();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand StartConnectionCMD { get; }
        public ICommand SendMessageCMD { get; }

        public BindingList<string> ChatList
        {
            get => _chatInterface.ChatList;
            private set
            {
                _chatInterface.ChatList = value;
                NotifyPropertyChanged("ChatList");
            }
        }
        public BindingList<Client> ClientList
        {
            get => _chatInterface.ClientList;
            private set
            {
                _chatInterface.ClientList = value;
                NotifyPropertyChanged("ClientList");
            }
        }

        public string WindowTitle => "Client";

        public string WindowIcon => "Client.ico";

        public bool IsActive
        {
            get => _chatInterface.IsActive;
            private set
            {
                _chatInterface.IsActive = value;
                NotifyPropertyChanged("IsActive");
            }
        }

        public int ActiveClients => _chatInterface.ClientList.Count;

        public string IpAddress
        {
            get => _chatInterface.IPAddress.ToString();
            set
            {
                if (IsActive)
                {
                    throw new Exception("Can't change this property when server is active");
                }

                _chatInterface.IPAddress = IPAddress.Parse(value);
                NotifyPropertyChanged("IpAddress");
            }
        }

        public ushort Port
        {
            get => _chatInterface.Port;
            set
            {
                if (IsActive)
                {
                    throw new Exception("Can't change this property when server is active");
                }

                _chatInterface.Port = value;
                NotifyPropertyChanged("Port");
            }
        }

        public string SourceUsername
        {
            get => _chatInterface.SourceUsername;
            set
            {
                _chatInterface.SourceUsername = value;
                if (IsActive)
                {
                    _chatInterface.ClientList[0].Username = value;
                }
                NotifyPropertyChanged("SourceUsername");
            }
        }

        public string MessageContent
        {
            get => messageContent;
            set
            {
                messageContent = value;
                NotifyPropertyChanged("MessageContent");
            }
        }

        private void SelectClient()
        {
            
            _chatInterface = new ChatClient();
            SourceUsername = _chatInterface.SourceUsername;
            
            IpAddress = "127.0.0.1";
            Port = 5960;

            ClientList = new BindingList<Client>();

            ChatList = new BindingList<string>();

            _chatInterface.IsActiveChanged = new EventHandler(IsActiveBool);

            _chatInterface.ClientList.ListChanged += (sender, e) =>
            {
                NotifyPropertyChanged("ActiveClients");
            };

        }

        public void StartConnection()
        {
            
            if (!IsActive)
            {
                try
                {
                    _chatInterface.StartConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _chatInterface.StopConnection();
            }
            
        }

        public void SendMessage(string messageContent)
        {
            //string sourceMessage = _chatInterface.SourceUsername + ": " + messageContent;

            string message = $"message {messageContent}";
            _chatInterface.Socket.Send(Encoding.Unicode.GetBytes(message));

           // _chatInterface.ChatList.Add(sourceMessage);
        }

        public void IsActiveBool(object sender, EventArgs e)
        {
            NotifyPropertyChanged("IsActive");
        }

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
