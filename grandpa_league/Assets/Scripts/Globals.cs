
using UnityEngine;

public class Globals
{
    public static bool UserInputting = false;
 
    private static int NUM_TESTS = 1000;
    private static int CUTENESS = 0;
    private static int STAT = 100;

    public static void RunUnitTests()
    {
        for (int j = 0; j < 3; j++)
        {
            var success = 0;
            for (int i = 0; i < NUM_TESTS; i++)
                if (Constants.Roll(CUTENESS, STAT, (int)Enums.Difficulty.VERY_EASY))
                    success++;
            Debug.Log(string.Format("Rolled {0} times at {1} cuteness, {2} stat, {3} difficulty got {4} successes", NUM_TESTS, CUTENESS, STAT, "VERY EASY", success));

            success = 0;
            for (int i = 0; i < NUM_TESTS; i++)
                if (Constants.Roll(CUTENESS, STAT, (int)Enums.Difficulty.EASY))
                    success++;
            Debug.Log(string.Format("Rolled {0} times at {1} cuteness, {2} stat, {3} difficulty got {4} successes", NUM_TESTS, CUTENESS, STAT, "EASY", success));

            success = 0;
            for (int i = 0; i < NUM_TESTS; i++)
                if (Constants.Roll(CUTENESS, STAT, (int)Enums.Difficulty.STANDARD))
                    success++;
            Debug.Log(string.Format("Rolled {0} times at {1} cuteness, {2} stat, {3} difficulty got {4} successes", NUM_TESTS, CUTENESS, STAT, "STANDARD", success));

            success = 0;
            for (int i = 0; i < NUM_TESTS; i++)
                if (Constants.Roll(CUTENESS, STAT, (int)Enums.Difficulty.HARD))
                    success++;
            Debug.Log(string.Format("Rolled {0} times at {1} cuteness, {2} stat, {3} difficulty got {4} successes", NUM_TESTS, CUTENESS, STAT, "HARD", success));

            success = 0;
            for (int i = 0; i < NUM_TESTS; i++)
                if (Constants.Roll(CUTENESS, STAT, (int)Enums.Difficulty.VERY_HARD))
                    success++;
            Debug.Log(string.Format("Rolled {0} times at {1} cuteness, {2} stat, {3} difficulty got {4} successes", NUM_TESTS, CUTENESS, STAT, "VERY_HARD", success));

            STAT -= 50;
        }
    }

    public static void VerifyStat(double value)
    {
        if(value > 100)
        {
            Debug.LogWarning(string.Format("WARNING: Just tried to set a stat to: {0}", value));
        }
    }

    public static void VerifyGrowth(double value)
    {
        if (value > 1)
        {
            Debug.LogWarning(string.Format("WARNING: Just tried to set a STAT GROWTH to: {0}", value));
        }
    }
}

