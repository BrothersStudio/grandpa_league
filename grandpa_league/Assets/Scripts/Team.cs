using UnityEngine;
using System.Collections;

public class Team 
{

    public string name;
    
    public Grandpa grandpa;
    public Parent[] parents;
    public Child[] children;
    
    public int cohesion;
    public int score;

	public Team(string inputName)
	{
        name = inputName;
        
        cohesion = 0;
        score = 0;
	}

    public void AddChild(Child newChild)
    {
        
    }
    
    public void RemoveChild(Child removedChild)
    {
        
    }  
 
    public void TradeChildren(Child newChild, Child removedChild)
    {
        
    }    
}