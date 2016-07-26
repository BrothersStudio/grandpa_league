public class Stat
{
    private double m_value = 0;
    private double m_minValue = 0;
    private double m_maxValue = 100;

    private double m_growthRate = 0.0;

    private bool m_growthBonus = false;
    private double m_growthBonusAmount = 0.0;

    private bool m_valueBonus = false;
    private double m_valueBonusAmount = 0.0;

    public Stat()
    {
    }

    public double Value
    {
        get { return this.m_valueBonus ? this.m_value + this.m_valueBonusAmount : this.m_value; }
        set { this.m_value = value; }
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
        get { return this.m_growthBonus ? this.m_growthRate + this.m_growthBonusAmount : this.m_growthRate; }
        set { this.m_growthRate = value; }
    }

    public bool GrowthBonus
    {
        get { return this.m_growthBonus; }
        set { m_growthBonus = value; }
    }

    public double GrowthBonusAmount
    {
        get { return this.m_growthBonusAmount; }
        set {this.m_growthBonusAmount = value; }
    }

    public bool ValueBonus
    {
        get { return this.m_growthBonus; }
        set { m_growthBonus = value; }
    }

    public double ValueBonusAmount
    {
        get { return this.m_valueBonusAmount; }
        set { this.m_valueBonusAmount = value; }
    }
}

