package ua.lpnu.interprocesschat.server;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

public class FileManager {

    private final String path;

    private File file;

    public FileManager(String path) {
        this.path = path;
        load();
    }

    private void load() {
        try {
            file = new File(path);

            if (!file.exists()) {
                file.createNewFile();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void log(String log) {
        try (BufferedWriter bw = new BufferedWriter(new FileWriter(file.getAbsoluteFile(), true))) {
            bw.write(log);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
