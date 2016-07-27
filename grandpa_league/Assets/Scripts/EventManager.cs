using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Reflection;
using System.IO;
using UnityEngine;

[Serializable]
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
        foreach (SimulationEvent ev in m_reservedEvents)
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
            if (month >= ev.EventMonth && month <= ev.EventMonthMax && ev.EventDay == 0)
            {
                int rand = Constants.RANDOM.Next(ev.EventMonth, ev.EventMonthMax);          //choose a random number between the months the ev occurs e.g. 2-6
                if (rand == (ev.EventMonth + ev.EventMonthMax) / 2)                         //if rand <= 4, let add it for the current month
                    eventsInMonth.Add(ev);
                else
                    ev.EventMonth++;                                                        //else move its month up by one. this way if EventMonth == EventMonthMax it will always be added
            }
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
        Outcome statOutcome = new Outcome();
        statOutcome.Status = (int)Enums.EventOutcome.SUCCESS;
        statOutcome.Mail = new Mail();
        statOutcome.Mail.Date = manager.Calendar.GetCurrentDay();
        statOutcome.Mail.Subject = "Your Social Security Check";
        statOutcome.Mail.Sender = manager.PlayerFamily.Parents[0].Name;
        statOutcome.Mail.Message = string.Format("Hey Dad,\n\n\tHere is your social security check for the month plus a little something extra I scraped up for you. The kids are doing just fine and are growing so fast! You'll barely even recognize them soon (between you and me, I'm worred {0} is already getting uglier like Mom did. Anyway have a good month!\n\nTotal Amount Applied to Account: ${1}.00\n\nLove,\n{2}",manager.PlayerFamily.Children[0] ,manager.PlayerFamily.Grandpa.MoneyGrowth, manager.PlayerFamily.Parents[0].Name);
        return statOutcome;
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
			if (requirements.Money >= 30000) 
			{
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;
				requirements.Parent.Popularity += 20; 
				manager.PlayerFamily.Grandpa.Pride += 250;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"Wow! That car is certainly going to turn heads. Thanks for the contribution, Grandpa!" +
					"{0} sees that you really do care!\n\n" +
					"{0}'s popularity way up!\n" +
					"Grandpa's pride way up!", 
					requirements.Parent.Name);
			} 
			else if (requirements.Money >= 2000) 
			{
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;

				requirements.Parent.Love += 5; 
				manager.PlayerFamily.Grandpa.Pride += 100;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That's not going to buy a nice car, Grandpa. More like a total clunker. Maybe back in your day! I'll take it anyway!\n\n" +
					"{0}'s love up.\n" +
					"Grandpa's pride up.",
					requirements.Parent.Name);
			}
			else
			{
				manager.PlayerFamily.Grandpa.Insanity += 5;
				manager.PlayerFamily.Grandpa.Pride -= 100;

				returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That's not nearly enough for a nice car... Are you okay, Grandpa?\n\n" +
					"Grandpa's insanity up.\n" +
					"Grandpa's pride down.");
			}
		} 
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;
		
		return returnObj;
	}

	// Grandpa sends family on vacation
	public static Outcome Event1002(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept) 
		{
			if (requirements.Money >= 5000) 
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

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"Wow! That car is certainly going to turn heads. " +
					"{0} sees that you really do care!\n\n" +
					"{0}'s popularity way up!\n" +
					"Grandpa's pride way up!", 
					requirements.Parent.Name);
			} 
			else
			{
				foreach (Parent parent in manager.PlayerFamily.Parents) 
				{
					parent.Love -= 5;
				}
				manager.PlayerFamily.Grandpa.Pride -= 50;

				returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That's not nearly enough money for the vacation we had in mind...\n\n" +
					"All parents' love down.\n" +
					"Grandpa's pride down.");
			}
		} 
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;
		
		return returnObj;
	}

	// Grandkid tries out for football team
	public static Outcome Event1003(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism >= 50) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.Athleticism += 10;
			requirements.Child.AthleticismGrowth += 0.1;
			requirements.Child.Popularity += 10;
			requirements.Child.PopularityGrowth += 0.1;

			requirements.Child.Artistry -= 5;

			manager.PlayerFamily.Grandpa.Pride += 150;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Great ball tossing, {0}! Now everyone will value you more as a person!\n\n" +
				"{0}'s athleticism up.\n" + 
				"{0}'s popularity up.\n" + 
				"{0}'s artistry down.\n" + 
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else
		{
			requirements.Child.Athleticism -= 5;
			requirements.Child.Popularity -= 5;
			requirements.Child.PopularityGrowth -= 0.01;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well, you didn't get on the indoor football team. That's disappointing, {0}...\n\n" +
				"{0}'s athleticism down.\n" + 
				"{0}'s popularity down.\n" + 
				"Grandpa's pride down.",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid is dating head cheerleader
	public static Outcome Event1008(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Popularity >= 80 && requirements.Child.Athleticism >= 50) 
		{
			requirements.Child.Popularity += 20;
			requirements.Child.PopularityGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 150;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Wow, {0} is so popular and talented at indoor football that they are dating the head cheerleader!\n\n" +
				"{0}'s popularity up.\n" + 
				"{0}'s popularity growth up.\n" + 
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
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

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Oh God, seeing your leg fly right out of the socket during that indoor football game was brutal! I guess you're off the indoor football team for the year.\n\n" +
				"{0}'s popularity up.\n" +
				"{0}'s athleticism way down.\n" +
				"Grandpa's pride down.",
				requirements.Child.Name);
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandkid great at practice
	public static Outcome Event1010(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism >= 70) 
		{
			requirements.Child.Athleticism += 10;
			requirements.Child.AthleticismGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Holy cow, {0} is just kicking ass at indoor football practice. It's clear to everyone that they're a very skilled player!\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"Grandpa's pride up.\n",
				requirements.Child.Name);
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandpa buys experimental football gear
	public static Outcome Event1011(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept && manager.PlayerFamily.Grandpa.Money >= 500) 
		{
			requirements.Child.Athleticism += 20;
			requirements.Child.AthleticismGrowth += 0.1;

			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));

			manager.PlayerFamily.Grandpa.Pride += 100;
			manager.PlayerFamily.Grandpa.Money -= 500;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Those Cambodians never disappoint! {0} is untouchable at practice now! I just hope no one finds out...\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else if (requirements.Accept && manager.PlayerFamily.Grandpa.Money < 500) 
		{
			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"\"Hey, what are you trying to pull, gramps? You don't have the cash...\"\n\n");
		}
		else
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;
		
		return returnObj;
	}

	// Grandkid's illegal gear discovered
	public static Outcome Event1012(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept && manager.PlayerFamily.Grandpa.Money >= 1000) 
		{
			manager.PlayerFamily.Grandpa.Money -= 1000;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"\"Nice doing business with you, gramps!\"\nDamn those Cambodians... Damn them straight to hell...");
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;

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
				"Grandpa's pride way down!\n",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid's football championship
	public static Outcome Event1013(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism >= 90) {
			requirements.Child.Athleticism += 20;
			requirements.Child.AthleticismGrowth += 0.2;

			requirements.Child.Popularity += 20;
			requirements.Child.PopularityGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 600;

			requirements.Grandpa.Pride -= 200;
			requirements.Grandpa.Insanity += 10;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"It's the most intense game of indoor football ever displayed! {0} scores every goal personally. " +
				"The entire indoor stadium are on their feet except {1}. During the last goal, {1}'s son is taken off the field on a stretcher!" +
				"But {0}'s team wins in the end. Of course.\n\n" +
				"{0}'s athleticism way up.\n" +
				"{0}'s athleticism growth way up.\n" +
				"{0}'s popularity way up.\n" +
				"{0}'s popularity growth way up.\n" +
				"Grandpa's pride way way way up!!\n",
				requirements.Child.Name, requirements.Grandpa.Name);
		} 
		else if (requirements.Child.Athleticism >= 60) 
		{
			requirements.Child.Athleticism += 10;
			requirements.Child.AthleticismGrowth += 0.05;

			manager.PlayerFamily.Grandpa.Pride += 200;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1}'s son's team takes an early lead. {0} puts on a respectable display of resistance. They even put their elbow through" +
				"Some poor kid's face! That's one for the highlight reel! Ultimately, {0} loses... But Grandpa is still proud! Maybe next year.\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"Grandpa's pride way up!\n",
				requirements.Child.Name, requirements.Grandpa.Name);
		} 
		else 
		{
			requirements.Child.Athleticism -= 10;
			requirements.Child.AthleticismGrowth -= 0.05;

			requirements.Child.Popularity -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Uhh, are you sure {0} was practicing all those times they said they were heading to practice? You really couldn't tell by their display " +
				"on the field. Utterly embarassing. Grandpa had to slink out the back at the end of the first quarter. {1} will never let him forget it.\n\n" +
				"{0}'s athleticism down.\n" +
				"{0}'s athleticism growth down.\n" +
				"{0}'s popularity down.\n" +
				"Grandpa's pride down.\n",
				requirements.Child.Name, requirements.Grandpa.Name);
		}
		requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));
		requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));
		return returnObj;
	}

	// Grandkid sucks at football
	public static Outcome Event1014(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Athleticism < 40) 
		{
			requirements.Child.AthleticismGrowth -= 0.1;
			requirements.Child.Popularity -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Wow, there's no way to put this lightly so I'm just going to come out and say it. {0} is just completely awful at football. " +
				"I'm not sure what happened. They got on the team okay. Now all the other kids laugh and throw balls at {1}.\n\n" +
				"{0}'s athleticism growth down.\n" +
				"{0}'s popularity down.\n" +
				"Grandpa's pride down.\n",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Gender) ? "her" : "him");
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandpa inspects his social security benefits
	public static Outcome Event1004(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Wisdom >= 50) 
		{
			manager.PlayerFamily.Grandpa.Wisdom += 5;
			manager.PlayerFamily.Grandpa.Insanity -= 5;

			manager.PlayerFamily.Grandpa.MoneyGrowth += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Going through your social security payments, you find a nice loophole. That'll increase your weekly income! Thanks Obama!\n\n" +
				"Grandpa's wisdom up.\n" + 
				"Grandpa's insanity down.\n" + 
				"Grandpa's income up by 100 per week!\n");
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandkid makes honor roll
	public static Outcome Event1005(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Intelligence >= 60) 
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
				"Grandpa's pride up.\n",
				requirements.Child.Name);
		}
		else  
		{
			requirements.Child.Intelligence -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} missed the honor roll... What a dummy...\n\n" +
				"{0}'s intelligence down.\n" + 
				"Grandpa's pride down.\n",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid tries out for play
	public static Outcome Event1006(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Artistry >= 90 && requirements.Child.Popularity >= 50) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += 10;
			requirements.Child.ArtistryGrowth += 0.1;

			requirements.Child.Popularity += 15;
			requirements.Child.PopularityGrowth += 0.02;

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += 10;
			}

			manager.PlayerFamily.Grandpa.Pride += 300;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{0} got the lead role in the play! As well as several other roles! {1} performance left everyone " +
				"in tears and it instantly began raining outside. They say drama will never be the same.\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth way up.\n" + 
				"{0}'s popularity up.\n" + 
				"{0}'s popularity growth up.\n" + 
				"All parents' love up.\n" + 
				"Grandpa's pride way up.\n",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Name) ? "Her" : "His");
		}
		else if (requirements.Child.Artistry >= 40 || requirements.Child.Popularity >= 60) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += 10;
			requirements.Child.ArtistryGrowth += 0.05;

			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{0} only messed up a handful of lines and fell off the stage only once or twice. Not too shabby! " +
				"{0} got a minor role in the play!\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth up.\n" + 
				"Grandpa's pride up.\n",
				requirements.Child.Name);
		}
		else 
		{
			requirements.Child.Artistry += 5;
			requirements.Child.Popularity -= 5;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Oof, wow, that was really hard to watch. Running off the stage in tears will probably hurt your reputation" +
				" a bit.\n\n" +
				"{0}'s artistry slightly up.\n" +
				"{0}'s popularity slightly down.\n" + 
				"Grandpa's pride down.\n",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid forgets lines at practice
	public static Outcome Event1015(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Artistry < 50) 
		{

			requirements.Child.Artistry -= 5;
			requirements.Child.ArtistryGrowth -= 0.01;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} shows up to play practice late practically every time and never seems to know {1} lines. " +
				"People are starting to talk...\n\n" +
				"{0}'s artistry slightly down.\n" + 
				"{0}'s artistry growth slightly down.\n" + 
				"Grandpa's pride slightly down.\n",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Name) ? "her" : "his");
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandkid's opening night
	public static Outcome Event1016(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Artistry >= 70) 
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += 20;
			requirements.Child.ArtistryGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 200;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"The play is a smash hit! The standing ovation has been going on for over 20 minutes already. Remarkable! People are shouting {0}'s " +
				"name at the top of their lungs. The lemur foundation can't count all the money they're making. The dream of having lemurs be as common " +
				"as rats is on the way to reality.\n\n" +
				"{0}'s artistry way up!\n" + 
				"{0}'s artistry growth up.\n" + 
				"Grandpa's pride way up!\n",
				requirements.Child.Name);
		}
		else 
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Popularity -= 10;
			requirements.Child.PopularityGrowth -= 0.05;

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well that could have gone better. People are killing lemurs in the streets. They are calling this the worst play of all time. {0} will never " +
				"live this down at school for being a part of it.\n\n" +
				"{0}'s popularity down!\n" + 
				"{0}'s popularity growth down.\n" + 
				"Grandpa's pride down.\n",
				requirements.Child.Name);
		}
		return returnObj;
	}
		
	// Grandpa finds buried gold
	public static Outcome Event1007(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Wisdom >= 30 && manager.PlayerFamily.Grandpa.Insanity < 70) 
		{
			manager.PlayerFamily.Grandpa.Wisdom += 5;
			manager.PlayerFamily.Grandpa.Insanity -= 5;

			int found_amount = 1000 + Constants.RANDOM.Next (1000, 2000);
			manager.PlayerFamily.Grandpa.Money += found_amount;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa was looking through his old treasure maps and remembered ${0} worth of confederate gold he buried many years ago! How lucky!\n\n" +
				"Grandpa's wisdom up.\n" + 
				"Grandpa's insanity down.\n" + 
				"Grandpa's money up.\n",
				found_amount);
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Parent gets a divorce 
	public static Outcome Event1017(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Parent.Love < 30 && manager.PlayerFamily.Parents.Count > 1) 
		{
			manager.PlayerFamily.Parents.Remove (requirements.Parent);

			string other_parent = "";
			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				Debug.Log ("Remaining parent: " + parent.Name);
				other_parent = parent.Name;
				parent.Love -= 30;
				parent.AddQualification (Qualification.GetQualificationByString ("IS_SINGLE"));
			}

			manager.PlayerFamily.Grandpa.Pride -= 200;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} has had it! What has this family done for {1} anyway?! {0} could have been an astronaut " +
				"or a plumber or a runway model but no... {3} gave it all up... For what?! {0} wants a divorce!! " +
				"{2} and the kids are inconsolable.\n\n" +
				"{0} has left the family!\n" + 
				"{2}'s love way way down!\n" + 
				"Grandpa's pride way down!\n",
				requirements.Parent.Name, Convert.ToBoolean(requirements.Parent.Gender) ? "her" : "him", other_parent, 
				Convert.ToBoolean(requirements.Parent.Gender) ? "She" : "He");
		}
		else if (manager.PlayerFamily.Parents.Count > 1)
		{
			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += 10;
			}

			manager.PlayerFamily.Grandpa.Wisdom += 10;
			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"It's obvious to everyone around them how in love {0} and {1} are! Grandpa feels proud for setting them " +
				"up all those years ago despite their protestations!\n\n" +
				"{0}'s love up.\n" + 
				"{1}'s love up.\n" + 
				"Grandpa's wisdom up.\n" + 
				"Grandpa's pride up.\n",
				manager.PlayerFamily.Parents[0].Name, manager.PlayerFamily.Parents[1].Name);
		}
		else
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Parent gets in car accident 
	public static Outcome Event1018(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Parents.Count > 1) 
		{
			manager.PlayerFamily.Parents.Remove (requirements.Parent);

			string other_parent = "";
			foreach (Parent parent in manager.PlayerFamily.Parents) {
				Debug.Log ("Remaining parent: " + parent.Name);
				other_parent = parent.Name;
				parent.Love += 20;
				parent.LoveGrowth -= 0.05;
				parent.Popularity += 20;
				parent.AddQualification (Qualification.GetQualificationByString ("IS_SINGLE"));
			}

			manager.PlayerFamily.Grandpa.Pride -= 100;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} was driving along the precarious cliffs on the edge of town to get the family a pizza and a really " +
				"unfortunately timed gust of wind threw {1} car straight into a bottomless pit. Yo, that, like, totally sucks. " +
				"Sorry.\n\n" +
				"{0} has left the family!\n" +
				"{2}'s love way up!\n" +
				"{2}'s love growth down!\n" +
				"{2}'s popularity way up!\n" +
				"Grandpa's pride down!\n",
				requirements.Parent.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "her" : "his", other_parent);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandkid throws party
	public static Outcome Event1019(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if ((requirements.Parent.Love >= 30 && requirements.Child.Popularity >= 70 && requirements.Accept) ||
		    (requirements.Child.Popularity >= 40 && requirements.Money >= 200 && requirements.Accept)) 
		{


			requirements.Child.Popularity += 20;
			requirements.Child.PopularityGrowth += 0.2;

			requirements.Parent.Popularity += 20;
			requirements.Parent.Love += 10;
			requirements.Parent.PopularityGrowth += 0.1;

			manager.PlayerFamily.Grandpa.Pride += 400;
			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.Money -= requirements.Money;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Holy shit. It's all kind of a blur, but that party was off the hook! Where did that elephant come from? I've never seen Grandpa dance like that. " +
				"It's so cool that your {1} let us throw that! Can't believe Barack Obama showed up. I want to remember last night for the rest of my life. " +
				"You're sooooo cool, {2}!\n\n" +
				"{2}'s popularity way up!\n" +
				"{2}'s popularity growth way up!\n" +
				"{3}'s popularity way up!\n" +
				"{3}'s popularity growth up!\n" +
				"Grandpa's pride way way up!!\n" +
				"Grandpa's insanity up.\n",
				manager.PlayerFamily.Grandpa.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "mom" : "dad", requirements.Child.Name, requirements.Parent.Name);
		} 
		else if (!requirements.Accept) 
		{
			requirements.Child.Popularity -= 5;
			requirements.Child.PopularityGrowth -= 0.05;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Lame! Don't you ever have any fun? You're wrong, it would have been a totally rad party... \nSniff... \n\n" +
				"{0}'s popularity slightly down.\n" +
				"{0}'s popularity growth slightly down.\n",
				requirements.Child.Name);
		} 
		else 
		{
			requirements.Child.Popularity -= 20;
			requirements.Child.PopularityGrowth -= 0.1;

			manager.PlayerFamily.Grandpa.Wisdom -= 10;
			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth += 0.05;
			manager.PlayerFamily.Grandpa.Money -= requirements.Money;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well, that was the might awkward night of {0}'s life. No amount of money would have made that fun. Who let that kid throw a party? {1}'s totally a loser... And who brought " +
				"that live tiger? Was that you, Grandpa?\n\n" +
				"{0}'s popularity way down!\n" +
				"{0}'s popularity growth way down!\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up.\n",
				requirements.Child.Name, Convert.ToBoolean (requirements.Child.Gender) ? "She" : "He");
		}

		return returnObj;
	}

	// Parent injured at construction site
	public static Outcome Event1020(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Parent.Intelligence < 40)
		{
			requirements.Parent.Intelligence -= 10;
			requirements.Parent.Popularity -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} was walking through a construction site the other day and got hit by a girder! Why was {1} even walking through there? " +
				"What a dummy. What a disgrace...\n\n" +
				"{0}'s intelligence down.\n" +
				"{0}'s popularity down!\n" +
				"Grandpa's pride down slightly.\n",
				requirements.Parent.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "she" : "he");
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandkid gets into Harvard
	public static Outcome Event1021(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Child.Intelligence > 90 && requirements.Child.Artistry > 50 && requirements.Child.Athleticism > 50)
		{
			foreach (Child child in manager.PlayerFamily.Children) 
			{
				child.Intelligence += 10;
				child.IntelligenceGrowth += 0.05;
			}

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Intelligence += 10;
				parent.IntelligenceGrowth += 0.05;
			}

			manager.PlayerFamily.Grandpa.Pride += 200;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Awesome! {0} just got into Harvard! {1} is going for Nautical Archaeology. It's been {2} passion for as long as you can remember. " +
				"You feel your entire family get smarter just by being in contact with {2}!\n\n" +
				"Entire family's intelligence up!!\n" +
				"Entire family's intelligence growth up!!\n" +
				"Grandpa's pride way up!!\n",
				requirements.Child.Name, Convert.ToBoolean (requirements.Child.Gender) ? "She" : "He", Convert.ToBoolean (requirements.Child.Gender) ? "her" : "his");
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa buys family dog
	public static Outcome Event1022(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 50)
		{
			requirements.Child.Intelligence -= 10;

			manager.PlayerFamily.Grandpa.Wisdom -= 10; 

			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth += 0.02;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa brought home a doggy! We were all so happy until it started foaming at the mouth and bit {1} right in the face! " +
				"Turns out Grandpa found it by the side of the road... Is Grandpa going to be okay, {2}?\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up.\n" +
				"Grandpa's pride down slightly.\n" +
				"{1}'s intelligence down.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Child.Name, requirements.Parent.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa lost at sea
	public static Outcome Event1023(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 50)
		{
			manager.PlayerFamily.Grandpa.Wisdom -= 10; 

			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth += 0.02;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1} just got off the phone with the coast guard. They finally found Grandpa. Turns out he wandered into a shipping container " +
				"and got halfway around the world before someone noticed! I'm getting worried about him...\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up.\n" +
				"Grandpa's pride down slightly.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Parent.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa gives life advice
	public static Outcome Event1024(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		int roll = Constants.RANDOM.Next (0, 1);
		if (manager.PlayerFamily.Grandpa.Wisdom > 50 && roll == 0) 
		{
			// Adult
			requirements.Parent.Intelligence += 10;
			requirements.Parent.Love += 10;
			requirements.Parent.Popularity += 10;

			manager.PlayerFamily.Grandpa.Wisdom += 10; 

			manager.PlayerFamily.Grandpa.Insanity -= 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth -= 0.05;

			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{1} was having a really rough day so Grandpa sat {2} down and gave {2} a life advice talk. {1} was completely reinvigorated and has a " +
				"new lease on life! That's the power of Grandpa, bitch!\n\n" +
				"{1} ALL STATS UP!!\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's insanity growth down.\n" +
				"Grandpa's pride up.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Parent.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "her" : "him");
		} 
		else if (manager.PlayerFamily.Grandpa.Wisdom > 50 && roll == 1) 
		{
			// Child
			requirements.Child.Intelligence += 10;
			requirements.Child.Athleticism += 10;
			requirements.Child.Popularity += 10;
			requirements.Child.Artistry += 10;
			requirements.Child.Cuteness += 10;

			manager.PlayerFamily.Grandpa.Wisdom += 10; 

			manager.PlayerFamily.Grandpa.Insanity -= 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth -= 0.05;

			manager.PlayerFamily.Grandpa.Pride += 100;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{1} was having a really rough day so Grandpa sat {2} down and gave {2} a life advice talk. {1} was completely reinvigorated and has a " +
				"new lease on life! That's the power of Grandpa, bitch!\n\n" +
				"{1} ALL STATS UP!!\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's insanity growth down.\n" +
				"Grandpa's pride up.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Child.Name, Convert.ToBoolean (requirements.Child.Gender) ? "her" : "him");
		}
		else
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa loses money
	public static Outcome Event1025(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 70)
		{
			double money_lost = manager.PlayerFamily.Grandpa.Money / 2;
			manager.PlayerFamily.Grandpa.Money -= money_lost;

			manager.PlayerFamily.Grandpa.Wisdom -= 10; 
			manager.PlayerFamily.Grandpa.WisdomGrowth -= 0.03;

			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth += 0.02;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa just lost ${1} on a ponzi scheme! He actually thought he could get rich quick selling plungers door to door. " +
				"Grandpa seems like he's losing more of his independance every day...\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's wisdom growth.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up.\n" +
				"Grandpa's pride down slightly.\n" +
				"Grandpa lost {1} dollars.\n",
				manager.PlayerFamily.Grandpa.Name, money_lost.ToString());
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// CHECK: Grandpa commits arson
	public static Outcome Event1030(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 30)
		{
			manager.Calendar.ScheduleEventInXDays(EventManager.GetEventById(1026), 1);
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa commits arson
	public static Outcome Event1026(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		manager.PlayerFamily.Grandpa.InsanityGrowth += 0.05;

		manager.PlayerFamily.Grandpa.Pride += 50;

		requirements.Grandpa.Pride -= 200;

		returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
		returnObj.OutcomeDescription = String.Format (
			"Grandpa just burned down {1}'s house! He was mumbling something about a league when I found him! I don't " +
			"think the police know.\n\n" +
			"Grandpa's insanity growth up.\n" +
			"Grandpa's pride up slightly.\n" +
			"{1} pride way down!\n",
			manager.PlayerFamily.Grandpa.Name, requirements.Grandpa.Name);

		return returnObj;
	}

	// Grandpa kills a cat
	public static Outcome Event1027(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 30)
		{
			manager.PlayerFamily.Grandpa.InsanityGrowth += 0.05;

			manager.PlayerFamily.Grandpa.Pride += 50;

			requirements.Grandpa.Pride -= 200;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Looks like Grandpa strangled {1}'s cat. I found them fighting in our yard this morning. It was not a pretty " +
				"sight, let me tell you. \n\n" +
				"Grandpa's insanity growth up.\n" +
				"Grandpa's pride up slightly.\n" +
				"{1} pride way down!\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Grandpa.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa is naked
	public static Outcome Event1028(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 60)
		{
			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.Wisdom -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1} found Grandpa wandering main street at the crack of dawn, naked as the day he was born. He was staring into " +
				"windows looking for what he called \"Battle Gear\". I have no idea what that means.\n\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's pride down.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Parent.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Grandpa gets a doctor
	public static Outcome Event1029(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (requirements.Accept)
		{
			manager.PlayerFamily.Grandpa.Insanity -= 10;
			manager.PlayerFamily.Grandpa.InsanityGrowth -= 0.1;
			manager.PlayerFamily.Grandpa.Wisdom += 10;
			manager.PlayerFamily.Grandpa.WisdomGrowth += 0.1;

			manager.PlayerFamily.Grandpa.MoneyGrowth -= 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa feels much better already! Those pesky voices are just fading into the background! There is no League. " +
				"There is no League. There is no League.\n\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's insanity growth down.\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's wisdom growth up.\n" +
				"Grandpa's income down.\n");
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;

		return returnObj;
	}

	// Grandpa joins a cult
	public static Outcome Event1031(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (manager.PlayerFamily.Grandpa.Insanity > 40)
		{
			manager.PlayerFamily.Grandpa.Insanity += 10;
			manager.PlayerFamily.Grandpa.Wisdom -= 10;

			manager.PlayerFamily.Grandpa.MoneyGrowth -= 10;

			manager.PlayerFamily.Grandpa.Pride -= 50;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa just brought over a bunch of pamphlets for The Church of the Tin Can. Seems like he's joined a cult. " +
				"He's paying them 10 dollars a month into their Collection Can.\n\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's income down slightly.\n" +
				"Grandpa's pride down.\n",
				manager.PlayerFamily.Grandpa.Name, requirements.Parent.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}
}
