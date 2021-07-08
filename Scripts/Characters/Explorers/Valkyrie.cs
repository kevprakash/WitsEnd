using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valkyrie : Explorer
{

    public override int[] getNumTrees()
    {
        return new int[] { 3, 1, 2, 2, 1 };
    }

    //Righteous Swing
    public override void ability1(Character[] targets)
    {
        Debug.Log("Righteous swing at " + targets[0]);
        combatPrint("Righteous swing at " + targets[0]);
        Character c = targets[0];
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
        if (c.isHit(100 + this.accuracy))
        {
            bool crit = isCrit();
            c.takeDamage((int)(crit ? 60 : 40) * (1 + (this.damage / 100)));
        }
        else
        {
            Debug.Log("Missed");
        }
    }

    //Become Divine/Return to mortality
    public override void ability2(Character[] targets)
    {
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        if (!isDivine())
        {
            Debug.Log("Becoming Divine");
            combatPrint("Becoming Divine");
            addStatusEffect("dmg", 50 + this.auxiliary, 10000);
            addStatusEffect("dodge", 20 + this.auxiliary/4, 10000);
            addStatusEffect("crit", 10 + this.auxiliary/4, 10000);
            addStatusEffect("speed", 10 + this.auxiliary/4, 10000);
            addStatusEffect("prot", 10 + this.auxiliary, 10000);
            addStatusEffect("divine", 1, 10000);

            //abilitySprites[0] = (Texture2D) Resources.Load("");
            abilitySprites[1] = (Texture2D)Resources.Load("ReturnToMortality");
            //abilitySprites[2] = (Texture2D) Resources.Load("");
            //abilitySprites[3] = (Texture2D) Resources.Load("");
            //abilitySprites[4] = (Texture2D) Resources.Load("");
            idleSprite = (Texture2D)Resources.Load("ValkDivineIdle");
        }
        else
        {
            Debug.Log("Return to Mortality");
            combatPrint("Return to Mortality");
            removeStatusEffect("dmg");
            removeStatusEffect("dodge");
            removeStatusEffect("crit");
            removeStatusEffect("speed");
            removeStatusEffect("prot");
            removeStatusEffect("divine");
            //Debug.Log(getStatusEffect("divine") + " " + isDivine());
            addStatusEffect("dmg", -40 - this.auxiliary, 5);

            //abilitySprites[0] = (Texture2D) Resources.Load("");
            abilitySprites[1] = (Texture2D)Resources.Load("BecomeDivine");
            //abilitySprites[2] = (Texture2D) Resources.Load("");
            //abilitySprites[3] = (Texture2D) Resources.Load("");
            //abilitySprites[4] = (Texture2D) Resources.Load("");
            idleSprite = (Texture2D)Resources.Load("ValkIdle");
        }
    }

    //Lead the Charge
    public override void ability3(Character[] targets)
    {
        Debug.Log("Leading the charge at " + targets[0]);
        combatPrint("Leading the charge at " + targets[0]);
        Character c = targets[0];
        move(-1);
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
        if (c.isHit(100 + this.accuracy))
        {
            bool crit = isCrit();
            c.takeDamage((int)(crit ? 30 : 20) * (1 + (this.damage / 100)));
            if (isDivine())
            {
                c.addStatusEffect("stun", 1, crit ? 2 : 1);
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }

    //Smite
    public override void ability4(Character[] targets)
    {
        Debug.Log("Smiting " + targets[0]);
        combatPrint("Smiting " + targets[0]);
        Character c = targets[0];
        move(1);
        this.party.focus(new int[] { party.getPositionOfCharacter(this) });
        c.getParty().focus(new int[] { c.getParty().getPositionOfCharacter(c) });
        if (c.isHit(100 + this.accuracy))
        {
            bool crit = isCrit();
            c.takeDamage((int)(crit ? 38 : 25) * (1 + (this.damage / 100)), ignoreProt: isDivine());
        }
        else
        {
            Debug.Log("Missed");
        }
    }

    //Pray
    public override void ability5(Character[] targets)
    {
        Debug.Log("Praying");
        combatPrint("Praying");
        this.party.focus(new int[] { 0, 1, 2, 3 });
        foreach (Character c in targets)
        {
            if (c != null)
            {
                Explorer e = (Explorer)c;
                e.modifySanity(5 + this.auxiliary);
            }
        }
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
        for(int i = 0; i < levelStats.Length; i++)
        {
            levelStats[i] = stats[i][level];
        }
        return levelStats;
    }

    public override int[] initCharacteristics()
    {
        return characteristicsNameToInt(new string[] { "combatant", "aggressive", "melee", "direct", "agile" });
    }

    public override bool[] canUseAbilities()
    {
        return new bool[]
        {
            party.getPositionOfCharacter(this) < 2,
            true,
            party.getPositionOfCharacter(this) >= 1,
            party.getPositionOfCharacter(this) < 1,
            true
        };
    }

    public bool isDivine()
    {
        return getStatusEffect("divine").Item2 > 0;
    }

    public override Character[][][] getValidTargets(Party enemies)
    {
        Character[][][] targets = new Character[][][]
        {
            new Character[][]{new Character[] {enemies.order[0]}, new Character[] { enemies.order[1] } , new Character[] { enemies.order[2] } },
            new Character[][]{ new Character[] {this} },
            new Character[][]{new Character[] { enemies.order[0]}, new Character[] { enemies.order[1] } },
            new Character[][]{new Character[] { enemies.order[0]}, new Character[] { enemies.order[1] } },
            new Character[][]{ party.order},
        };
        return targets;
    }
}
