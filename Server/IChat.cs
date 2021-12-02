using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;

namespace SocketChat
{
    public interface IChat
    {
        Socket Socket { get; set; }
        Thread Thread { get; set; }
        Dispatcher Dispatcher { get; set; }
        bool IsActive { get; set; }
        IPAddress IPAddress { get; set; }
        ushort Port { get; set; }
        IPEndPoint IPEndPoint { get; }
        int ClientIdCounter { get; set; }
        string SourceUsername { get; set; }

        BindingList<Client> ClientList { get; set; }
        BindingList<string> ChatList { get; set; }       

        EventHandler IsActiveChanged { get; set; }

        void StartConnection();
        void StopConnection();
        void OnIsActiveChanged(EventArgs e);
    }
}
