using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Reflection;

public class EventManager
{
    private static List<SimulationEvent> m_knownEvents = new List<SimulationEvent>();
    private static List<SimulationEvent> m_hiddenEvents = new List<SimulationEvent>();

    public EventManager()
    {
        XDocument topLevel = XDocument.Load("../Data/events.xml");
        List<XElement> allEvents = topLevel.Elements("event").ToList();

        foreach (XElement simEvent in allEvents)
        {
            switch (Int32.Parse(simEvent.Attribute("type").Value))
            {
                case (int)Enums.EventType.HIDDEN:
                    m_hiddenEvents.Add(new SimulationEvent(simEvent.Attribute("name").Value,
                                                    simEvent.Attribute("description").Value,
                                                    Int32.Parse(simEvent.Attribute("id").Value),
                                                    0,
                                                    Int32.Parse(simEvent.Attribute("priority").Value)
                                                    ));
                    break;
                case (int)Enums.EventType.KNOWN:
                    m_hiddenEvents.Add(new SimulationEvent(simEvent.Attribute("name").Value,
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

    public static Action<DataManager> GetEventFunctionById(int eventId)
    {
        switch (eventId)
        {
            case 0:
                return TestEvent1;
            case 1:
                return TestEvent2;
            default:
                return null;
        }
    }
    public static void TestEvent1(DataManager manager)
    {
        manager.PlayerFamily.Grandpa.Money += 1000;
    }

    public static void TestEvent2(DataManager manager)
    {
        manager.PlayerFamily.Grandpa.Name = "Leeroy Jenkins";
    }
}