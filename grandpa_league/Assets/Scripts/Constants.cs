using System;

static class Constants
{
    public const int NUM_FAMILIES = 2;
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
}
