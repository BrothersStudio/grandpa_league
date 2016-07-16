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
        if(this.m_currentDay == 29)
        {
            this.m_currentDay = 1;
            this.m_currentMonth++;
            if(this.m_currentMonth == 13)
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
            for (var j = 1; j <= 4; j++)
                for (var k = 1; k <= 7; k++)
                    this.m_days.Add(new Day(Constants.DAY_NAMES[k], i, k*j, this.m_currentYear));
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
        int randomInt = Constants.RANDOM.Next(Constants.RANDOM_FACTOR - 1);
        if (randomInt == 1)
            this.m_events.Add(EventManager.GetRandomEvent());

        foreach (SimulationEvent knownEvent in EventManager.GetEventsByDate(this.m_month, this.m_day, this.m_year))
        {
            this.m_events.Add(knownEvent);
        }

        if(this.m_dayName == "Sunday")
        {
            SimulationEvent weekleyLevelUp = new SimulationEvent(null, "level_up", "", 0, 0, 0);
            this.m_events.Add(weekleyLevelUp);
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