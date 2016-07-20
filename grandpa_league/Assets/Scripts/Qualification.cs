using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

static class Qualification
{
    private static Dictionary<string, int> m_qualifications = new Dictionary<string, int>();

    static Qualification()
    {
        TextAsset tmp = Resources.Load("qualifications") as TextAsset;
        TextReader reader = new StringReader(tmp.text);
        XDocument topLevel = XDocument.Load(reader);
        List<XElement> allQualifications = topLevel.Root.Descendants("qualification").ToList();

        foreach (XElement qual in allQualifications)
        {
            m_qualifications.Add(qual.Attribute("name").Value, Int32.Parse(qual.Attribute("id").Value));
        }
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
}

