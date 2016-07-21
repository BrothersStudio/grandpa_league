using System;
using System.Collections.Generic;

public class SimulationEvent
{
    private string m_eventName = "";
    private string m_eventDescription = "";
    private int m_eventType = -1;
    private int m_eventId = -1;
    private int m_priority = -1;
    private Func<DataManager, Requirement, Outcome> m_eventFunction = null;
    private List<string> m_outcomeStrings = new List<string>();         //TODO: add strings to events.xml and load them into here
                                                                        //possible just a list of ids in events.xml which correspond
                                                                        //to strings in strings.xml

    private Requirement m_requirements = null;
    private int m_eventMonth = 0;
    private int m_eventMaxMonth = 12;
    private int m_eventDay = 0;
    private double m_chance = 0;

    public SimulationEvent(Requirement requirements, double chance, string eventName, string eventDescription, int eventId, int eventType, int priority, string month="0", int day=0)
    {
        this.m_eventDescription = eventDescription;
        this.m_eventName = eventName;
        this.m_eventType = eventType;
        this.m_eventId = eventId;
        this.m_priority = priority;

        var monthArr = month.Split('-');
        if (monthArr.Length == 1)
            this.m_eventMonth = Int32.Parse(month);
        else
        {
            this.m_eventMonth = Int32.Parse(monthArr[0]);
            this.m_eventMaxMonth = Int32.Parse(monthArr[1]);
        }
        this.m_eventDay = day;

        this.m_requirements = requirements;
        this.m_chance = chance;

        this.m_eventFunction = EventManager.GetEventFunctionById(eventId);
    }

    public Outcome RunEvent(DataManager currentManager)
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

    public int Priority
    {
        get { return this.m_priority;  }
    }

    public int EventMonthMax
    {
        get { return this.m_eventMaxMonth;  }
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

    public double Chance
    {
        get { return this.m_chance; }
    }
}