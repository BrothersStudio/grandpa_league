public class Outcome
{
    private     int     m_status            = -1;
    private     string  m_outcomeString     = "";

    public Outcome(int status, string outcomeDescription)
    {
        this.m_status = status;
        this.m_outcomeString = outcomeDescription;
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
}

