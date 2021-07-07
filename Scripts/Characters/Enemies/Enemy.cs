using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Enemy : Character
{

    public static readonly string[] characteristicNames = new string[] {"Fauna", "Flora", "Eldritch", "Gaean", "Ethereal", "Cosmic", "ID1", "ID2", "ID3"};

    public override void atDeathsDoor()
    {
        //throw new System.NotImplementedException();
    }

    public override List<int> compileClassSpecificData()
    {
        return new List<int>();
    }

    public override void onDeathUnique()
    {
        
    }

    public override void setLevelClassSpecific(int level)
    {
        survivalChance = 10;
    }

    public static int[] characteristicsNameToInt(string[] cnames)
    {
        int[] cints = new int[characteristicNames.Length];
        for (int i = 0; i < cints.Length; i++)
        {
            cints[i] = cnames.Contains(characteristicNames[i]) ? 1 : 0;
        }
        return cints;
    }
}
