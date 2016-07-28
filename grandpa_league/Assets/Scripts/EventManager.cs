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
    private static List<Ability> m_abilities = new List<Ability>();

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
                case (int)Enums.EventType.ABILITY:
                    m_abilities.Add(new Ability(simEvent.Attribute("ability_name").Value,
                                                simEvent.Attribute("ability_description").Value,
                                                Int32.Parse(simEvent.Attribute("cooldown").Value),
                                                simEvent.Attribute("picture").Value,
                                                new SimulationEvent(eventRequirements, 0,
                                                                    simEvent.Attribute("name").Value,
                                                                    simEvent.Attribute("description").Value,
                                                                    Int32.Parse(simEvent.Attribute("id").Value), (int)Enums.EventType.ABILITY, 2)
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
        statOutcome.Mail.Message = string.Format("Hey Dad,\n\n\tHere is your social security check for the month plus a little something extra I scraped up for you. The kids are doing just fine and are growing so fast! You'll barely even recognize them soon (between you and me, I'm worred {0} is already getting uglier like Mom did. Anyway have a good month!\nTotal Amount Applied to Account: ${1}.00\n\nLove,\n{2}",manager.PlayerFamily.Children[0].Name, manager.PlayerFamily.Grandpa.MoneyGrowth, manager.PlayerFamily.Parents[0].Name);
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

				requirements.Parent.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT; 
				manager.PlayerFamily.Grandpa.Pride += Constants.Character.MAJOR_PRIDE_CHANGE_AMOUNT;

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

				requirements.Parent.Love += Constants.Character.MINOR_STAT_CHANGE_AMOUNT; 
				manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That's not going to buy a nice car, Grandpa. More like a total clunker. Maybe back in your day! I'll take it anyway!\n\n" +
					"{0}'s love up.\n" +
					"Grandpa's pride way up.",
					requirements.Parent.Name);
			}
			else
			{
				manager.PlayerFamily.Grandpa.Insanity += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
				manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;;

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
			if (requirements.Money >= 2000) 
			{
				foreach (Parent parent in manager.PlayerFamily.Parents) 
				{
					parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
					parent.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
				}
				foreach (Child child in manager.PlayerFamily.Children) 
				{
					child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
					child.Athleticism += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
				}
				manager.PlayerFamily.Grandpa.Money -= requirements.Money;
				manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

				returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That was the best vacation I've ever been on! We feel much closer as a family and the kids ran around so much " +
					"that they're looking like buff demigods! Thanks, Grandpa!\n\n" +
					"{0}'s popularity way up!\n" +
					"All parents' love up!\n" +
					"All parents' popularity up!\n" +
					"All children's popularity up!\n" +
					"All children's athleticism up slightly." +
					"Grandpa's pride way up.");
			} 
			else
			{
				foreach (Parent parent in manager.PlayerFamily.Parents) 
				{
					parent.Love -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
				}
				manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

				returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
				returnObj.OutcomeDescription = String.Format (
					"That's not nearly enough money for the first class Carribean vacation we had in mind...\n\n" +
					"All parents' love down slightly.\n" +
					"Grandpa's pride down slightly.");
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
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.EASY))
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.Athleticism += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
			requirements.Child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			requirements.Child.Artistry -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Great ball tossing, {0}! Now everyone will value you more as a person! You'll probably be as buff as Dwayne \"The Rock\" Johnson " +
				"by the end of the season!\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" + 
				"{0}'s popularity up.\n" +
				"{0}'s popularity growth up.\n" + 
				"{0}'s artistry down slightly.\n" + 
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else
		{
			requirements.Child.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well, you didn't get on the team. That's disappointing, {0}. Not to mention you looked really dumb out there. Some of the kids are calling you " +
				"Not-So-Sticky-Fingers behind your back!\n\n" +
				"{0}'s popularity down slightly.\n" +
				"{0}'s popularity growth down slightly.\n" + 
				"Grandpa's pride down slightly.",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid is dating head cheerleader
	public static Outcome Event1008(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.HARD) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.STANDARD)) 
		{
			requirements.Child.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth += Constants.Character.MAJOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Wow, {0} is so popular and talented at indoor football that they are dating the head cheerleader! That's a couple that will be sure to turn " +
				"some heads!\n\n" +
				"{0}'s popularity way up!\n" + 
				"{0}'s popularity growth way up!\n" + 
				"Grandpa's pride way up.",
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
		if (!Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.VERY_HARD))
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.Athleticism -= Constants.Character.MAJOR_STAT_CHANGE_AMOUNT * 2;
			requirements.Child.AthleticismGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Oh God, seeing your leg fly right out of the socket during that indoor football game was brutal! I guess you're off the indoor football team for the year.\n\n" +
				"{0}'s popularity up.\n" +
				"{0}'s athleticism way way down!\n" +
				"{0}'s athleticism growth down.\n" +
				"Grandpa's pride down slightly.",
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
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.HARD))
		{
			requirements.Child.Athleticism += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Holy cow, {0} is just kicking ass at indoor football practice. It's clear to everyone that they're a very skilled player!\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"Grandpa's pride up.",
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
			requirements.Child.Athleticism += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			requirements.Child.AddQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Money -= 500;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Those Cambodians never disappoint! {0} is untouchable at practice now! {1} can slip through even the tightest of defenses. " +
				"I just hope no one finds out our dirty little secret...\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up.\n" +
				"Grandpa's pride up.",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Gender) ? "She" : "He");
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
				"\"Nice doing business with you, gramps!\"\nDamn those Cambodians... \nDamn them straight to hell...");
		}
		else  
		{
			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;

			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ILLEGAL_GEAR"));
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("ON_FOOTBALL_TEAM"));

			requirements.Child.AthleticismGrowth -= Constants.Character.MAJOR_STAT_GROWTH_AMOUNT;
			requirements.Child.Athleticism -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.Popularity -= Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

			returnObj.OutcomeDescription = String.Format (
				"You're gonna regret that pal! \n\"{0}, you're off the team!\"\nOh! My pride! It physically hurts!\n\n" +
				"{0}'s athleticism down.\n" +
				"{0}'s athleticism growth down.\n" +
				"{0}'s popularity way down!\n" +
				"Grandpa's pride way down!",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid's football championship
	public static Outcome Event1013(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.VERY_HARD)) 
		{
			requirements.Child.Athleticism += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth += Constants.Character.MAJOR_STAT_GROWTH_AMOUNT;

			requirements.Child.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MAJOR_PRIDE_CHANGE_AMOUNT;

			requirements.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;
			requirements.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"It's the most intense game of indoor football ever displayed! {0} scores every goal personally. " +
				"The entire indoor stadium are on their feet except {1}. During the last goal, {1}'s son is taken off the field on a stretcher!" +
				"But {0}'s team wins in the end. Of course.\n\n" +
				"{0}'s athleticism way up!\n" +
				"{0}'s athleticism growth way up!\n" +
				"{0}'s popularity way up!\n" +
				"{0}'s popularity growth up.\n" +
				"{1}'s pride way down!\n" +
				"Grandpa's pride way way up!!\n",
				requirements.Child.Name, requirements.Grandpa.Name);
		} 
		else if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.STANDARD)) 
		{
			requirements.Child.Athleticism += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1}'s son's team takes an early lead. {0} puts on a respectable display of resistance. They even put their elbow through" +
				"Some poor kid's face! That's one for the highlight reel! Ultimately, {0} loses... But Grandpa is still proud! Maybe next year.\n\n" +
				"{0}'s athleticism up.\n" +
				"{0}'s athleticism growth up slightly.\n" +
				"Grandpa's pride way up!\n",
				requirements.Child.Name, requirements.Grandpa.Name);
		} 
		else 
		{
			requirements.Child.Athleticism -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.AthleticismGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			requirements.Child.Popularity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Uhh, are you sure {0} was practicing all those times they said they were heading to practice? You really couldn't tell by their display " +
				"on the field. Utterly embarassing. Grandpa had to slink out the back at the end of the first quarter. {1} will never let him forget it.\n\n" +
				"{0}'s athleticism down.\n" +
				"{0}'s athleticism growth down.\n" +
				"{0}'s popularity down.\n" +
				"Grandpa's pride down.",
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
		if (!Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.EASY))
		{
			requirements.Child.AthleticismGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT;
			requirements.Child.Popularity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Wow, there's no way to put this lightly so I'm just going to come out and say it. {0} is just completely awful at football. " +
				"I'm not sure what happened. They got on the team okay. Now all the other kids laugh and throw balls at {1}.\n\n" +
				"{0}'s athleticism growth down.\n" +
				"{0}'s popularity down.\n" +
				"Grandpa's pride down.",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Wisdom, (int)Enums.Difficulty.STANDARD))
		{
			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Insanity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			int income_gain = Constants.RANDOM.Next (75, 150);
			manager.PlayerFamily.Grandpa.MoneyGrowth += income_gain;
			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Going through your social security payments, you find a nice loophole. That'll increase your weekly income! Thanks Obama!\n\n" +
				"Grandpa's wisdom up slightly.\n" + 
				"Grandpa's insanity down slightly.\n" + 
				"Grandpa's income up by {0} per week!\n" +
				"Grandpa's pride up.",
				income_gain);
		}
		else  
			returnObj.Status = (int)Enums.EventOutcome.PASS;
		
		return returnObj;
	}

	// Grandkid makes honor roll
	public static Outcome Event1005(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Intelligence, (int)Enums.Difficulty.STANDARD)) 
		{
			requirements.Child.Intelligence += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			}

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} has made the honor roll! Wow, you're like a freakin' genius {0}!\n\n" +
				"{0}'s intelligence up.\n" + 
				"All parents' love up.\n" + 
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else  
		{
			requirements.Child.Intelligence -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{0} missed the honor roll... What a dummy...\n\n" +
				"{0}'s intelligence down slightly.\n" + 
				"Grandpa's pride down.",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid tries out for play
	public static Outcome Event1006(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Artistry, (int)Enums.Difficulty.HARD) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.EASY)) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.ArtistryGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			requirements.Child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			}

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 3;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{0} got the lead role in the play! As well as several other roles! {1} performance left everyone " +
				"in tears and it instantly began raining outside. They say drama will never be the same.\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth up.\n" + 
				"{0}'s popularity up.\n" + 
				"{0}'s popularity growth up slightly.\n" + 
				"All parents' love up.\n" + 
				"Grandpa's pride way up!",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Name) ? "Her" : "His");
		}
		else if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Artistry, (int)Enums.Difficulty.EASY) || 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.HARD)) 
		{
			requirements.Child.AddQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.ArtistryGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{0} only messed up a handful of lines and fell off the stage only once or twice. Not too shabby! " +
				"{0} got a minor role in the play!\n\n" +
				"{0}'s artistry up.\n" + 
				"{0}'s artistry growth up.\n" + 
				"Grandpa's pride up.",
				requirements.Child.Name);
		}
		else 
		{
			requirements.Child.Artistry += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			requirements.Child.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Oof, wow, that was really hard to watch. Running off the stage in tears will probably hurt your reputation" +
				" a bit.\n\n" +
				"{0}'s artistry slightly up.\n" +
				"{0}'s popularity slightly down.\n" + 
				"Grandpa's pride slightly down.",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandkid forgets lines at practice
	public static Outcome Event1015(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (!Constants.Roll(requirements.Child.Cuteness, requirements.Child.Artistry, (int)Enums.Difficulty.STANDARD)) 
		{
			requirements.Child.Artistry -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			requirements.Child.ArtistryGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} shows up to play practice late practically every time and never seems to know {1} lines. " +
				"People are starting to talk...\n\n" +
				"{0}'s artistry slightly down.\n" + 
				"{0}'s artistry growth slightly down.\n" + 
				"Grandpa's pride slightly down.",
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
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Artistry, (int)Enums.Difficulty.HARD)) 
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Artistry += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.ArtistryGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			requirements.Child.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MAJOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"The play is a smash hit! The standing ovation has been going on for over 20 minutes already. Remarkable! People are shouting {0}'s " +
				"name at the top of their lungs. The lemur foundation can't count all the money they're making. The dream of having lemurs be as common " +
				"as rats is on the way to reality.\n\n" +
				"{0}'s artistry way up!\n" + 
				"{0}'s artistry growth up.\n" +
				"{0}'s popularity way up!\n" + 
				"Grandpa's pride way way up!!",
				requirements.Child.Name);
		}
		else 
		{
			requirements.Child.RemoveQualification (Qualification.GetQualificationByString ("IN_PLAY"));

			requirements.Child.Popularity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well that could have gone better. People are killing lemurs in the streets. They are calling this the worst play of all time. {0} will never " +
				"live this down at school for being a part of it.\n\n" +
				"{0}'s popularity down!\n" + 
				"{0}'s popularity growth down slightly.\n" + 
				"Grandpa's pride down.",
				requirements.Child.Name);
		}
		return returnObj;
	}

	// Grandpa finds buried gold
	public static Outcome Event1007(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Wisdom, (int)Enums.Difficulty.EASY) && 
			!Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.HARD))
		{
			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Insanity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			int found_amount = 1000 + Constants.RANDOM.Next (1000, 2000);
			manager.PlayerFamily.Grandpa.Money += found_amount;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa was looking through his old treasure maps and remembered ${0} worth of confederate gold he buried many years ago! How lucky!\n\n" +
				"Grandpa's wisdom up slightly.\n" + 
				"Grandpa's insanity down slightly.\n" + 
				"Grandpa gained {0} dollars!",
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
		if (!Constants.Roll(0, requirements.Parent.Love, (int)Enums.Difficulty.VERY_EASY) && 
			manager.PlayerFamily.Parents.Count > 1) 
		{
			manager.PlayerFamily.Parents.Remove (requirements.Parent);

			string other_parent = "";
			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				Debug.Log ("Remaining parent: " + parent.Name);
				other_parent = parent.Name;
				parent.Love -= Constants.Character.MAJOR_STAT_CHANGE_AMOUNT * 2;
				parent.AddQualification (Qualification.GetQualificationByString ("IS_SINGLE"));
			}

			manager.PlayerFamily.Grandpa.Pride -=  Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} has had it! What has this family done for {1} anyway?! {0} could have been an astronaut " +
				"or a plumber or a runway model but no... {3} gave it all up... For what?! {0} wants a divorce!! " +
				"{2} and the kids are inconsolable.\n\n" +
				"{0} has left the family!\n" + 
				"{2}'s love way way down!!\n" + 
				"Grandpa's pride way down!",
				requirements.Parent.Name, Convert.ToBoolean(requirements.Parent.Gender) ? "her" : "him", other_parent, 
				Convert.ToBoolean(requirements.Parent.Gender) ? "She" : "He");
		}
		else if (manager.PlayerFamily.Parents.Count > 1)
		{
			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			}

			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"It's obvious to everyone around them how in love {0} and {1} are! Grandpa feels proud for setting them " +
				"up all those years ago despite their protestations!\n\n" +
				"{0}'s love up.\n" + 
				"{1}'s love up.\n" + 
				"Grandpa's wisdom up.\n" + 
				"Grandpa's pride up.",
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
				parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
				parent.LoveGrowth -= Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
				parent.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
				parent.AddQualification (Qualification.GetQualificationByString ("IS_SINGLE"));
			}

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} was driving along the precarious cliffs on the edge of town to get the family a pizza and a really " +
				"unfortunately timed gust of wind threw {1} car straight into a bottomless pit. Yo, that, like, totally sucks. " +
				"Sorry.\n\n" +
				"{0} has left the family!\n" +
				"{2}'s love up.\n" +
				"{2}'s love growth down.\n" +
				"{2}'s popularity way up!\n" +
				"Grandpa's pride down!",
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
		if ((Constants.Roll(requirements.Child.Cuteness, requirements.Parent.Love, (int)Enums.Difficulty.EASY) && 
			(Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.HARD) && 
				requirements.Accept)) 
			||
			(Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.EASY) && 
				requirements.Money >= 200 && requirements.Accept)) 
		{
			requirements.Child.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth += Constants.Character.MAJOR_STAT_GROWTH_AMOUNT;

			requirements.Parent.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Parent.PopularityGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MAJOR_PRIDE_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
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
				"Grandpa's insanity up.",
				manager.PlayerFamily.Grandpa.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "mom" : "dad", requirements.Child.Name, requirements.Parent.Name);
		} 
		else if (!requirements.Accept) 
		{
			requirements.Child.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Lame! Don't you ever have any fun? You're wrong, it would have been a totally rad party... \nSniff... \n\n" +
				"{0}'s popularity slightly down.\n",
				requirements.Child.Name);
		} 
		else 
		{
			requirements.Child.Popularity -= Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;
			requirements.Child.PopularityGrowth -= Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
			manager.PlayerFamily.Grandpa.Money -= requirements.Money;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Well, that was the might awkward night of {0}'s life. No amount of money would have made that fun. Who let that kid throw a party? {1}'s totally a loser... And who brought " +
				"that live tiger? Was that you, Grandpa?\n\n" +
				"{0}'s popularity way down!\n" +
				"{0}'s popularity growth down.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up.\n" +
				"Grandpa's pride down.",
				requirements.Child.Name, Convert.ToBoolean (requirements.Child.Gender) ? "She" : "He");
		}

		return returnObj;
	}

	// Parent injured at construction site
	public static Outcome Event1020(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (!Constants.Roll(0, requirements.Parent.Intelligence, (int)Enums.Difficulty.EASY))
		{
			requirements.Parent.Intelligence -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Parent.Popularity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE;
			returnObj.OutcomeDescription = String.Format (
				"{0} was walking through a construction site the other day and got hit by a girder! Why was {1} even walking through there? " +
				"What a dummy. What a disgrace...\n\n" +
				"{0}'s intelligence down.\n" +
				"{0}'s popularity down!\n" +
				"Grandpa's pride down slightly.",
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
		if (Constants.Roll(requirements.Child.Cuteness, requirements.Child.Intelligence, (int)Enums.Difficulty.HARD) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Artistry, (int)Enums.Difficulty.EASY) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Athleticism, (int)Enums.Difficulty.EASY))
		{
			foreach (Child child in manager.PlayerFamily.Children) 
			{
				child.Intelligence += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
				child.IntelligenceGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
			}

			foreach (Parent parent in manager.PlayerFamily.Parents) 
			{
				parent.Intelligence += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
				parent.IntelligenceGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
			}

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Awesome! {0} just got into Harvard! {1} is going for Nautical Archaeology. It's been {2} passion for as long as you can remember. " +
				"You feel your entire family get smarter just by being in contact with {2}!\n\n" +
				"Entire family's intelligence up!!\n" +
				"Entire family's intelligence growth up!!\n" +
				"Grandpa's pride way up!!",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.STANDARD))
		{
			requirements.Child.Intelligence -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa brought home a doggy! We were all so happy until it started foaming at the mouth and bit {1} right in the face! " +
				"Turns out Grandpa found it by the side of the road... Is Grandpa going to be okay, {2}?\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up slightly.\n" +
				"Grandpa's pride down slightly.\n" +
				"{1}'s intelligence down.",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.STANDARD))
		{
			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT; 

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT; 

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1} just got off the phone with the coast guard. They finally found Grandpa. Turns out he wandered into a shipping container " +
				"and got halfway around the world before someone noticed! I'm getting worried about him...\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up slightly.\n" +
				"Grandpa's pride down slightly.",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Wisdom, (int)Enums.Difficulty.STANDARD) && roll == 0) 
		{
			// Adult
			requirements.Parent.Intelligence += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Parent.Love += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Parent.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Insanity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT; 

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{1} was having a really rough day so Grandpa sat {2} down and gave {2} a life advice talk. {1} was completely reinvigorated and has a " +
				"new lease on life! That's the power of Grandpa, bitch!\n\n" +
				"{1} ALL STATS UP!!\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's pride up.",
				manager.PlayerFamily.Grandpa.Name, requirements.Parent.Name, Convert.ToBoolean (requirements.Parent.Gender) ? "her" : "him");
		} 
		else if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Wisdom, (int)Enums.Difficulty.STANDARD) && roll == 1) 
		{
			// Child
			requirements.Child.Intelligence += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Child.Athleticism += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Child.Artistry += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			requirements.Child.Cuteness += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Insanity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT; 

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS;
			returnObj.OutcomeDescription = String.Format (
				"{1} was having a really rough day so Grandpa sat {2} down and gave {2} a life advice talk. {1} was completely reinvigorated and has a " +
				"new lease on life! That's the power of Grandpa, bitch!\n\n" +
				"{1} ALL STATS UP!!\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's pride up.",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.HARD))
		{
			double money_lost = manager.PlayerFamily.Grandpa.Money / 2;
			manager.PlayerFamily.Grandpa.Money -= money_lost;

			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			manager.PlayerFamily.Grandpa.WisdomGrowth -= Constants.Character.MINOR_STAT_GROWTH_AMOUNT; 

			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT; 
			manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT;  

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT; 

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa just lost ${1} on a ponzi scheme! He actually thought he could get rich quick selling plungers door to door. " +
				"Grandpa seems like he's losing more of his independance every day...\n\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's wisdom growth down slightly.\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's insanity growth up slightly.\n" +
				"Grandpa's pride down slightly.\n" +
				"Grandpa lost {1} dollars.",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.EASY))
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
		manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT; 

		manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT; 

		requirements.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2; 

		returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
		returnObj.OutcomeDescription = String.Format (
			"Grandpa just burned down {1}'s house! He was mumbling something about a league when I found him! I don't " +
			"think the police know.\n\n" +
			"Grandpa's insanity growth up.\n" +
			"Grandpa's pride up.\n" +
			"{1} pride way down!",
			manager.PlayerFamily.Grandpa.Name, requirements.Grandpa.Name);

		return returnObj;
	}

	// Grandpa kills a cat
	public static Outcome Event1027(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.EASY))
		{
			manager.PlayerFamily.Grandpa.InsanityGrowth += Constants.Character.MINOR_STAT_GROWTH_AMOUNT; 

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT; 

			requirements.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT * 2; 

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Looks like Grandpa strangled {1}'s cat. I found them fighting in our yard this morning. It was not a pretty " +
				"sight, let me tell you. \n\n" +
				"Grandpa's insanity growth up slightly.\n" +
				"Grandpa's pride up slightly.\n" +
				"{1} pride way down!",
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
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.STANDARD))
		{
			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"{1} found Grandpa wandering main street at the crack of dawn, naked as the day he was born. He was staring into " +
				"windows looking for what he called \"Battle Gear\". I have no idea what that means.\n\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's pride down slightly.",
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
			manager.PlayerFamily.Grandpa.Insanity -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.InsanityGrowth -= Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;
			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.WisdomGrowth += Constants.Character.STANDARD_STAT_GROWTH_AMOUNT;

			manager.PlayerFamily.Grandpa.MoneyGrowth -= 50;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa feels much better already! Those pesky voices are just fading into the background! There is no League. " +
				"There is no League. There is no League.\n\n" +
				"Grandpa's insanity down.\n" +
				"Grandpa's insanity growth down.\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's wisdom growth up.\n" +
				"Grandpa's monthly income down $50.");
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR;

		return returnObj;
	}

	// Grandpa joins a cult
	public static Outcome Event1031(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.STANDARD))
		{
			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.MoneyGrowth -= 10;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_FOREVER;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa just brought over a bunch of pamphlets for The Church of the Tin Can. He's trying to get {1} to wear this weird " +
				"hat. Seems like he's joined a cult. " +
				"He's paying them 10 dollars a month into their Collection Can.\n\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's monthly income down $10.",
				requirements.Child.Name);
		}
		else 
			returnObj.Status = (int)Enums.EventOutcome.PASS;

		return returnObj;
	}

	// Valentine's Day
	public static Outcome Event1032(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Wisdom, (int)Enums.Difficulty.EASY) || 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.STANDARD))
		{
			requirements.Child.Popularity += Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Wisdom += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa is one smooth operator. He whispers into {0}'s ear the secret word that only grandpas know which will make any {1}'s " +
				"knees weak. It's a critical hit! {0} has a date to the Valentine's Day dance!\n\n" +
				"{0}'s popularity way up!\n" +
				"Grandpa's wisdom up.\n" +
				"Grandpa's pride up.",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Gender) ? "fella" : "lady");
		}
		else if (Constants.Roll(0, manager.PlayerFamily.Grandpa.Insanity, (int)Enums.Difficulty.STANDARD))
		{
			requirements.Child.Popularity -= Constants.Character.MAJOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Wisdom -= Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa, you're nuts!! I never should have listened to you! I knew I shouldn't have put a hornet's nest in {1} locker! " +
				"Hornet's don't even make honey! How was that an acceptable sentiment?! Can you stop drooling and listen to me?\n\n" +
				"{0}'s popularity way down!\n" +
				"Grandpa's insanity up.\n" +
				"Grandpa's wisdom down.\n" +
				"Grandpa's pride down.",
				requirements.Child.Name, Convert.ToBoolean(requirements.Child.Gender) ? "his" : "her");
		}
		else
		{
			requirements.Child.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Insanity += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Oof, rejected. Thanks anyway, Grandpa. I thought that Columbian shrew mating dance you taught me was spot on. It could have gone worse, I guess.\n\n" +
				"{0}'s popularity down slightly.\n" +
				"Grandpa's insanity up slightly.\n" +
				"Grandpa's pride down slightly.",
				requirements.Child.Name);
		}

		return returnObj;
	}

	// Grandpa's Birthday
	public static Outcome Event1033(DataManager manager, Requirement requirements)
	{
		Outcome returnObj = new Outcome();
		if (Constants.Roll(0, requirements.Parent.Popularity, (int)Enums.Difficulty.STANDARD) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.STANDARD))
		{
			requirements.Parent.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;
			requirements.Child.Popularity += Constants.Character.STANDARD_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride += Constants.Character.MAJOR_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"Grandpa couldn't believe it when Mitt Romney jumped out of that cake! Everyone was there wishing Grandpa a happy birthday! " +
				"Even {2} was there kissing Grandpa's ring. Grandpa will treasure moments (and belongings) like these for the rest of his life.\n\n" +
				"{0}'s popularity up.\n" +
				"{1}'s popularity up.\n" +
				"Grandpa's pride way up.",
				requirements.Child.Name, requirements.Parent.Name, requirements.Grandpa.Name);
		}
		else if (Constants.Roll(0, requirements.Parent.Popularity, (int)Enums.Difficulty.VERY_EASY) && 
			Constants.Roll(requirements.Child.Cuteness, requirements.Child.Popularity, (int)Enums.Difficulty.VERY_EASY))
		{
			requirements.Parent.Popularity += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			requirements.Child.Popularity += Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.MINOR_PRIDE_CHANGE_AMOUNT;
			requirements.Grandpa.Pride += Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"The party was going pretty good until {2} released that swarm of hornets into the house. He shouted something about pride and sprinted away. " +
				"I didn't realize old guys could move that fast.\n\n" +
				"{0}'s popularity up slightly.\n" +
				"{1}'s popularity up slightly.\n" +
				"{2}'s pride up.\n" +
				"Grandpa's pride down slightly.",
				requirements.Child.Name, requirements.Parent.Name, requirements.Grandpa.Name);
		}
		else
		{
			requirements.Parent.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;
			requirements.Child.Popularity -= Constants.Character.MINOR_STAT_CHANGE_AMOUNT;

			manager.PlayerFamily.Grandpa.Pride -= Constants.Character.STANDARD_PRIDE_CHANGE_AMOUNT;

			returnObj.Status = (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR;
			returnObj.OutcomeDescription = String.Format (
				"That was... awkward. A lot of standing around. I hate when people don't dance. Who invited all those nerds? Grandpa's gone and locked himself " +
				"in his room. \n\n" +
				"{0}'s popularity down slightly.\n" +
				"{1}'s popularity down slightly.\n" +
				"Grandpa's pride down.",
				requirements.Child.Name, requirements.Parent.Name);
		}

		return returnObj;
	}
}
