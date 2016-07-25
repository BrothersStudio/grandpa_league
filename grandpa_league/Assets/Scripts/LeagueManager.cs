using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class LeagueManager
{
    public static void SimulateDay(DataManager manager)
    {
        SimulateStatChanges(manager);
        SimulateCharacterMovement(manager);
        SimulateLeagueStandings(manager);
        //ScheduleLeagueEvents(manager);
    }

    private static void SimulateStatChanges(DataManager manager)
    {
        foreach(Family family in manager.LeagueFamilies)
        {
            foreach(Character cur in family.GetAllCharacters())
            {
                cur.UpgradeRandomStat();
            }
        }
    }

    private static void SimulateCharacterMovement(DataManager manager)
    {

    }

    private static void SimulateLeagueStandings(DataManager manager)
    {

    }
}