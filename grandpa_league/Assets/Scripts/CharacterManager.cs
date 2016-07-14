using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

public class CharacterManager
{
    private List<Child> m_children = new List<Child>();
    private List<Parent> m_parents = new List<Parent>();
    private List<Grandpa> m_grandparents = new List<Grandpa>();

    public CharacterManager()
    {
        XDocument topLevel = XDocument.Load("../Data/characters.xml");
        List<XElement> allCharacters = topLevel.Elements("character").ToList();

        foreach (XElement character in allCharacters)
        {
            switch(Int32.Parse(character.Attribute("type").Value))
            {
                case (int)Enums.Character.CHILD:
                    this.m_children.Add(new Child(  character.Attribute("name").Value,
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
                    this.m_parents.Add(new Parent(  character.Attribute("name").Value,
                                                    character.Attribute("gender").Value,
                                                    character.Attribute("age").Value,
                                                    character.Attribute("wealth").Value,
                                                    character.Attribute("intelligence").Value
                                                    ));
                    break;
                case (int)Enums.Character.GRANDPA:
                    this.m_grandparents.Add(new Grandpa(character.Attribute("name").Value,
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

    public Parent GetRandomParent()
    {
        int randomInt = Constants.RANDOM.Next(this.m_parents.Count);
        Parent returnVal = this.m_parents[randomInt];
        this.m_parents.RemoveAt(randomInt);
        return returnVal;
    }

    public List<Child> GetRandomChildren(int numChildren)
    {
        List<Child> returnList = new List<Child>();
        for (int i = 0; i < numChildren; i++)
        {
            int randomInt = Constants.RANDOM.Next(this.m_children.Count);
            returnList.Add(this.m_children[randomInt]);
            this.m_children.RemoveAt(randomInt);
        }
        return returnList;
    }
}