using System;
using System.Collections.Generic;

public class DataManager
{
    private PlayerInfo          m_currentInfo       = null;
    private Calendar            m_currentCalendar   = null;
    private List<Family>        m_league            = new List<Family>();
    private Family              m_playerFamily      = null;

    private CharacterManager    m_characterManager  = null;

    public DataManager (string playerName)
    {
        this.m_characterManager = new CharacterManager();
        this.m_currentInfo = new PlayerInfo(playerName);
        this.m_currentCalendar = new Calendar();

        //for (int i = 0; i < Constants.NUM_FAMILIES; i++)
        //    this.m_league.Add(new Family(true));

        this.m_playerFamily = new Family();
        this.m_playerFamily.Grandpa = new Grandpa(playerName);
        this.m_playerFamily.Parent = this.m_characterManager.GetRandomParent();
        this.m_playerFamily.Children = this.m_characterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);
    }

    public Family PlayerFamily
    {
        get { return this.m_playerFamily; }
    }
}