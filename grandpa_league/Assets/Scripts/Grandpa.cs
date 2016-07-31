using System;
using System.Collections.Generic;

[Serializable]
public class Grandpa : Character
{
    private Stat            m_insanity          = new Stat();
    private Stat            m_wisdom            = new Stat();
    private double             m_money             = 0;
    private double             m_income            = 0;
    private Stat            m_pride             = new Stat();

    private Stat            m_cumulativePride   = new Stat();
        
	public Grandpa(string name, int age, int pride, int wisdom, int insanity, int money, string spriteName)
	{
        this.m_name = name;
        this.m_age = age;
        this.m_pride.MaxValue = Int32.MaxValue;
		this.m_pride.Value = pride;
        this.m_wisdom.Value = wisdom;
        this.m_insanity.Value = insanity;
		this.m_money = money;

        this.m_insanity.GrowthRate = (insanity / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (insanity / Constants.Character.GROWTH_DIVIDER);
        this.m_wisdom.GrowthRate = -1 * (wisdom / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (wisdom / Constants.Character.GROWTH_DIVIDER);
        this.m_income = Constants.Character.INITIAL_MONEY_GROWTH;

        this.m_spriteName = spriteName;
    }

    public override void UpgradeRandomStat(double amt)
    {
        base.UpgradeRandomStat(amt);

        int random = Constants.RANDOM.Next(0, 2);
        switch (random)
        {
            case 0:
                this.m_wisdom.Value += amt;
                break;
            case 1:
                this.m_money += amt * 20;
                break;
            case 2:
                this.m_insanity.Value += amt;
                break;
        }
    }

    public override void SetDoubleStatMultiplier(bool multiply)
    {
        if (multiply)
        {
            this.m_insanity.ValueMultiplierActive = true;
            this.m_insanity.ValueMultiplier = 2;

            this.m_wisdom.ValueMultiplierActive = true;
            this.m_wisdom.ValueMultiplier = 2;
        }
        else
        {
            this.m_insanity.ValueMultiplierActive = false;
            this.m_insanity.ValueMultiplier = 0;

            this.m_wisdom.ValueMultiplierActive = false;
            this.m_wisdom.ValueMultiplier = 0;
        }
    }

    public override void UpgradeRandomStatGrowth(double amt)
    {
        base.UpgradeRandomStatGrowth(amt);

        int random = Constants.RANDOM.Next(0, 2);
        switch (random)
        {
            case 0:
                this.m_insanity.GrowthRate += amt;
                break;
            case 1:
                this.m_wisdom.GrowthRate += amt;
                break;
            case 2:
                this.m_income += amt * 100;
                break;
        }
    }

    public Grandpa(string name)
    {
        this.m_name = name;
    }

    public double Wisdom                                            //DEPRECATED
    {
        get { return this.m_wisdom.Value; }
        set { Globals.VerifyStat(value); this.m_wisdom.Value = value > 100 ? 100 : value; }
    }

    public double WisdomGrowth
    {
        get { return this.m_wisdom.GrowthRate; }
        set { Globals.VerifyGrowth(value); this.m_wisdom.GrowthRate = value; }
    }

    public double Insanity                                            //DEPRECATED
    {
        get { return this.m_insanity.Value; }
        set { Globals.VerifyStat(value); this.m_insanity.Value = value > 100 ? 100 : value; }
    }

    public double InsanityGrowth
    {
        get { return this.m_insanity.GrowthRate; }
        set { Globals.VerifyGrowth(value); this.m_insanity.GrowthRate = value; }
    }

    public double Money                                         
    {
        get { return this.m_money; }
        set { this.m_money = value < 0 ? 0 : value; }
    }

	public double MoneyGrowth
	{
		get { return this.m_income; }
		set { this.m_income = value; }
	}

    public double Pride                                            //DEPRECATED
    {
        get { return this.m_pride.Value; }
        set { this.m_pride.Value = value; }
    }

    public Stat CumulativePride
    {
        get { return this.m_cumulativePride; }
        set { this.m_cumulativePride = value; }
    }
}