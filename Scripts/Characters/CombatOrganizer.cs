using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatOrganizer : MonoBehaviour
{

    public Party playerParty;
    public Party enemyParty;
    public Character[] turnOrder;
    public int turnIndex = 0;

    public bool turn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turn)
        {
            turn = false;
            if(playerParty != null && enemyParty != null && (turnOrder == null || turnOrder.Length == 0))
            {
                nextRound();
            }
            startTurn();
        }
    }

    public void endTurn()
    {
        if(playerParty.allDead() || enemyParty.allDead())
        {
            combatEnd();
            return;
        }
        foreach (Party p in new Party[] { playerParty, enemyParty }) 
        { 
            bool hasDivine = false;
            foreach (Character c in p.order)
            {
                if (c == null) continue;
                hasDivine = c.getStatusEffect("divine").Item2 > 0;
                if (hasDivine) break;
            }

            foreach (Character c in p.order)
            {
                if (c is Explorer)
                {
                    ((Explorer)c).modifySanity(-5);
                }
            }
        }

        if(turnOrder[turnIndex] != null && turnOrder[turnIndex].isAlive())
            turnOrder[turnIndex].tickEndTurnEffects();
        turnIndex++;
        if(turnIndex >= turnOrder.Length)
        {
            nextRound();
        }
        //startTurn();
    }

    public void startTurn()
    {
        if(turnOrder[turnIndex] == null)
        {
            turnIndex++;
            if (turnIndex >= turnOrder.Length)
            {
                nextRound();
            }
            startTurn();
        }
        else
        {
            Character c = turnOrder[turnIndex];
            c.bleed();
            c.poison();

            if(c.isAlive())
                c.useAbility(c.getParty() == playerParty ? enemyParty : playerParty);

            endTurn();
        }
    }

    public void nextRound()
    {
        turnIndex = 0;
        List<Character> toAdd = new List<Character>();
        toAdd.AddRange(playerParty.order);
        toAdd.AddRange(enemyParty.order);

        turnOrder = new Character[toAdd.Count];
        int insertIndex = 0;
        toAdd.RemoveAll(a => a == null);
        while (toAdd.Count > 1)
        {
            int speedSum = 0;
            int min = 0;
            foreach (Character c in toAdd)
            {
                if (c.getSpeed() < min)
                {
                    min = c.getSpeed();
                }
            }
            foreach (Character c in toAdd)
            {
                speedSum += c.getSpeed() + 1 - min;
            }


            int randVal = Random.Range(1, speedSum);
            int temp = 0;
            int i = 0;
            for(; i < toAdd.Count && temp < randVal; i++)
            {
                temp += toAdd[i].getSpeed();
            }
            turnOrder[insertIndex] = toAdd[i-1];
            toAdd.RemoveAt(i-1);
            insertIndex++;
            //Debug.Log(insertIndex + " " + toAdd.Count);
        }
        turnOrder[insertIndex] = toAdd[0];
    }

    public void combatEnd()
    {
        if(playerParty.allDead() && enemyParty.allDead())
        {
            Debug.Log("The battle ends in a draw");
            Destroy(playerParty.gameObject);
            Destroy(enemyParty.gameObject);
        }
        else if (playerParty.allDead())
        {
            Debug.Log("The enemy wins");
            Destroy(playerParty.gameObject);
        }
        else
        {
            Debug.Log("The player wins");
            Destroy(enemyParty.gameObject);
        }
        Destroy(gameObject);
    }
}
