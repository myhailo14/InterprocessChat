package ua.lpnu.interprocesschat.server;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.function.Predicate;

public class EventArrayList <T> extends CopyOnWriteArrayList<T> {

    private final List<Event> removeEventList;

    private final List<Event> addEventList;

    public EventArrayList() {
        this.removeEventList = new ArrayList<>();
        this.addEventList = new ArrayList<>();
    }

    public void addEventHandler(Event event, EventType eventType) {
        if (eventType == EventType.ADD) addEventList.add(event);
        else if (eventType == EventType.REMOVE) removeEventList.add(event);
    }

    @Override
    public boolean remove(Object o) {
        boolean remove = super.remove(o);
        if (remove) removeEventList.forEach(Event::on);
        return remove;
    }

    @Override
    public boolean removeIf(Predicate<? super T> filter) {
        boolean removeIf = super.removeIf(filter);
        if (removeIf) {
            removeEventList.forEach(Event::on);
        }
        return removeIf;
    }

    @Override
    public boolean add(T t) {
        super.add(t);
        addEventList.forEach(Event::on);
        return true;
    }
}
