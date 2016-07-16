using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

public class CharacterManager
{
    private static List<Child> m_children = new List<Child>();
    private static List<Parent> m_parents = new List<Parent>();
    private static List<Grandpa> m_grandparents = new List<Grandpa>();

    static CharacterManager()
    {
		XDocument topLevel = XDocument.Load(Application.dataPath + "/Data/characters.xml");
		List<XElement> allCharacters = topLevel.Root.Descendants("character").ToList();

        foreach (XElement character in allCharacters)
        {
            switch(Int32.Parse(character.Attribute("type").Value))
            {
                case (int)Enums.Character.CHILD:
                    m_children.Add(new Child(  character.Attribute("name").Value,
                                                    character.Attribute("gender").Value,
                                                    character.Attribute("age").Value,
                                                    character.Attribute("cuteness").Value,
                                                    character.Attribute("intelligence").Value,
                                                    character.Attribute("fine_motor").Value,
                                                    character.Attribute("gross_motor").Value,
                                                    character.Attribute("disposition").Value
                                                    ));
                    break;
                case (int)Enums.Character.PARENT:
                    m_parents.Add(new Parent(  character.Attribute("name").Value,
                                                    character.Attribute("gender").Value,
                                                    character.Attribute("age").Value,
                                                    character.Attribute("wealth").Value,
                                                    character.Attribute("intelligence").Value
                                                    ));
                    break;
                case (int)Enums.Character.GRANDPA:
                    m_grandparents.Add(new Grandpa(character.Attribute("name").Value,
                                                        character.Attribute("age").Value,
                                                        character.Attribute("wisdom").Value,
                                                        character.Attribute("insanity").Value
                                                    ));
                    break;
                default:
                    break; 
            }
        }
    }

    public static Parent GetRandomParent()
    {
        int randomInt = Constants.RANDOM.Next(m_parents.Count);
        Parent returnVal = m_parents[randomInt];
        m_parents.RemoveAt(randomInt);
        return returnVal;
    }

    public static List<Child> GetRandomChildren(int numChildren)
    {
        List<Child> returnList = new List<Child>();
        for (int i = 0; i < numChildren; i++)
        {
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
}