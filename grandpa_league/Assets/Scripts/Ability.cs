using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Ability
{
    private string m_abilityName = "";
    private string m_abilityDescription = "";
    private int m_abilityCooldown = 0;
    private int m_currentCooldown;
    private string m_abilityPicture = "";
    private double m_insanityCost = 0;
    private int m_moneyCost = 0;

    private SimulationEvent m_abilityEvent = null;

    public Ability(string name, string description, int cooldown, string picture, double insanityCost, int moneyCost, SimulationEvent abilityEvent)
    {
        this.m_abilityName = name;
        this.m_abilityDescription = description;
        this.m_abilityCooldown = cooldown;
        this.m_abilityEvent = abilityEvent;
        this.m_insanityCost = insanityCost;
        this.m_moneyCost = moneyCost;
    }

    public string Name
    {
        get { return this.m_abilityName; }
    }

    public string Description
    {
        get { return this.m_abilityDescription; }
    }

    public int MaxCooldown
    {
        get { return this.m_abilityCooldown; }
    }

    public int CurrentCooldown
    {
        get { return this.m_currentCooldown; }
        set { this.m_currentCooldown = value; }
    }

    public int MoneyCost
    {
        get { return this.m_moneyCost; }
    }

    public double InsanityCost
    {
        get { return this.m_insanityCost; }
    }

    public string Picture
    {
        get { return this.m_abilityPicture; }
    }

    public SimulationEvent Event
    {
        get { return this.m_abilityEvent; }
    }
}
