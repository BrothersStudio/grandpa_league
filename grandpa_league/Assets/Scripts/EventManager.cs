using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Reflection;
using System.IO;
using UnityEngine;

public static class EventManager
{
    private static List<SimulationEvent> m_knownEvents = new List<SimulationEvent>();
    private static List<SimulationEvent> m_hiddenEvents = new List<SimulationEvent>();
    private static List<SimulationEvent> m_reservedEvents = new List<SimulationEvent>();

    static EventManager()
    {

        TextAsset tmp = Resources.Load("events") as TextAsset;
        TextReader reader = new StringReader(tmp.text);
        XDocument topLevel = XDocument.Load(reader);
        List<XElement> allEvents = topLevel.Root.Descendants("event").ToList();

        foreach (XElement simEvent in allEvents)
        {
            string qualification = simEvent.Attributes("qualification").Count() == 0 ? "NONE" : simEvent.Attribute("qualification").Value;

            Requirement eventRequirements = new Requirement(Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_children").Value) & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_parent").Value) & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_grandpa").Value) & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_money").Value) & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_accept").Value) & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_children").Value) >> 1 & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_parent").Value) >> 1 & 1),
                                                            Convert.ToBoolean(Int32.Parse(simEvent.Attribute("req_grandpa").Value) >> 1 & 1),
                                                            Qualification.GetQualificationByString(qualification),
                                                            simEvent.Attributes("age").Count() == 0 ? null : Convert.ToString(simEvent.Attribute("age").Value)
                                                            );

            switch (Int32.Parse(simEvent.Attribute("type").Value))
            {
                case (int)Enums.EventType.HIDDEN:
                    m_hiddenEvents.Add(new SimulationEvent( eventRequirements,
                                                            Double.Parse(simEvent.Attribute("chance").Value),
                                                            simEvent.Attribute("name").Value,
                                                            simEvent.Attribute("description").Value,
                                                            Int32.Parse(simEvent.Attribute("id").Value),
                                                            (int)Enums.EventType.HIDDEN,
                                                            Int32.Parse(simEvent.Attribute("priority").Value),
                                                            simEvent.Attributes("month").Count() == 0 ? "0" : simEvent.Attribute("month").Value
                                                            ));
                    break;
                case (int)Enums.EventType.KNOWN:
                    m_knownEvents.Add(new SimulationEvent(eventRequirements,
                                                            1.0,
                                                            simEvent.Attribute("name").Value,
                                                            simEvent.Attribute("description").Value,
                                                            Int32.Parse(simEvent.Attribute("id").Value),
                                                            (int)Enums.EventType.KNOWN,
                                                            Int32.Parse(simEvent.Attribute("priority").Value),
                                                            simEvent.Attribute("month").Value,
                                                            simEvent.Attributes("day").Count() == 0 ? 0 : Int32.Parse(simEvent.Attribute("day").Value)
                                                            ));
                    break;
                case (int)Enums.EventType.RESERVED:
                    m_reservedEvents.Add(new SimulationEvent(eventRequirements,
                                                            Double.Parse(simEvent.Attribute("chance").Value),
                                                            simEvent.Attribute("name").Value,
                                                            simEvent.Attribute("description").Value,
                                                            Int32.Parse(simEvent.Attribute("id").Value),
                                                            (int)Enums.EventType.RESERVED,
                                                            Int32.Parse(simEvent.Attribute("priority").Value)
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

    public static List<SimulationEvent> GetAllHiddenEvents()
    {
        return m_hiddenEvents;
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

    public static SimulationEvent GetEventById(int id)
    {
        foreach (SimulationEvent ev in m_hiddenEvents)
            if (ev.EventId == id)
                return ev;
        foreach (SimulationEvent ev in m_knownEvents)
            if (ev.EventId == id)
                return ev;
        return null;
    }

    public static SimulationEvent GetSystemEventById(int id)
    {
        foreach (SimulationEvent ev in m_reservedEvents)
            if (ev.EventId == id)
                return ev;
        return null;
    }

    public static List<SimulationEvent> GetEventsByMonth(int month)
    {
        List<SimulationEvent> eventsInMonth = new List<SimulationEvent>();
        foreach(SimulationEvent ev in m_knownEvents)
        {
            if (ev.EventMonth >= month && ev.EventMonthMax <= month && ev.EventDay == 0)
                eventsInMonth.Add(ev);
        }
        return eventsInMonth;
    }

    public static Func<DataManager, Requirement, Outcome> GetEventFunctionById(int eventId)
    {
        string eventMethodName = "Event" + eventId.ToString();
        MethodInfo methodInfo = typeof(EventManager).GetMethod(eventMethodName);
        return (Func<DataManager, Requirement, Outcome>)Delegate.CreateDelegate(typeof(Func<DataManager, Requirement, Outcome>), methodInfo);
    }


    /* PLAYER DEFINED EVENTS BEYOND THIS POINT ONLY!!
     * PLEASE ADD APPROPRIATE FUNCTION HEADERS WITH COLLOQUIAL INFO 
     * EVENT FUNCTION BANE /MUST/ BE OF THE FORMAT Event[EVENTID] */

    //NAME: WEEKLY STAT UPGRADE DO NOT CHANGE FROM Event0
    public static Outcome Event0(DataManager manager, Requirement requirements)
    {
        foreach(Family leagueFam in manager.LeagueFamilies)
        {
            leagueFam.ApplyStatUpgrades();
        }
		manager.PlayerFamily.ApplyStatUpgrades();

        return new Outcome((int)Enums.EventOutcome.SUCCESS, "level_upgrade_applied");
    }

    //NAME: TRADE_ACCEPT_REJECT
    public static Outcome Event1(DataManager manager, Requirement requirements)
    {
        Outcome tradeOutcome = requirements.Trade.PerformTradeAction(manager);
        return tradeOutcome;
    }

    //NAME: TEST EVENT 1
    public static Outcome Event10(DataManager manager, Requirement requirements)
    {
        manager.PlayerFamily.Grandpa.Money += 1000;

        return new Outcome((int)Enums.EventOutcome.SUCCESS, String.Format("grandpa_won_lottery: {0}", manager.PlayerFamily.Grandpa.Money));
    }

    //NAME: TEST EVENT 2
    public static Outcome Event11(DataManager manager, Requirement requirements)
    {
        if (requirements.Accept)
        {
            manager.PlayerFamily.Grandpa.Name = "Leeroy Jenkins";
            return new Outcome((int)Enums.EventOutcome.SUCCESS, String.Format("Grandpa renamed {0}", manager.PlayerFamily.Grandpa.Name));

        }
        return new Outcome((int)Enums.EventOutcome.FAILURE, "BOO!");
    }
    
    //NAME: Grandkid's class does some fingerpainting
 /*public static Outcome Event101(DataManager manager, Requirement requirements)
    {
    	int successes = 0;
    	List<string> outcome = new List<string>();
    	Outcome returnObj = new Outcome()
    	
    	if(requirements.Child.Artistry >= 65 && requirements.Child.Popularity >= 60)
    	{
    		successes+=2;
    	}
    	else if(requirements.Child.Artistry >= 60)
    	{
    		successes++;
    	}
    	
    	if(successes >= 2)
    	{
            returnObj.OutcomeDescription = String.Format("Turns out { 0}");
    	}
    } */

	// Grandpa buys a car
	public static Outcome Event1001(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept) 
		{
			if (requirements.Money > 30000) 
			{
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;
				requirements.Parent.Popularity += 20; 
				manager.PlayerFamily.Grandpa.Pride += 250;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
				returnObj.OutcomeDescription = String.Format (
					"Wow! That car is certainly going to turn heads. " +
					"{0} sees that you really do care!\n\n" +
					"{0}'s popularity way up!\n" +
					"{1}'s pride way up!", 
					requirements.Parent.Name, manager.PlayerFamily.Grandpa.Name);
			} 
			else if (requirements.Money > 2000) 
			{
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;

				requirements.Parent.Love += 5; 
				manager.PlayerFamily.Grandpa.Pride += 100;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
				returnObj.OutcomeDescription = String.Format (
					"It's the thought that counts, {0}...\n\n" +
					"{0}'s pride up.\n" +
					"{1}'s love up.", 
					requirements.Grandpa.Name, requirements.Parent.Name);
			}
			else
			{
				manager.PlayerFamily.Grandpa.Insanity += 5;
				manager.PlayerFamily.Grandpa.Pride -= 100;

				returnObj.Status = (int)Enums.EventOutcome.FAILURE;
				returnObj.OutcomeDescription = String.Format (
					"That's not nearly enough for a nice car... Are you okay, {0}?\n\n" +
					"{0}'s insanity up.\n" +
					"{0}'s pride down.", 
					manager.PlayerFamily.Grandpa.Name);
			}
		} 
		else 
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandpa sends family on vacation
	public static Outcome Event1002(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept) 
		{
			if (requirements.Money > 5000) 
			{
				foreach (Parent parent in manager.PlayerFamily.Parents) 
				{
					parent.Love += 10;
					parent.Popularity += 15;
				}
				foreach (Child child in manager.PlayerFamily.Children) 
				{
					child.Popularity += 10;
					child.Athleticism += 5;
				}
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;
				manager.PlayerFamily.Grandpa.Pride += 200;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
				returnObj.OutcomeDescription = String.Format (
					"Wow! That car is certainly going to turn heads. " +
					"{0} sees that you really do care!\n\n" +
					"{0}'s popularity way up!\n" +
					"{1}'s pride way up!", 
					requirements.Parent.Name, manager.PlayerFamily.Grandpa.Name);
			} 
			else
			{
				foreach (Parent parent in manager.PlayerFamily.Parents) 
				{
					parent.Love -= 5;
				}
				manager.PlayerFamily.Grandpa.Pride -= 50;

				returnObj.Status = (int)Enums.EventOutcome.FAILURE;
				returnObj.OutcomeDescription = String.Format (
					"That's not nearly enough money for the vacation we had in mind...\n\n" +
					"{0}'s pride down.\n" +
					"All parents' love down.", 
					manager.PlayerFamily.Grandpa.Name);
			}
		} 
		else 
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandkid tries out for football team
	public static Outcome Event1003(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		requirements.Child.Athleticism = 51;
		if (requirements.Child.Athleticism > 50) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.Athleticism += 10;
			requirements.Child.AthleticismGrowth += 0.1;
			requirements.Child.Popularity += 10;
			requirements.Child.PopularityGrowth += 0.1;

			requirements.Child.Artistry -= 5;

			manager.PlayerFamily.Grandpa.Pride += 150;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Great ball tossing, {0}! Now everyone will value you more as a person!\n\n" +
				"{0}'s athleticism up.\n" + 
				"{0}'s popularity up.\n" + 
				"{0}'s artistry down.\n" + 
				"{1}'s pride up.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		else
		{
			requirements.Child.Athleticism -= 5;
			requirements.Child.Popularity -= 5;
			requirements.Child.PopularityGrowth -= 0.01;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"Well, you didn't get on the indoor football team. That's disappointing, {0}...\n\n" +
				"{0}'s athleticism down.\n" + 
				"{0}'s popularity down.\n" + 
				"{1}'s pride down.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		return returnObj;
	}

	// Grandkid is dating head cheerleader
	public static Outcome Event1008(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Popularity > 80 && requirements.Child.Athleticism > 50) 
		{
			requirements.Child.Popularity += 20;
			requirements.Child.PopularityGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 150;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Wow, {1} is so popular and talented at indoor football that they are dating the head cheerleader!\n\n" +
				"{1}'s popularity up.\n" + 
				"{1}'s popularity growth up.\n" + 
				"{0}'s pride up.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Child.Name);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandkid critically injured
	public static Outcome Event1009(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism < 80) 
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.Popularity += 10;
			requirements.Child.Athleticism -= 30;
			requirements.Child.AthleticismGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"Oh God, seeing your leg fly right out of the socket during that indoor football game was brutal! I guess you're off the indoor football team for the year.\n\n" +
				"{1}'s popularity up.\n" +
				"{1}'s athleticism way down.\n" +
				"{0}'s pride down.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Child.Name);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandkid great at practice
	public static Outcome Event1010(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism > 70) 
		{
			requirements.Child.Athleticism += 10;
			requirements.Child.AthleticismGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Holy cow, {1} is just kicking ass at indoor football practice. It's clear to everyone that they're a very skilled player!\n\n" +
				"{1}'s athleticism up.\n" +
				"{1}'s athleticism growth up.\n" +
				"{0}'s pride up.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Child.Name);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandpa buys experimental football gear
	public static Outcome Event1011(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Money >= 500.0 && requirements.Accept) 
		{
			requirements.Child.Athleticism += 20;
			requirements.Child.AthleticismGrowth += 0.1;

			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));

			manager.PlayerFamily.Grandpa.Pride += 100;
			manager.PlayerFamily.Grandpa.Money -= requirements.Money;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Those Cambodians never disappoint! {0} is untouchable at practice now! I just hope no one finds out...\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"{1}'s pride up.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandson's illegal gear discovered
	public static Outcome Event1012(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Money >= 1000.0 && requirements.Accept) 
		{
			manager.PlayerFamily.Grandpa.Money -= requirements.Money;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"\"Nice doing business with you, gramps!\"\nDamn those Cambodians... Damn them straight to hell...");
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.FAILURE;

			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.AthleticismGrowth -= 0.2;
			requirements.Child.Athleticism -= 30;
			requirements.Child.Popularity -= 30;

			manager.PlayerFamily.Grandpa.Pride -= 200;

			returnObj.OutcomeDescription = String.Format (
				"You're gonna regret that pal! \"{0}, you're off the team!\"\nOh! My pride! It physically hurts!\n\n" +
				"{0}'s athleticism way down!\n" +
				"{0}'s athleticism growth way down!\n" +
				"{0}'s popularity way down!\n" +
				"{1}'s pride way down!\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		return returnObj;
	}

	// Grandpa inspects his social security benefits
	public static Outcome Event1004(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Wisdom > 50) 
		{
			manager.PlayerFamily.Grandpa.Wisdom += 5;
			manager.PlayerFamily.Grandpa.Insanity -= 5;

			manager.PlayerFamily.Grandpa.MoneyGrowth += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Going through your social security payments, you find a nice loophole. That'll increase your weekly income! Thanks Obama! \n\n" +
				"{0}'s wisdom up.\n" + 
				"{0}'s insanity down.\n" + 
				"{0}'s income up.\n",
				manager.PlayerFamily.Grandpa.Name);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}

	// Grandkid makes honor roll
	public static Outcome Event1005(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Intelligence > 60) 
		{
			requirements.Child.Intelligence += 10;
			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += 10;
			}

			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} has made the honor roll! Wow, nice job!\n\n" +
				"{0}'s intelligence up.\n" + 
				"All parents' love up.\n" + 
				"{1}'s pride up.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		else  
		{
			requirements.Child.Intelligence -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} missed the honor roll... What a dummy...\n\n" +
				"{0}'s intelligence down.\n" + 
				"{1}'s pride down.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		return returnObj;
	}

	// Grandkid tries out for play
	public static Outcome Event1006(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Artistry > 90 && requirements.Child.Popularity > 50) 
		{
			requirements.Child.Artistry += 10;
			requirements.Child.ArtistryGrowth += 0.1;

			requirements.Child.Popularity += 15;
			requirements.Child.PopularityGrowth += 0.02;

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += 10;
			}

			manager.PlayerFamily.Grandpa.Pride += 300;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} is the lead role in the play! Very impressive! Grandpa is visibly pleased.\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth way up.\n" + 
				"{0}'s popularity up.\n" + 
				"{0}'s popularity growth up.\n" + 
				"All parents' love up.\n" + 
				"{1}'s pride way up.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		else if (requirements.Child.Artistry > 60) 
		{
			requirements.Child.Artistry += 10;
			requirements.Child.ArtistryGrowth += 0.05;

			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} got a role in the play!\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth up.\n" + 
				"{1}'s pride up.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		else 
		{
			requirements.Child.Artistry += 5;
			requirements.Child.Popularity -= 5;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Oof, wow, that was really embarassing. Maybe next play?\n\n" +
				"{0}'s artistry slightly up.\n" +
				"{0}'s popularity slightly down.\n" + 
				"{1}'s pride down.\n",
				requirements.Child.Name, manager.PlayerFamily.Grandpa.Name);
		}
		return returnObj;
	}

	// Grandpa finds buried gold
	public static Outcome Event1007(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Wisdom > 30 && manager.PlayerFamily.Grandpa.Insanity < 70) 
		{
			manager.PlayerFamily.Grandpa.Wisdom += 5;
			manager.PlayerFamily.Grandpa.Insanity -= 5;

			int found_amount = 1000 + Constants.RANDOM.Next (1000, 2000);
			manager.PlayerFamily.Grandpa.Money += found_amount;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa was looking through his old treasure maps and remembered ${1} worth of confederate gold he buried many years ago! How lucky!\n\n" +
				"{0}'s wisdom up.\n" + 
				"{0}'s insanity down.\n" + 
				"{0}'s money up.\n",
				manager.PlayerFamily.Grandpa.Name, found_amount);
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		}
		return returnObj;
	}
}
