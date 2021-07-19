using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class CombatOrganizer : MonoBehaviour
{

    public Party playerParty;
    public Party enemyParty;
    public Character[] turnOrder;
    public int turnIndex = 0;
    public MovementControl moveControl;
    public Animator animator;
    public string combatString = "";

    public bool turn = false;

    private 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerParty == null && GameObject.Find("Player party") != null)
        {
            playerParty = GameObject.Find("Player party").GetComponent<Party>();
        }
        if (turn)
        {
            moveControl.canMove = false;
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

        bool hasDivine = false;

        foreach (Party p in new Party[] { playerParty, enemyParty })
        {
            foreach (Character c in p.order)
            {
                if (c == null) continue;
                hasDivine = c.getStatusEffect("divine").Item2 > 0;
                if (hasDivine) break;
            }
            if (hasDivine) break;
        }

        if (hasDivine)
        {
            foreach (Party p in new Party[] { playerParty, enemyParty })
            {
                foreach (Character c in p.order)
                {
                    if (c is Explorer)
                    {
                        ((Explorer)c).modifySanity(-1);
                    }
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

        startTurn();
    }

    public async void startTurn()
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
        else if(turnOrder[turnIndex].getStatusEffect("stun").Item2 > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            endTurn();
        }
        else
        {
            Character c = turnOrder[turnIndex];
            c.bleed();
            c.poison();

            if (c.isAlive())
            {
                c.useAbility(c.getParty() == playerParty ? enemyParty : playerParty);
            }

            if(!playerParty.hasCharacter(c))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(3000));
                endTurn();
            }
            else
            {
                PopupSystem ps = GameObject.Find("UI Manager").GetComponent<PopupSystem>();
                ps.popup("Was this a good choice?");
                Debug.Log("Player character made a move");
            }
        }
    }

    public void approveAction()
    {
        judgeAction(1);
    }

    public void disapproveAction()
    {
        judgeAction(0);
    }

    public async void judgeAction(int judgement)
    {
        Explorer e = (Explorer)turnOrder[turnIndex];
        ((int, int), List<int>) lm = e.lastMemory;
        e.memory[lm.Item1.Item1][lm.Item1.Item2].Add((lm.Item2, judgement));
        animator.SetTrigger("close");
        await e.train(lm.Item1.Item1, lm.Item1.Item2);
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        endTurn();
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


            int randVal = UnityEngine.Random.Range(1, speedSum);
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
            //Destroy(playerParty.gameObject);
            Destroy(enemyParty.gameObject);
        }
        else if (playerParty.allDead())
        {
            Debug.Log("The enemy wins");
            //Destroy(playerParty.gameObject);
        }
        else
        {
            Debug.Log("The player wins");
            Destroy(enemyParty.gameObject);
        }
        turn = false;
        moveControl.canMove = true;
        //Destroy(gameObject);
    }

    public async void spawnEnemies(string[] enemies, int[] levels)
    {
        GameObject ePartyObj = (GameObject) Resources.Load("Enemy Party");
        ePartyObj = Instantiate(ePartyObj, new Vector3(0, 0, 0), new Quaternion()); 
        Party eParty = ePartyObj.GetComponent<Party>();
        for(int i = 0; i < enemies.Length; i++)
        {
            GameObject enemyObj = (GameObject) Resources.Load(enemies[i]);
            enemyObj = Instantiate(enemyObj, eParty.targetLocations[i], new Quaternion());
            Enemy eComp = enemyObj.GetComponent<Enemy>();
            eComp.setLevel(levels[i]);
            eParty.addCharacter(eComp);
            enemyObj.transform.SetParent(eParty.gameObject.transform);
            enemyObj.name = enemies[i] + " " + i;
        }
        eParty.gameObject.transform.SetParent(GameObject.Find("Main Camera").transform);
        eParty.transform.localPosition = new Vector3(0, 0, 2);
        enemyParty = eParty;
        moveControl.canMove = false;
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        turn = true;
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 10000, 20), combatString);
    }
}
