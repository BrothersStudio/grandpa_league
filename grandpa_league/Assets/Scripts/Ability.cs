using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Ability
{
    private string m_abilityName = "";
    private string m_abilityDescription = "";
    private int m_abilityCooldown = 0;
    private string m_abilityPicture = "";

    private SimulationEvent m_abilityEvent = null;

    public Ability(string name, string description, int cooldown, string picture, SimulationEvent abilityEvent)
    {
        this.m_abilityName = name;
        this.m_abilityDescription = description;
        this.m_abilityCooldown = cooldown;
        this.m_abilityEvent = abilityEvent;
    }

    public string Name
    {
        get { return this.m_abilityName; }
    }

    public string Description
    {
        get { return this.m_abilityDescription; }
    }

    public int Cooldown
    {
        get { return this.m_abilityCooldown; }
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
