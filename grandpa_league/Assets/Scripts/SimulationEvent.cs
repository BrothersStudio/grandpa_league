using System;
using System.Collections.Generic;

public class SimulationEvent
{
    private string m_eventName = "";
    private string m_eventDescription = "";
    private int m_eventType = -1;
    private int m_eventId = -1;
    private int m_priority = -1;
    private Func<DataManager, Requirement, int> m_eventFunction = null;
    private List<string> m_outcomeStrings = new List<string>();         //TODO: add strings to events.xml and load them into here
                                                                        //possible just a list of ids in events.xml which correspond
                                                                        //to strings in strings.xml

    private Requirement m_requirements = null;
    private int m_eventMonth = 0;
    private int m_eventDay = 0;

    public SimulationEvent(Requirement requirements, string eventName, string eventDescription, int eventId, int eventType, int priority, int month=0, int day=0)
    {
        this.m_eventDescription = eventDescription;
        this.m_eventName = eventName;
        this.m_eventType = eventType;
        this.m_eventId = eventId;
        this.m_priority = priority;

        this.m_eventMonth = month;
        this.m_eventDay = day;

        this.m_requirements = requirements;

        this.m_eventFunction = EventManager.GetEventFunctionById(eventId);
    }

    public int RunEvent(DataManager currentManager)
    {
        return this.m_eventFunction(currentManager, this.m_requirements);      //TODO: need something like check the return value and return the right string for what happened
    }

    public int EventId
    {
        get { return this.m_eventId; }
    }
    public int EventMonth
    {
        get { return this.m_eventMonth; }
    }

    public Requirement Requirements
    {
        get { return this.m_requirements; }
        set { this.m_requirements = value; }        //setter ever needed?
    }
    public int EventDay
    {
        get { return this.m_eventDay; }
    }

    public string EventName
    {
        get { return this.m_eventName; }
    }

    public string EventDescription
    {
        get { return this.m_eventDescription; }
    }
}