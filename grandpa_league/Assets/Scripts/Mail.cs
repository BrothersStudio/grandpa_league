using System.Collections.Generic;
using System;

[Serializable]
public class Mail
{
    private string m_message = "";
    private string m_subject = "";
    private string m_sender = "";
    private Dictionary<string, int> m_date = null;
    private string m_stringDate = "";
    private bool m_read = false;
    private string m_image = null;

    public Mail(string subject="", string message="", string sender="", Dictionary<string, int> date=null)
    {
        this.m_subject = subject;
        this.m_message = message;
        this.m_date = date;
        this.m_sender = sender;

        if (date != null)
        {
            this.m_stringDate = string.Format("{0}/{1}/{2}", date["month"], date["day"], date["year"]);
        }
    }

    public string GetStringDate()
    {
        if (this.m_date != null && this.m_stringDate == "")
            this.m_stringDate = string.Format("{0}/{1}/{2}", m_date["month"], m_date["day"], m_date["year"]);
        return this.m_stringDate;
    }


    public string Message
    {
        get { return this.m_message; }
        set { this.m_message = value;  }
    }

    public string Subject
    {
        get { return this.m_subject; }
        set { this.m_subject = value; }
    }

    public string StringDate
    {
        get { return this.GetStringDate(); }
        set { this.m_stringDate = value; }
    }

    public Dictionary<string, int> Date
    {
        get { return this.m_date; }
        set { this.m_date = value; }
    }

    public string Sender
    {
        get { return this.m_sender;  }
        set { this.m_sender = value; }
    }

    public bool Read
    {
        get { return this.m_read; }
        set { this.m_read = value; }
    }

    public string Image
    {
        get { return this.m_image; }
        set { this.m_image = value; }
    }
}