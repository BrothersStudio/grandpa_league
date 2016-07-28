using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

static class Qualification
{
    private static Dictionary<string, int> m_qualifications = new Dictionary<string, int>();        //TODO: fix this fucking mess of a data structure hole I dug myself
    private static Dictionary<int, string> m_qualificationNames = new Dictionary<int, string>();
    private static Dictionary<int, bool> m_qualificationHidden = new Dictionary<int, bool>();

    static Qualification()
    {
        TextAsset tmp = Resources.Load("qualifications") as TextAsset;
        TextReader reader = new StringReader(tmp.text);
        XDocument topLevel = XDocument.Load(reader);
        List<XElement> allQualifications = topLevel.Root.Descendants("qualification").ToList();

        foreach (XElement qual in allQualifications)
        {
            m_qualifications.Add(qual.Attribute("name").Value, Int32.Parse(qual.Attribute("id").Value));
            m_qualificationNames.Add(Int32.Parse(qual.Attribute("id").Value), qual.Attribute("display_name").Value);
            m_qualificationHidden.Add(Int32.Parse(qual.Attribute("id").Value), Convert.ToBoolean(qual.Attribute("hidden").Value));
        }
    }

    public static string GetDisplayName(int qualNumber)
    {
        return m_qualificationNames[qualNumber];
    }

    public static int GetQualificationByString(string qualName)
    {
        return m_qualifications[qualName];
    }

    public static string GetQualificationString(int qualNumber)
    {
        foreach(KeyValuePair<string, int> entry in m_qualifications)
        {
            if (entry.Value == qualNumber)
                return entry.Key;
        }
        return "NONE";
    }

    public static bool IsQualificationHidden(int id)
    {
        foreach (KeyValuePair<int, bool> entry in m_qualificationHidden)
        {
            if (entry.Key == id)
                return entry.Value;
        }
        Debug.LogError("Looking for hidden qualification that isn't in the database!");
        return false;
    }
}

