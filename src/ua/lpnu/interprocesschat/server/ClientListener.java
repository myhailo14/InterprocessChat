package ua.lpnu.interprocesschat.server;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.logging.Logger;

public class ClientListener extends Thread {

    private final Logger log = Logger.getLogger(ClientListener.class.getName());

    private Client client;

    private EventQueue<Message> messages;

    private InputStream in;

    private OutputStream out;

    private Socket socket;

    public ClientListener(Client client, Socket socket, EventQueue<Message> messages) {
        try {
            this.in = socket.getInputStream();
            this.out = socket.getOutputStream();
            this.client = client;
            this.messages = messages;
            this.socket = socket;
            start();
        } catch (IOException e) {
            log.info( "Cannot get input and output streams");
        }
    }

    @Override
    public void run() {
        byte[] buffer = new byte[1024];
        Arrays.fill(buffer, (byte) -1);
        CommandDispatcher commandDispatcher = new CommandDispatcher(buffer, client, messages);
        while (true) {
            try {
                Arrays.fill(buffer, (byte) -1);
                System.out.println(this);
                int read = in.read(buffer);
                if (read != -1) {
                    boolean result = commandDispatcher.dispatch();
                    if (!result) {
                        log.info(client + " sent incorrect message.");
                    }
                } else {
                    log.info(client + " disconnected from the server");
                    close();
                    messages.add(new Message(this, Message.Type.UPDATE));
                    return;
                }
            } catch (IOException e) {
                e.printStackTrace();
                close();
                messages.add(new Message(this, Message.Type.UPDATE));
                log.info("Socket was closed incorrectly");
                return;
            }
        }
    }

    private void close() {
        try {
            in.close();
            out.close();
            socket.close();
        } catch (IOException e) {
            log.info("Cannot close socket");
        }
    }

    public Client getClient() {
        return client;
    }

    public void sendMessage(String message) {
        try {
            out.write(message.getBytes(StandardCharsets.UTF_16LE));
            out.flush();
        } catch (IOException e) {
            log.info("Cannot send a message");
        }
    }

    @Override
    public String toString() {
        return "ClientListener{" +
                "Thread=" + super.toString() +
                ", client=" + client +
                ", messages=" + messages +
                ", in=" + in +
                ", out=" + out +
                ", socket=" + socket +
                '}';
    }

    public boolean isSocketClosed() {
        return socket.isClosed();
    }
}
