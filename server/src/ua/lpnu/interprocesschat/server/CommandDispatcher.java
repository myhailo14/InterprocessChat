package ua.lpnu.interprocesschat.server;

import java.nio.charset.StandardCharsets;
import java.util.logging.Logger;

public class CommandDispatcher {

    private final byte[] buffer;
    private final Client client;
    private final EventQueue<Message> messages;

    private final Logger log = Logger.getLogger(CommandDispatcher.class.getName());

    public CommandDispatcher(byte[] buffer, Client client, EventQueue<Message> messages) {
        this.buffer = buffer;
        this.client = client;
        this.messages = messages;
    }

    public boolean dispatch() {
        String strMessage = new String(buffer, 0, indexOf(), StandardCharsets.UTF_16LE);
        log.info(client.getName() + " sent = [" + strMessage + "] to the server");
        if (strMessage.contains("auth")) {
            client.setName(strMessage.replace("auth ", ""));
            messages.add(new Message(client, Message.Type.UPDATE));
            return true;
        } else if (strMessage.contains("message")) {
            messages.add(new Message(client, Message.Type.MESSAGE, strMessage.replace("message ", "")));
            return true;
        }
        return false;
    }

    private int indexOf() {
        int i = 0;
        while (buffer[i++] != -1);
        return i - 1;
    }

}
