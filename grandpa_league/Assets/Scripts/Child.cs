using System.Collections;
using System;

public class Child : Character
{
    private     int     m_cuteness              = 0;
    private     int     m_intelligence          = 0;
    private     int     m_fineMotorSkills       = 0;
    private     int     m_grossMotorSkills      = 0;
    private     int     m_disposition           = 0;

    private     double  m_cutenessGrowth        = 0;
    private     double  m_intelligenceGrowth    = 0;
    private     double  m_fineMotorGrowth       = 0;
    private     double  m_grossMotorGrowth      = 0;
    private     double  m_dispositionGrowth     = 0;

    public Child(string name, string gender, string age, string cuteness, string intelligence, string fine_motor, string gross_motor, string disposition)
    {
        this.m_name = name;
        this.m_gender = Int32.Parse(gender);
        this.m_age = Int32.Parse(age);
        this.m_cuteness = Int32.Parse(cuteness);
        this.m_intelligence = Int32.Parse(intelligence);
        this.m_fineMotorSkills = Int32.Parse(fine_motor);
        this.m_grossMotorSkills = Int32.Parse(gross_motor);
        this.m_disposition = Int32.Parse(disposition);

        this.m_cutenessGrowth = -1 * this.m_cuteness / 300;
        this.m_intelligenceGrowth = this.m_intelligence / 300;
        this.m_fineMotorGrowth = this.m_fineMotorSkills / 300;
        this.m_grossMotorGrowth = this.m_grossMotorSkills / 300;
        this.m_dispositionGrowth = -1 * this.m_disposition / 300;
}
    
    public int Cuteness
    {
        get { return this.m_cuteness;  }
        set { this.m_cuteness = value; }
    }

    public double CutenessGrowth
    {
        get { return this.m_cutenessGrowth; }
        set { this.m_cutenessGrowth = value; }
    }

    public int Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligenceGrowth; }
        set { this.m_intelligenceGrowth = value; }
    }

    public int FineMotorSkills
    {
        get { return this.m_fineMotorSkills; }
        set { this.m_fineMotorSkills = value; }
    }

    public double FineMotorGrowth
    {
        get { return this.m_fineMotorGrowth; }
        set { this.m_fineMotorGrowth = value; }
    }

    public int GrossMotorSkills
    {
        get { return this.m_grossMotorSkills; }
        set { this.m_grossMotorSkills = value; }
    }

    public double GrossMotorGrowth
    {
        get { return this.m_grossMotorGrowth; }
        set { this.m_grossMotorGrowth = value; }
    }

    public int Disposition
    {
        get { return this.m_disposition; }
        set { this.m_disposition = value; }
    }

    public double DispositionGrowth
    {
        get { return this.m_dispositionGrowth; }
        set { this.m_dispositionGrowth = value; }
    }
}