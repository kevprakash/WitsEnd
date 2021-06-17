﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Explorer
{

    public override int[] getNumTrees()
    {
        return new int[] { 4, 2, 4, 2, 2 };
    }

    //Camouflage
    public override void ability1(Character[] targets)
    {
        Debug.Log("Camouflaging " + targets[0]);
        Character c = targets[0];
        c.addStatusEffect("dodge", 5 + this.auxiliary, 3);
        c.addStatusEffect("acc", 10 + this.auxiliary, 3);
        c.addStatusEffect("invisible", 0, 1 + this.auxiliary / 8);
        this.party.focus(new int[] { party.getPositionOfCharacter(this), party.getPositionOfCharacter(c) });
    }

    //Tranq Shot
    public override void ability2(Character[] targets)
    {
        Debug.Log("Tranq Shot at " + targets[0]);
        Character c = targets[0];
        if (c.isHit(100 + this.accuracy)) 
        {
            c.takeDamage((int)(10 * (1 + (this.damage/100.0))));
            c.addStatusEffect("stun", 1, 1 + this.auxiliary / 4);
            c.getMarked(3);
        }
        else
        {
            Debug.Log("Missed");
        }
        this.move(1);
        this.party.focus(new int[] { party.getPositionOfCharacter(this)});
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
    }

    //Healing spell
    public override void ability3(Character[] targets)
    {
        Debug.Log("Using healing spell on " + targets[0]);
        Character c = targets[0];
        c.getHealed((int)(20 * (1 + (this.heal/100.0))));
        c.addStatusEffect("acc", 10 + this.auxiliary, 3);
        this.party.focus(new int[] { party.getPositionOfCharacter(this), party.getPositionOfCharacter(c) });
    }

    //Hawk Eye
    public override void ability4(Character[] targets)
    {
        Debug.Log("Hawk targets " + targets[0]);
        Character c = targets[0];
        if (c.isHit(100 + this.accuracy))
        {
            c.takeDamage((int)(25 * (1 + (this.damage / 100.0))));
            c.getMarked(3);
        }
        else
        {
            Debug.Log("Missed");
        }
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
    }

    //Sic' em
    public override void ability5(Character[] targets)
    {
        Debug.Log("Sic'ing wolf on " + targets[0]);
        Character c = targets[0];
        if (c.isHit(100 + this.accuracy))
        {
            c.takeDamage((int)(10 * (1 + (this.damage / 100.0))));
            c.addStatusEffect("bleed", 1 + this.auxiliary/4, 3);
            c.getMarked(3);
        }
        else
        {
            Debug.Log("Missed");
        }
        this.move(-1);
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
    }

    public override int[] calculateInitStats()
    {
        int[][] stats = new int[][]
        {
            new int[]{100, 110, 120, 130, 140},                              //health
            new int[]{0, 0, 0, 0, 0},                                        //prot
            new int[]{10, 15, 20, 25, 30},                                   //dodge
            new int[]{0, 10, 20 , 30 , 40},                                  //dmg
            new int[]{0, 5, 10, 15, 20},                                     //acc
            new int[]{7, 9, 11, 13, 15},                                     //crit
            new int[]{0, 10, 20, 30, 40},                                    //heal
            new int[]{0, 2, 4, 6, 8},                                        //aux
            new int[]{5, 5, 5, 5, 5}                                         //speed
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
        return characteristicsNameToInt(new string[] { "support", "aggressive", "ranged", "indirect", "agile" });
    }

    public override bool[] canUseAbilities()
    {
        return new bool[]
        {
            party.getPositionOfCharacter(this) >= 2,
            party.getPositionOfCharacter(this) >= 2,
            true,
            party.getPositionOfCharacter(this) >= 1,
            party.getPositionOfCharacter(this) < 3,
        };
    }

    public override Character[][][] getValidTargets(Party enemies)
    {
        Character[][][] targets = new Character[][][]
        {
            new Character[][]{new Character[] {party.order[0]}, new Character[] { party.order[1] } , new Character[] { party.order[2] } , new Character[] { party.order[3] } },
            new Character[][]{ new Character[] {enemies.order[2]}, new Character[] { enemies.order[3]} },
            new Character[][]{new Character[] {party.order[0]}, new Character[] { party.order[1] } , new Character[] { party.order[2] } , new Character[] { party.order[3] } },
            new Character[][]{ new Character[] {enemies.order[2]}, new Character[] { enemies.order[3]} },
            new Character[][]{ new Character[] {enemies.order[0]}, new Character[] { enemies.order[1]} },
        };
        return targets;
    }
}
