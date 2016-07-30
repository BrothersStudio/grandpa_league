using System;


static class Constants
{
    public const int NUM_FAMILIES = 6;
	public const int INITIAL_PARENTS = 2;
    public const int INITIAL_CHILDREN = 3;
    public const int RANDOM_FACTOR = 10;
	public static Random RANDOM = new Random();

    public static bool Roll(double cuteness, double inStat, int difficulty)
    {
        double cutenessIncrease = cuteness / 10;
        double successChance = 0;
        switch (difficulty)
        {
            case (int)Enums.Difficulty.VERY_HARD:
                successChance = ((9.6 * Math.Pow(10, -3)) * Math.Pow(inStat, 2)) - (0.46 * inStat) + (1.776 * Math.Pow(10, -15));
                break;
            case (int)Enums.Difficulty.HARD:
                successChance = (9.2 * Math.Pow(10, -3) * Math.Pow(inStat, 2)) - (0.18 * inStat) + 1;
                break;
            case (int)Enums.Difficulty.STANDARD:
                successChance = (0.8 * inStat) + 10;
                break;
            case (int)Enums.Difficulty.EASY:
                successChance = (-5 * Math.Pow(10, -3) * Math.Pow(inStat, 2)) + (1.25 * inStat) + 20;
                break;
            case (int)Enums.Difficulty.VERY_EASY:
                successChance = (-8.2 * Math.Pow(10, -3) * Math.Pow(inStat, 2)) + (1.51 * inStat) + 30;
                break;
        }

        successChance += cutenessIncrease;
        int randomInt = RANDOM.Next(0, 100);
        if (successChance < randomInt)
            return false;
        return true;
    }

    public static string[] DAY_NAMES = 
		{
            "Sunday",
            "Monday",
			"Tuesday",
			"Wednesday",
			"Thursday",
			"Friday",
			"Saturday"
		};
	public static string[] MONTH_NAMES = 
		{
			"",
			"January",
			"February",
			"March",
			"April",
			"May",
			"June",
			"July",
			"August",
			"September",
			"October",
			"November",
			"December",
		};

    public struct Family
    {
        public const double GRANDPA_UPKEEP = 150;
        public const double CHILD_UPKEEP = 200;
        public const double PARENT_UPKEEP = 100;

        public const double WIN_PRIDE_AMOUNT = 1000;
        public const double LOSS_PRIDE_AMOUNT = -500;
        public const double DRAW_PRIDE_AMOUNT = 0;
    }

    public struct Player
    {
        public const double INITIAL_MONEY = 100000;
        public const double INITIAL_INCOME = 1400;
        public const string DEFAULT_SURNAME = "Smith";
        public const double INITIAL_WISDOM = 70;
        public const double INITIAL_WISDOM_GROWTH = -0.01;
        public const double INITIAL_INSANITY = 10;
        public const double INITIAL_INSANITY_GROWTH = 0.01;

		public const string SPRITE_NAME = "Grandpa_Sprite_Scaled";
    }

    public struct Character
    {
        public const double GROWTH_DIVIDER = 300;
        public const double MAX_INITIAL_GROWTH = 0.05;
        public const double INITIAL_MONEY_GROWTH = 400;

        public const double MAJOR_STAT_CHANGE_AMOUNT = 5;
        public const double STANDARD_STAT_CHANGE_AMOUNT = 2.5;
        public const double MINOR_STAT_CHANGE_AMOUNT = 1;

        public const double MAJOR_STAT_GROWTH_AMOUNT = 0.025;
        public const double STANDARD_STAT_GROWTH_AMOUNT = 0.01;
        public const double MINOR_STAT_GROWTH_AMOUNT = 0.0005;

        public const double MAJOR_PRIDE_CHANGE_AMOUNT = 500;
        public const double STANDARD_PRIDE_CHANGE_AMOUNT = 100;
        public const double MINOR_PRIDE_CHANGE_AMOUNT = 50;

        public const double GROWTH_BONUS_AMOUNT = 0.02;
    }

    public struct League
    {
        public const double MAJOR_STAT_INCREASE_CHANCE = 0.005;
        public const double MINOR_STAT_INCREASE_CHANCE = 0.01;
        public const double TINY_STAT_INCREASE_CHANCE = 0.05;
        public const double MAJOR_STAT_DECREASE_CHANCE = 0.002;
        public const double MINOR_STAT_DECREASE_CHANCE = 0.008;
        public const double TINY_STAT_DECREASE_CHANCE = 0.03;

        public const double MAJOR_STAT_INCREASE_AMOUNT = 10;
        public const double MINOR_STAT_INCREASE_AMOUNT = 5;
        public const double TINY_STAT_INCREASE_AMOUNT = 1;
        public const double MAJOR_STAT_DECREASE_AMOUNT = 9;
        public const double MINOR_STAT_DECREASE_AMOUNT = 4;
        public const double TINY_STAT_DECREASE_AMOUNT = 2;

        public const double MAJOR_STAT_GROWTH_AMOUNT = 0.05;
        public const double MINOR_STAT_GROWTH_AMOUNT = 0.02;
        public const double TINY_STAT_GROWTH_AMOUNT = 0.01;

        public const double CHARACTER_TRADE_CHANCE = 0.02;
        public const double CHARACTER_ADD_CHANCE = 0.005;
        public const double CHARACTER_REMOVE_CHANCE = 0.005;

        public const double PRIDE_INCREASE_CHANCE = 0.09;
        public const double PRIDE_DECREASE_CHANCE = 0.10;

        public const double STANDARD_PRIDE_INCREASE_AMOUNT = 100;
        public const double MAJOR_PRIDE_INCREASE_AMOUNT = 500;
        public const double MINOR_PRIDE_INCREASE_AMOUNT = 50;
    }
}
