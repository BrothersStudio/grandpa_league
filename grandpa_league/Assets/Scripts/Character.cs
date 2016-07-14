public class Character
{
    protected string    m_name      = "";
    protected int       m_gender    = 0;
    protected int       m_age       = 0;

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

}