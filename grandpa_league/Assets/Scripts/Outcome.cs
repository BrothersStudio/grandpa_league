public class Outcome
{
    private     int     m_status;
    private     string  m_outcomeString;
    private     string  m_mail              = null;

    public Outcome(int status=-1, string outcome="")
    {
		this.m_status = status;
		this.m_outcomeString = outcome;
    }

    public int Status
    {
        get { return this.m_status; }
        set { this.m_status = value; }
    }

    public string OutcomeDescription
    {
        get { return this.m_outcomeString; }
        set { this.m_outcomeString = value; }
    }

    public string Mail
    {
        get { return this.m_mail; }
        set { this.m_mail = value; }
    }
}

