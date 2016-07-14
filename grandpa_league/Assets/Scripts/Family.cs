using System.Collections.Generic;

public class Family
{
    private     string      m_familyName    = "";
    private     Grandpa     m_grandpa       = null;
    private     Parent      m_parent        = null;
    private     List<Child> m_children      = new List<Child>();

    public Family()
    {
    }

    public Grandpa Grandpa
    {
        get { return this.m_grandpa; }
        set { this.m_grandpa = value; }
    }

    public Parent Parent
    {
        get { return this.m_parent; }
        set { this.m_parent = value; }
    }

    public List<Child> Children
    {
        get { return this.m_children; }
        set { this.m_children = value; }
    }
}