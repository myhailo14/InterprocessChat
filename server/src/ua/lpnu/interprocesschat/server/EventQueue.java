package ua.lpnu.interprocesschat.server;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentLinkedQueue;

public class EventQueue<E> extends ConcurrentLinkedQueue<E> {

    private final List<Event> addEventList;

    private final List<Event> removeEventList;

    public EventQueue() {
        this.addEventList = new ArrayList<>();
        this.removeEventList = new ArrayList<>();
    }

    public void addEventHandler(Event event, EventType type) {
        if (type == EventType.ADD) addEventList.add(event);
        else if (type == EventType.REMOVE) removeEventList.add(event);
    }

    @Override
    public boolean add(E e) {
        boolean add = super.add(e);
        addEventList.forEach(Event::on);
        return add;
    }

    @Override
    public boolean remove(Object o) {
        boolean remove = super.remove(o);
        removeEventList.forEach(Event::on);
        return remove;
    }
}
