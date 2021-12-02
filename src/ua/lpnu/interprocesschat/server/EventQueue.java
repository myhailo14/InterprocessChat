package ua.lpnu.interprocesschat.server;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentLinkedQueue;

public class EventQueue<E> extends ConcurrentLinkedQueue<E> {

    private final List<AddEvent> addEventList;

    public EventQueue() {
        this.addEventList = new ArrayList<>();
    }

    public void addEventHandler(AddEvent addEvent) {
        addEventList.add(addEvent);
    }

    @Override
    public boolean add(E e) {
        boolean add = super.add(e);
        addEventList.forEach(AddEvent::on);
        return add;
    }
}
