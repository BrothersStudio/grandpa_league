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
        this.m_playerFamily.Grandpa.Money = Constants.Player.INITIAL_MONEY;
        this.m_playerFamily.Grandpa.MoneyGrowth = Constants.Player.INITIAL_INCOME;
        this.m_playerFamily.Parents = CharacterManager.GetRandomParents(Constants.INITIAL_PARENTS);
        this.m_playerFamily.Children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);

        string[] splitName = playerName.Split(' ');
        if (splitName.Length >= 2)
            this.m_playerFamily.FamilyName = splitName[splitName.Length - 1];
        else
            this.m_playerFamily.FamilyName = Constants.Player.DEFAULT_SURNAME;

        for (int i = 1; i < Constants.NUM_FAMILIES; i++)
            this.m_league.Add(new Family(true));

        this.m_playerFamily.Mailbox.Add(this.GetInitialMail());
    }

    private Mail GetInitialMail()
    {
        Mail mail = new Mail();
        mail.StringDate = "January 1, 2016";
        mail.Sender = string.Format("{0} {1}", this.m_playerFamily.Parents[0].Name, this.m_playerFamily.FamilyName);
        mail.Subject = "Your new home";
        mail.Message = string.Format("Hey Dad,\n\n\tEnjoying the new digs at the nursing home yet? I promise this is just a temporary situation until we can figure out why you were wandering around the neighbor's garage in the middle of the night. Anyways, the kids are excited to see you this weekend! Looking forward to seeing you. \n\nLove,\n{0}", this.m_playerFamily.Parents[0].Name);
        return mail;
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