using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : Enemy
{
    public override int[] calculateStats()
    {
        int[][] stats = new int[][]
        {
            new int[]{80, 90, 100, 110, 120},                              //health
            new int[]{0, 0, 0, 0, 0},                                        //prot
            new int[]{10, 13, 15, 18, 20},                                    //dodge
            new int[]{0, 5, 10 , 15 , 20},                                  //dmg
            new int[]{0, 5, 5, 10, 10},                                     //acc
            new int[]{2, 4, 6, 8, 10},                                       //crit
            new int[]{0, 0, 0, 0, 0},                                        //heal
            new int[]{0, 0, 0, 0, 0},                                        //aux
            new int[]{0, 0, 0, 0, 0}                                         //speed
        };
        int[] levelStats = new int[stats.Length];
        for (int i = 0; i < levelStats.Length; i++)
        {
            levelStats[i] = stats[i][level];
        }
        return levelStats;
    }

    public override int[] initCharacteristics()
    {
        return characteristicsNameToInt(new string[] {"Fauna"});
    }

    public override void useAbility(Party enemies)
    {
        List<Character> possibleTargets = new List<Character>();
        foreach (Character c in enemies.order)
        {
            if (c != null && c.getStatusEffect("invisible").Item2 <= 0)
                possibleTargets.Add(c);
        }
        if (possibleTargets.Count > 0) { 
            int targetIndex = Random.Range(0, possibleTargets.Count);
            Character target = possibleTargets[targetIndex];
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                spit(target);
            }
            else
            {
                lunge(target);
            }
        }
        
    }

    public void spit(Character target)
    {
        Debug.Log("Spitting at " + target);
        //combatPrint("Spitting at " + target);
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        target.getParty().focus(new int[] { target.getParty().getPositionOfCharacter(target) });
        if(target.isHit(60 + this.accuracy))
        {
            bool crit = isCrit();
            target.takeDamage((int)((crit ? 23 : 15) * (1 + (this.damage / 100))));
            combatPrint(this + " spit at " + target + " dealing " + (int)((crit ? 23 : 15) * (1 + (this.damage / 100))) + " damage");
        }
        else
        {
            Debug.Log("Missed");
            combatPrint(this + " spit at " + target + " but missed");
        }
        targetSprite = abilitySprites[0];
    }

    public void lunge(Character target)
    {
        Debug.Log("Lunging at " + target);
        //combatPrint("Lunging at " + target);
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        target.getParty().focus(new int[] { target.getParty().getPositionOfCharacter(target) });
        if (target.isHit(80 + this.accuracy))
        {
            bool crit = isCrit();
            target.takeDamage((int)((crit ? 15 : 10) * (1 + (this.damage / 100))));
            combatPrint(this + " lunged at " + target + " dealing " + (int)((crit ? 15 : 10) * (1 + (this.damage / 100))) + " damage");
        }
        else
        {
            Debug.Log("Missed");
            combatPrint(this + " lunged at " + target + " but missed");
        }
        targetSprite = abilitySprites[1];
    }
}
