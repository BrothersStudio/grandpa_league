using System;

[Serializable]
public class PlayerInfo
{
    private     string      m_name = "";

    public PlayerInfo(string name)
    {
        this.m_name = name;
    }
}