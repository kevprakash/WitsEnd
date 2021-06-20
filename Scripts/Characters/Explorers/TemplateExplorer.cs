using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateExplorer : Explorer
{

    public override int[] getNumTrees()
    {
        return new int[] {0, 0, 0, 0, 0};
    }

    public override void ability1(Character[] targets)
    {
        throw new System.NotImplementedException();
    }

    public override void ability2(Character[] targets)
    {
        throw new System.NotImplementedException();
    }

    public override void ability3(Character[] targets)
    {
        throw new System.NotImplementedException();
    }

    public override void ability4(Character[] targets)
    {
        throw new System.NotImplementedException();
    }

    public override void ability5(Character[] targets)
    {
        throw new System.NotImplementedException();
    }

    public override int[] calculateStats()
    {
        int[][] stats = new int[][]
        {
            new int[]{100, 115, 130, 145, 160},                              //health
            new int[]{0, 0, 0, 0, 0},                                        //prot
            new int[]{5, 10, 15, 20, 25},                                    //dodge
            new int[]{0, 20, 40 , 60 , 80},                                  //dmg
            new int[]{0, 5, 10, 15, 20},                                     //acc
            new int[]{6, 7, 8, 9, 10},                                       //crit
            new int[]{0, 0, 0, 0, 0},                                        //heal
            new int[]{0, 0, 0, 0, 0},                                        //aux
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
        throw new System.NotImplementedException();
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
            new Character[][]{ new Character[] {enemies.order[2]}, new Character[] { party.order[3]} },
            new Character[][]{new Character[] {party.order[0]}, new Character[] { party.order[1] } , new Character[] { party.order[2] } , new Character[] { party.order[3] } },
            new Character[][]{ new Character[] {enemies.order[2]}, new Character[] { party.order[3]} },
            new Character[][]{ new Character[] {enemies.order[0]}, new Character[] { party.order[1]} },
        };
        return targets;
    }
}
