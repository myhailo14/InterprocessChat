package ua.lpnu.interprocesschat.server;

public class Application {

    public static final int PORT = 5960;

    public static void main(String[] args) {
        Server server = new Server("127.0.0.1", PORT);
        server.start();
    }

}
