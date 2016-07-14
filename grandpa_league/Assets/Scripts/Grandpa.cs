using UnityEngine;
using System.Collections;

public class Grandpa 
{

    public string name;

    public int craziness;
    public int wisdom;
    
	public Grandpa(string inputName)
	{
        name = inputName;
        
        craziness = 0;
        wisdom = 100;
	}
    
    public void AddCraziness(int added_craziness)
    {
        craziness += added_craziness;
    }

    public void SubtractCraziness(int subtracted_craziness)
    {
        craziness -= subtracted_craziness;
    }

    public void AddedWisdom(int added_wisdom)
    {
        wisdom += added_wisdom;
    }    
    
    public void SubtractWisdom(int subtracted_wisdom)
    {
        wisdom -= subtracted_wisdom;
    }    
}