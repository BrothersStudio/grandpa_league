using System.Collections.Generic;
using System;

[Serializable]
public class Character
{
    protected string        m_name              = "";
    protected int           m_gender            = 0;
    protected int           m_age               = 0;
    protected List<int>     m_qualifications    = new List<int>();
    protected string        m_spriteName        = "default_sprite";

    public string SpriteName
    {
        get { return this.m_spriteName; }
        set { this.m_spriteName = value; }
    }

    public string Name
    {
        get { return this.m_name; }
        set { this.m_name = value; }
    }

    public int Gender
    {
        get { return this.m_gender; }
        set { this.m_gender = value; }
    }

	public int Age
	{
		get { return this.m_age; }
		set { this.m_age = value; }
	}

    public List<int> Qualifications
    {
        get { return this.m_qualifications; }
    }

    public bool MeetsAgeRequirement(int minAge, int maxAge)
    {
        if (this.m_age >= minAge && this.m_age <= maxAge)
            return true;
        return false;
    }

    public void AddQualification(int qualification)
    {
        if (!this.m_qualifications.Contains(qualification))
            this.m_qualifications.Add(qualification);
    }

    public void RemoveQualification(int qualification)
    {
        this.m_qualifications.Remove(qualification);
    }

    public virtual void UpgradeRandomStat(double amt) { }
    public virtual void UpgradeRandomStatGrowth(double amt) { }

}