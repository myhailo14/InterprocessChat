package ua.lpnu.interprocesschat.server;

import java.io.IOException;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.*;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.logging.Logger;

public class Server extends Thread {

    private final Logger log = Logger.getLogger(Server.class.getName());

    private ServerSocket server;

    private List<ClientListener> clientListenerList;

    private EventQueue<Message> messages;

    static {
        String path = Objects.requireNonNull(Server.class.getClassLoader()
                        .getResource("logging.properties"))
                .getFile();
        System.setProperty("java.util.logging.config.file", path);
    }

    public Server(String ip, int port) {
        try {
            log.info("Server is starting...");
            server = new ServerSocket(port, 50, InetAddress.getByName(ip));
            server.setReceiveBufferSize(1024);
            messages = new EventQueue<>();
            clientListenerList = new ArrayList<>();
            messages.addEventHandler(() -> {
                Optional<Message> optionalMessage = Optional.ofNullable(messages.poll());
                if (optionalMessage.isPresent()) {
                    Message message = optionalMessage.get();
                    if (message.getType() == Message.Type.UPDATE) {
                        log.info(Message.Type.UPDATE + " command was executed");
                        clientListenerList.forEach(client -> client.sendMessage("update " +
                                Arrays.toString(clientListenerList.stream()
                                .map(ClientListener::getClient).map(Client::getName).toArray()))
                        );
                    } else if (message.getType() == Message.Type.MESSAGE) {
                        if (message.getContent().length == 1) {
                            log.info(Message.Type.MESSAGE + " command was executed");
                            clientListenerList.forEach(client -> {
                                Object[] a = List.of(message.getAuthor().getName(), String.join(", ",
                                        message.getContent())).toArray();
                                client.sendMessage("message " + Arrays.toString(a));
                            });
                        }
                    }
                }
            });
            log.info("Server was started.");
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
