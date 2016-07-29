using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class LeagueManager
{
    public static void SimulateDay(DataManager manager)
    {
        //SimulateStatChanges(manager);
        SimulateCharacterMovement(manager);
        SimulateLeagueStandings(manager);
        //ScheduleLeagueEvents(manager);            //TODO
    }

    private static void SimulateStatChanges(DataManager manager)
    {
        foreach(Family family in manager.LeagueFamilies)
        {
            foreach(Character cur in family.GetAllCharacters())
            {
                int random = Constants.RANDOM.Next(0, 1);
                
                if (random == 1)
                {
                    random = Constants.RANDOM.Next(0, 1000);
                    if (random < Constants.League.MAJOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MAJOR_STAT_INCREASE_AMOUNT);            //make sure this is calling the right one
                    else if (random < Constants.League.MINOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MINOR_STAT_INCREASE_AMOUNT);
                    else if (random < Constants.League.TINY_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.TINY_STAT_INCREASE_AMOUNT);

                    random = Constants.RANDOM.Next(0, 1000);
                    if (random < Constants.League.MAJOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MAJOR_STAT_GROWTH_AMOUNT);
                    else if (random < Constants.League.MINOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MINOR_STAT_GROWTH_AMOUNT);
                    else if (random < Constants.League.TINY_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.TINY_STAT_GROWTH_AMOUNT);
                }
                else
                {
                    if (random < Constants.League.MAJOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MAJOR_STAT_INCREASE_AMOUNT);            
                    else if (random < Constants.League.MINOR_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MINOR_STAT_INCREASE_AMOUNT);
                    else if (random < Constants.League.TINY_STAT_INCREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.TINY_STAT_INCREASE_AMOUNT);

                    random = Constants.RANDOM.Next(0, 1000);
                    if (random < Constants.League.MAJOR_STAT_DECREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MAJOR_STAT_DECREASE_AMOUNT);
                    else if (random < Constants.League.MINOR_STAT_DECREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.MINOR_STAT_DECREASE_AMOUNT);
                    else if (random < Constants.League.TINY_STAT_DECREASE_CHANCE * 1000)
                        cur.UpgradeRandomStat(Constants.League.TINY_STAT_DECREASE_AMOUNT);
                }
            }
        }
    }

    private static void SimulateCharacterMovement(DataManager manager)
    {
        //TODO
    }

    private static void SimulateLeagueStandings(DataManager manager)
    {
        foreach(Family fam in manager.LeagueFamilies)
        {
            int random = Constants.RANDOM.Next(0, 2);                   //TODO maj, min, and also decrease
            if (random == 1)
                fam.Grandpa.Pride -= Constants.League.STANDARD_PRIDE_INCREASE_AMOUNT;
            else
                fam.Grandpa.Pride += Constants.League.STANDARD_PRIDE_INCREASE_AMOUNT;
        }
    }
}