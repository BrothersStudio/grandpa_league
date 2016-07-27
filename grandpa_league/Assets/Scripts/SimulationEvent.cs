using System;
using System.Collections.Generic;

[Serializable]
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
    private Outcome m_outcome = null;

    public SimulationEvent(Requirement requirements, double chance, string eventName, string eventDescription, int eventId, int eventType, int priority, string month="0", int day=0)
    {
        this.m_eventDescription = eventDescription;
        this.m_eventName = eventName;
        this.m_eventType = eventType;
        this.m_eventId = eventId;
        this.m_priority = priority;

        var monthArr = month.Split('-');
        if (monthArr.Length == 1)
        {
            this.m_eventMonth = Int32.Parse(month);
            this.m_eventMaxMonth = this.m_eventMonth;
        }
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
        try
        {
            this.m_outcome = this.m_eventFunction(currentManager, this.m_requirements);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(string.Format("{0}, {1}, {2}", e.Source, e.Message, e.StackTrace));
            this.m_outcome = new Outcome();
            this.m_outcome.Mail = new Mail();
            this.m_outcome.Mail.Subject = "Re: oops!";
            this.m_outcome.Mail.Sender = "Three Brothers Studio Chris";
            this.m_outcome.Mail.Message = string.Format("Oops,\n\n Sorry about that little hiccup. Your event crashed (we weren't expecting that to happen). If you see any developers around tell them this:\n\n{0}\n{1}\n{2}", e.Source, e.Message, e.StackTrace.Substring(0, 250));
        }
        return this.m_outcome;
    }

    public void FormatEventDescription(DataManager currentManager)
    {
        try
        {
            if (this.m_eventDescription.Contains("{G}"))
                this.m_eventDescription = this.m_eventDescription.Replace("{G}", currentManager.PlayerFamily.Grandpa.Name);
            if (this.m_eventDescription.Contains("{EG}"))
                this.m_eventDescription = this.m_eventDescription.Replace("{EG}", this.m_requirements.Grandpa.Name);
            if (this.m_eventDescription.Contains("{C}"))
                this.m_eventDescription = this.m_eventDescription.Replace("{C}", this.m_requirements.Child.Name);
            if (this.m_eventDescription.Contains("{P}"))
                this.m_eventDescription = this.m_eventDescription.Replace("{P}", this.m_requirements.Parent.Name);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }

    public int EventId
    {
        get { return this.m_eventId; }
    }
    public int EventMonth
    {
        get { return this.m_eventMonth; }
        set { this.m_eventMonth = value;  }
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

    public int EventType
    {
        get { return this.m_eventType; }
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