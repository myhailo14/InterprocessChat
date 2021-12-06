package ua.lpnu.interprocesschat.server;

public class Message {

    public enum Type {
        UPDATE, MESSAGE;
    }

    private final Client author;

    private final Type type;

    private final String[] content;

    public Message(Client author, Type type, String ... content) {
        this.author = author;
        this.type = type;
        this.content = content;
    }

    public Type getType() {
        return type;
    }

    public Client getAuthor() {
        return author;
    }

    public String[] getContent() {
        return content;
    }

}
