using System;

public class SimulationEvent
{
    private string m_eventName = "";
    private string m_eventDescription = "";
    private int m_eventType = -1;
    private int m_eventId = -1;
    private int m_priority = -1;
    private Action<DataManager> m_eventFunction = null;

    private int m_eventMonth = 0;
    private int m_eventDay = 0;

    public SimulationEvent(string eventName, string eventDescription, int eventId, int eventType, int priority, int month=0, int day=0)
    {
        this.m_eventDescription = eventDescription;
        this.m_eventName = eventName;
        this.m_eventType = eventType;
        this.m_eventId = eventId;
        this.m_priority = priority;

        this.m_eventMonth = month;
        this.m_eventDay = day;

        this.m_eventFunction = EventManager.GetEventFunctionById(eventId);
    }

    public void RunEvent(DataManager currentManager)
    {
        this.m_eventFunction(currentManager);
    }

    public int EventId
    {
        get { return this.m_eventId; }
    }
    public int EventMonth
    {
        get { return this.m_eventMonth; }
    }

    public int EventDay
    {
        get { return this.m_eventDay; }
    }
}