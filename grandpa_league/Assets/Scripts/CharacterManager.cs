using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.IO;

[Serializable]
public class CharacterManager
{
    private static List<Child> m_children = new List<Child>();
    private static List<Parent> m_parents = new List<Parent>();
    private static List<Grandpa> m_grandparents = new List<Grandpa>();

    static CharacterManager()
    {
        TextAsset tmp = Resources.Load("characters") as TextAsset;
        TextReader reader = new StringReader(tmp.text);
        XDocument topLevel = XDocument.Load(reader);
		List<XElement> allCharacters = topLevel.Root.Descendants("character").ToList();

        foreach (XElement character in allCharacters)
        {
            switch(Int32.Parse(character.Attribute("type").Value))
            {
                case (int)Enums.Character.CHILD:
                    m_children.Add(new Child(  character.Attribute("name").Value,
                                                    Int32.Parse(character.Attribute("gender").Value),
                                                    Int32.Parse(character.Attribute("age").Value),
                                                    Int32.Parse(character.Attribute("cuteness").Value),
                                                    Int32.Parse(character.Attribute("intelligence").Value),
                                                    Int32.Parse(character.Attribute("artistry").Value),
                                                    Int32.Parse(character.Attribute("athleticism").Value),
                                                    Int32.Parse(character.Attribute("popularity").Value),
                                                    character.Attribute("sprite_name").Value
                                                    ));
                    break;
                case (int)Enums.Character.PARENT:
                    m_parents.Add(new Parent(  character.Attribute("name").Value,
                                                    Int32.Parse(character.Attribute("gender").Value),
                                                    Int32.Parse(character.Attribute("age").Value),
                                                    Int32.Parse(character.Attribute("intelligence").Value),
                                                    Int32.Parse(character.Attribute("popularity").Value),
                                                    Int32.Parse(character.Attribute("love").Value),
                                                    character.Attribute("sprite_name").Value
                                                    ));
                    break;
                case (int)Enums.Character.GRANDPA:
                    m_grandparents.Add(new Grandpa(character.Attribute("name").Value,
                                                        Int32.Parse(character.Attribute("age").Value),
														Int32.Parse(character.Attribute("pride").Value),
                                                        Int32.Parse(character.Attribute("wisdom").Value),
                                                        Int32.Parse(character.Attribute("insanity").Value),
                                                        Int32.Parse(character.Attribute("money").Value),
                                                        character.Attribute("sprite_name").Value
                                                    ));
                    break;
                default:
                    break; 
            }
        }
    }

	public static List<Parent> GetRandomParents(int numParents)
    {
		List<Parent> returnList = new List<Parent>();
		for (int i = 0; i < numParents; i++)
		{
			if (m_parents.Count == 0)
				Debug.LogError ("Trying to assign random parents, but none are left!");
			int randomInt = Constants.RANDOM.Next(m_parents.Count);
			returnList.Add(m_parents[randomInt]);
			m_parents.RemoveAt(randomInt);
		}
		return returnList;
    }

    public static List<Child> GetRandomChildren(int numChildren)
    {
        List<Child> returnList = new List<Child>();
        for (int i = 0; i < numChildren; i++)
        {
			if (m_children.Count == 0)
				Debug.LogError ("Trying to assign random children, but none are left!");			
            int randomInt = Constants.RANDOM.Next(m_children.Count);
            returnList.Add(m_children[randomInt]);
            m_children.RemoveAt(randomInt);
        }
        return returnList;
    }

    public static Grandpa GetRandomGrandpa()
    {
        int randomInt = Constants.RANDOM.Next(m_grandparents.Count);
        Grandpa returnVal = m_grandparents[randomInt];
        m_grandparents.RemoveAt(randomInt);
        return returnVal;
    }

    public static List<Child> GetRemainingChildren()
    {
        return m_children;
    }

    public static List<Parent> GetRemainingParents()
    {
        return m_parents;
    }

    public static List<Grandpa> GetRemainingGrandpas()
    {
        return m_grandparents;
    }
}