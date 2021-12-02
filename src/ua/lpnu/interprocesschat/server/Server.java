package ua.lpnu.interprocesschat.server;

import java.io.IOException;
import java.lang.reflect.Array;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

public class Server extends Thread {

    private ServerSocket server;

    private List<ClientListener> clientListenerList;

    private EventQueue<Message> messages;

    public Server(String ip, int port) {
        try {
            server = new ServerSocket(port, 50, InetAddress.getByName(ip));
            server.setReceiveBufferSize(1024);
            messages = new EventQueue<>();
            clientListenerList = new ArrayList<>();
            messages.addEventHandler(() -> {
                Optional<Message> optionalMessage = Optional.ofNullable(messages.poll());
                if (optionalMessage.isPresent()) {
                    Message message = optionalMessage.get();
                    if (message.getType() == Message.Type.UPDATE) {
                        clientListenerList.forEach(client -> client.sendMessage("update " +
                                Arrays.toString(clientListenerList.stream()
                                .map(ClientListener::getClient).map(Client::getName).toArray()))
                        );
                    } else if (message.getType() == Message.Type.MESSAGE) {
                        if (message.getContent().length == 1) {
                            clientListenerList.forEach(client -> {
                                Object[] a = List.of(message.getAuthor().getName(), String.join(", ",
                                        message.getContent())).toArray();
                                client.sendMessage("message " + Arrays.toString(a));
                            });
                        }
                    }
                }
            });
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void run() {
        try {
            while (true) {
                Socket socket = server.accept();
                clientListenerList.add(new ClientListener(new Client(), socket, messages));
                clientListenerList.removeIf(p -> !p.isAlive());
                System.out.println(clientListenerList);
            }
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            close();
        }
    }

    private void close() {
        try {
            server.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
