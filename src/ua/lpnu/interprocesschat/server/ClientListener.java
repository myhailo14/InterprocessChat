package ua.lpnu.interprocesschat.server;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;

public class ClientListener extends Thread {

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
            e.printStackTrace();
        }
    }

    private int indexOf(byte[] bytes) {
        int i = 0;
        while (bytes[i++] != -1);
        return i - 1;
    }

    @Override
    public void run() {
        byte[] buffer = new byte[1024];
        Arrays.fill(buffer, (byte) -1);
        while (true) {
            try {
                Arrays.fill(buffer, (byte) -1);
                int read = in.read(buffer);
                if (read != -1) {
                    String strMessage = new String(buffer, 0, indexOf(buffer), StandardCharsets.UTF_16LE);
                    if (strMessage.contains("auth")) {
                        this.client.setName(strMessage.replace("auth ", ""));
                        messages.add(new Message(this.client, Message.Type.UPDATE));
                    } else if (strMessage.contains("message")) {
                        messages.add(new Message(this.client, Message.Type.MESSAGE, strMessage.replace("message ", "")));
                    }
                } else {
                    closeSocket();
                    return;
                }
            } catch (IOException e) {
                closeSocket();
                e.printStackTrace();
                return;
            }
        }
    }

    private void closeSocket() {
        try {
            socket.close();
        } catch (IOException e) {
            e.printStackTrace();
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
            e.printStackTrace();
        }
    }

}
