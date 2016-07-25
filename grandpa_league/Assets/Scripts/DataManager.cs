using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataManager
{
    private PlayerInfo              m_currentInfo       = null;
    private Calendar                m_currentCalendar   = null;
    private List<Family>            m_league            = new List<Family>();
    private Family                  m_playerFamily      = null;
    private List<int>               m_blacklist         = new List<int>();

    public DataManager (string playerName)
    {
        this.m_currentInfo = new PlayerInfo(playerName);
        this.m_currentCalendar = new Calendar();

        this.m_playerFamily = new Family();
        this.m_playerFamily.Grandpa = new Grandpa(playerName);
        this.PlayerFamily.Grandpa.Money = 1200;
        this.PlayerFamily.Grandpa.MoneyGrowth = 200;
        this.m_playerFamily.Parents = CharacterManager.GetRandomParents(Constants.INITIAL_PARENTS);
        this.m_playerFamily.Children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);

        for (int i = 1; i < Constants.NUM_FAMILIES; i++)
            this.m_league.Add(new Family(true));
    }

    public Family PlayerFamily
    {
        get { return this.m_playerFamily; }
    }

    public List<Family> LeagueFamilies
    {
        get { return this.m_league; }
    }

    public Calendar Calendar
    {
        get { return this.m_currentCalendar; }
    }

    public PlayerInfo PlayerInfo
    {
        get { return this.m_currentInfo; }
        set { this.m_currentInfo = value; }
    }

    public List<int> Blacklist
    {
        get { return this.m_blacklist; }
        set { this.m_blacklist = value; }
    }
}