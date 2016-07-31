using System;

[Serializable]
public class Stat
{
    private double m_value = 0;
    private double m_minValue = 0;
    private double m_maxValue = 100;

    private double m_growthRate = 0.0;

    private bool m_growthBonus = false;
    private double m_growthBonusAmount = Constants.Character.GROWTH_BONUS_AMOUNT;

    private bool m_valueModifier = false;
    private double m_valueMultiplier = 0.0;

    public Stat()
    {
    }

    public double Value
    {
        get { return this.m_value; }
        set { this.m_value = this.m_growthBonus ? value  * (this.m_growthBonusAmount + 1) : value ; }
    }

    public double MaxValue
    {
        get { return this.m_maxValue; }
        set { this.m_maxValue = value; }
    }

    public double MinValue
    {
        get { return this.m_minValue; }
        set { this.m_minValue = value; }
    }

    public double GrowthRate
    {
        get { return this.m_growthRate > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : this.m_growthRate; }
        set { this.m_growthRate = value ; }
    }

    public bool GrowthBonus
    {
        get { return this.m_growthBonus; }
        set { this.m_growthBonus = value; }
    }

    public double GrowthBonusAmount
    {
        get { return this.m_growthBonusAmount; }
        set {this.m_growthBonusAmount = value; }
    }

    public bool ValueMultiplierActive
    {
        get { return this.m_growthBonus; }
        set { m_growthBonus = value; }
    }

    public double ValueMultiplier
    {
        get { return m_valueMultiplier; }
        set { m_valueMultiplier = value; }
    }
}

