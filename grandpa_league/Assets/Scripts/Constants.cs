using System;

static class Constants
{
    public const int NUM_FAMILIES = 6;
	public const int INITIAL_PARENTS = 2;
    public const int INITIAL_CHILDREN = 3;
    public const int RANDOM_FACTOR = 10;
	public static Random RANDOM = new Random();

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
