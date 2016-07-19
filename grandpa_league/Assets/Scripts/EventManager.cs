using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Reflection;
using UnityEngine;

public static class EventManager
{
    private static List<SimulationEvent> m_knownEvents = new List<SimulationEvent>();
    private static List<SimulationEvent> m_hiddenEvents = new List<SimulationEvent>();

    static EventManager()
    {

		XDocument topLevel = XDocument.Load(Application.dataPath + "/Data/events.xml");
		List<XElement> allEvents = topLevel.Root.Descendants("event").ToList();

        foreach (XElement simEvent in allEvents)
        {
            Requirement eventRequirements = new Requirement(Convert.ToBoolean(simEvent.Attribute("req_children").Value),
                                                            Convert.ToBoolean(simEvent.Attribute("req_parent").Value),
                                                            Convert.ToBoolean(simEvent.Attribute("req_grandpa").Value),
                                                            Convert.ToBoolean(simEvent.Attribute("req_money").Value),
                                                            Convert.ToBoolean(simEvent.Attribute("req_accept").Value));

            switch (Int32.Parse(simEvent.Attribute("type").Value))
            {
                case (int)Enums.EventType.HIDDEN:
                    m_hiddenEvents.Add(new SimulationEvent( eventRequirements,
                                                            simEvent.Attribute("name").Value,
                                                            simEvent.Attribute("description").Value,
                                                            Int32.Parse(simEvent.Attribute("id").Value),
                                                            0,
                                                            Int32.Parse(simEvent.Attribute("priority").Value)
                                                            ));
                    break;
                case (int)Enums.EventType.KNOWN:
                    m_knownEvents.Add(new SimulationEvent( eventRequirements,
                                                            simEvent.Attribute("name").Value,
                                                            simEvent.Attribute("description").Value,
                                                            Int32.Parse(simEvent.Attribute("id").Value),
                                                            1,
                                                            Int32.Parse(simEvent.Attribute("priority").Value),
                                                            Int32.Parse(simEvent.Attribute("month").Value),
                                                            Int32.Parse(simEvent.Attribute("day").Value)
                                                            ));
                    break;
                default:
                    break;
            }
        }
    }

    public static SimulationEvent GetRandomEvent()
    {
        int rand = Constants.RANDOM.Next(m_hiddenEvents.Count);
        return m_hiddenEvents[rand];
    }

    public static List<SimulationEvent> GetEventsByDate(int month, int day, int year)
    {
        List<SimulationEvent> eventsOnDay = new List<SimulationEvent>();
        foreach(SimulationEvent ev in m_knownEvents)
        {
            if (ev.EventMonth == month && ev.EventDay == day)
                eventsOnDay.Add(ev);
        }
        return eventsOnDay;
    }

    public static Func<DataManager, Requirement, int> GetEventFunctionById(int eventId)
    {
        string eventMethodName = "Event" + eventId.ToString();
        MethodInfo methodInfo = typeof(EventManager).GetMethod(eventMethodName);
        return (Func<DataManager, Requirement, int>)Delegate.CreateDelegate(typeof(Func<DataManager, Requirement, int>), methodInfo);
    }


    /* PLAYER DEFINED EVENTS BEYOND THIS POINT ONLY!!
     * PLEASE ADD APPROPRIATE FUNCTION HEADERS WITH COLLOQUIAL INFO 
     * EVENT FUNCTION BANE /MUST/ BE OF THE FORMAT Event[EVENTID] */

    //NAME: WEEKLY STAT UPGRADE DO NOT CHANGE FROM Event0
    public static int Event0(DataManager manager, Requirement requirements)
    {
        foreach(Family leagueFam in manager.LeagueFamilies)
        {
            leagueFam.ApplyStatUpgrades();
        }
		manager.PlayerFamily.ApplyStatUpgrades();

        return (int)Enums.EventOutcome.SUCCESS;
    }

    //NAME: GRANDPA WINS LOTTERY
    public static int Event1(DataManager manager, Requirement requirements)
    {
        manager.PlayerFamily.Grandpa.Money += 1000;
        return (int)Enums.EventOutcome.SUCCESS;
    }

    //NAME: GRANDPA CHANGES HIS NAME
    public static int Event2(DataManager manager, Requirement requirements)
    {
        if (requirements.Accept)
        {
            manager.PlayerFamily.Grandpa.Name = "Leeroy Jenkins";
            return (int)Enums.EventOutcome.SUCCESS;

        }
        return (int)Enums.EventOutcome.FAILURE;
    }
}