package ua.lpnu.interprocesschat.server;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentLinkedQueue;

public class EventQueue<E> extends ConcurrentLinkedQueue<E> {

    private final List<Event> eventList;

    public EventQueue() {
        this.eventList = new ArrayList<>();
    }

    public void addEventHandler(Event event) {
        eventList.add(event);
    }

    @Override
    public boolean add(E e) {
        boolean add = super.add(e);
        eventList.forEach(Event::on);
        return add;
    }
}
