static class Enums
{
    public enum Gender {MALE = 0, FEMALE = 1 }

    public enum Character {CHILD = 0, PARENT = 1, GRANDPA = 2 }

    public enum Month {JANUARY = 1, FEBRUARY = 2, MARCH = 3, APRIL = 4, MAY = 5, JUNE = 6, JULY = 7, AUGUST = 8, SEPTEMBER = 9, OCTOBER = 10, NOVEMBER = 11, DECEMBER = 12 }

    public enum Days { MONDAY = 1,  TUESDAY = 2, WEDNESDAY = 3, THURSDAY = 4, FRIDAY = 5, SATURDAY = 6, SUNDAY = 7 }

    public enum Priority { LOW = 0, MED = 1, HIGH = 2 }

    public enum EventType { HIDDEN = 0, KNOWN = 1, RESERVED = 2 }

    public enum EventOutcome { SUCCESS = 0, FAILURE = 1, PASS = 2, SUCCESS_BLACKLIST_YEAR = 3, FAILURE_BLACKLIST_YEAR = 4, SUCCESS_BLACKLIST_FOREVER = 5, FAILURE_BLACKLIST_FOREVER = 6, PASS_BLACKLIST_YEAR = 7, PASS_BLACKLIST_FOREVER = 8 }

    public enum SystemEvents { WEEKLY_STAT_UP = 0, TRADE_ACCEPT_REJECT = 1 }
}
