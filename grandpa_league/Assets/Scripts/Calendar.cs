using System.Collections.Generic;

public class Calendar
{
    private int m_currentMonth = 1;
    private int m_currentDay = 1;
    private int m_currentYear = 2016;
    private List<Day> m_days = new List<Day>();

    public Calendar()
    {
        this.GenerateCalendarForYear();
    }

    public void AdvanceDay()
    {
        this.m_currentDay++;
        if (this.m_currentDay == 29)
        {
            this.m_currentDay = 1;
            this.m_currentMonth++;
            if (this.m_currentMonth == 13)
            {
                this.m_currentMonth = 1;
                this.m_currentYear++;
                this.GenerateCalendarForYear();
            }
        }
    }

    public List<SimulationEvent> GetEventsForCurrentDay()
    {
        return this.m_days[this.m_currentDay * this.m_currentMonth].GetEvents();
    }

    private void GenerateCalendarForYear()
    {
        for (var i = 1; i <= 12; i++)
        {
            for (var j = 1; j <= 4; j++)
                for (var k = 1; k <= 7; k++)
                    this.m_days.Add(new Day(Constants.DAY_NAMES[k], i, k * j, this.m_currentYear));

            foreach (SimulationEvent seasonalEvent in EventManager.GetEventsByMonth(i))
            {
                this.m_days[Constants.RANDOM.Next(1, 27) * i].AddEvent(seasonalEvent);
            }
        }
    }

    public Dictionary<string, int> GetCurrentDay()
    {
        Dictionary<string, int> currentDay = new Dictionary<string, int>();
        currentDay.Add("month", this.m_currentMonth);
        currentDay.Add("day", this.m_currentDay);
        currentDay.Add("year", this.m_currentYear);
        return currentDay;
    }

    public void ScheduleEventByDate(SimulationEvent simEvent, int day, int month)
    {
       this.m_days[28 * (month - 1) + (day - 1)].AddEvent(simEvent);
    }
}

public class Day
{
    private int m_year = 0;
    private int m_month = 0;
    private int m_day = 0;
    private string m_dayName = "";
    private List<SimulationEvent> m_events = null;

    public Day(string dayName, int month, int day, int year)
    {
        this.m_year = year;
        this.m_day = day;
        this.m_month = month;
        this.m_dayName = dayName;

		this.m_events = new List<SimulationEvent> ();

        SimulationEvent randomEvent = EventManager.GetRandomEvent();
        if(Constants.RANDOM.Next(0, 100) <= randomEvent.Chance * 100)
            this.m_events.Add(randomEvent);

        foreach (SimulationEvent knownEvent in EventManager.GetEventsByDate(this.m_month, this.m_day, this.m_year))
        {
            this.m_events.Add(knownEvent);
        }

        if(this.m_dayName == "Sunday")
        {
            SimulationEvent weeklyLevelUp = new SimulationEvent(null, 1, "level_up", "", 0, 0, 0);
            this.m_events.Add(weeklyLevelUp);
        }
    }

    public void AddEvent(SimulationEvent eventToAdd)
    {
        this.m_events.Add(eventToAdd);
    }

    public List<SimulationEvent> GetEvents()
    {
        return this.m_events;
    }
}