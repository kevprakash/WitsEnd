using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Threading.Tasks;

public abstract class Explorer : Character
{
    public static readonly Dictionary<string, int> nameToInt;
    protected int sanity = 100;
    protected int armorLevel;
    protected int equipLevel;
    protected string explorerName;
    public DecisionTree[][] decisionTrees;
    public List<(List<int>, int)>[][] memory;

    public bool trainTest = false;

    public ((int, int), List<int>) lastMemory;

    static Explorer()
    {
        nameToInt = new Dictionary<string, int>();
        string[] charNames = new string[] { "support", "combatant", "aggressive", "defensive", "ranged", "melee", "direct", "indirect", "agile", "fortified" };
        for (int i = 0; i < charNames.Length; i++)
        {
            nameToInt.Add(charNames[i], i % 2);
        }
    }

    public void Update()
    {
        if (trainTest)
        {
            trainTest = false;
            train();
        }
    }

    public DecisionTree[][] initializeTrees(int[] numTrees)
    {
        DecisionTree[][] dts = new DecisionTree[5][];
        memory = new List<(List<int>, int)>[5][];
        for(int i = 0; i < 5; i++)
        {
            dts[i] = new DecisionTree[numTrees[i]];
            memory[i] = new List<(List<int>, int)>[numTrees[i]];
            for(int j = 0; j < numTrees[i]; j++)
            {
                dts[i][j] = new DecisionTree();
                memory[i][j] = new List<(List<int>, int)>();
            }
        }
        return dts;
    }

    public override List<int> compileClassSpecificData()
    {
        return new List<int>(new int[] {sanity, armorLevel, equipLevel});
    }

    public abstract void ability1(Character[] targets);

    public abstract void ability2(Character[] targets);
    
    public abstract void ability3(Character[] targets);
    
    public abstract void ability4(Character[] targets);
    
    public abstract void ability5(Character[] targets);

    public string getName()
    {
        return explorerName;
    }

    public override void atDeathsDoor()
    {
        //throw new System.NotImplementedException();
    }

    public override void onDeathUnique()
    {

    }

    public static int[] characteristicsNameToInt(string[] cnames)
    {
        int[] cints = new int[cnames.Length];
        for(int i = 0; i < cints.Length; i++)
        {
            cints[i] = nameToInt[cnames[i].ToLower()];
        }
        return cints;
    }

    public override void setLevelClassSpecific(int level)
    {
        armorLevel = level;
        equipLevel = level;
        survivalChance = 60;
        if(decisionTrees == null)
            decisionTrees = initializeTrees(getNumTrees());
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public abstract int[] getNumTrees();

    public void useAbilityFromIndex(int index, Character[] targets)
    {
        index = index + 1;
        switch (index)
        {
            case 1:
                ability1(targets);
                break;
            case 2:
                ability2(targets);
                break;
            case 3:
                ability3(targets);
                break;
            case 4:
                ability4(targets);
                break;
            case 5:
                ability5(targets);
                break;
            default:
                Debug.Log("Attempted to use Ability " + index);
                throw new System.NotImplementedException();
        }

        targetSprite = abilitySprites[index-1];
    }

    public abstract bool[] canUseAbilities();

    public bool[][] getEvaluations(int[] row)
    {
        //Debug.Log("Getting evaluations");
        bool[] canUse = canUseAbilities();
        bool[][] evaluations = new bool[5][];
        for(int i = 0; i < evaluations.Length; i++)
        {
            evaluations[i] = new bool[decisionTrees[i].Length];
            if (!canUse[i]) continue;

            for(int j = 0; j < evaluations[i].Length; j++)
            {
                evaluations[i][j] = decisionTrees[i][j].evaluate(row);
            }
        }
        return evaluations;
    }

    public override void useAbility(Party enemies)
    {
        List<int> temp = party.getPartyData();
        temp.AddRange(enemies.getPartyData());
        int[] data = temp.ToArray();
        bool[][] evals = getEvaluations(data);

        Character[][][] targets = removeInvalidTargets(getValidTargets(enemies));

        List<((int, int), Character[])> candidates = new List<((int, int), Character[])>();
        List<((int, int), Character[])> fails = new List<((int, int), Character[])>();

        for (int i = 0; i < evals.Length; i++)
        {
            for (int j = 0; j < evals[i].Length; j++)
            {
                if (targets[i][j] != null)
                {
                    if (evals[i][j])
                    {
                        candidates.Add(((i, j), targets[i][j]));
                    }
                    else
                    {
                        fails.Add(((i, j), targets[i][j]));
                    }
                }
            }
        }

        bool hasCandidates = candidates.Count > 0;
        int index = Random.Range(0, hasCandidates ? candidates.Count : fails.Count);
        while(isNullTargets((hasCandidates ? candidates[index] : fails[index]).Item2)){
            if (hasCandidates)
            {
                candidates.RemoveAt(index);
                hasCandidates = candidates.Count > 0;
            }
            else
            {
                fails.RemoveAt(index);
                if(fails.Count < 1)
                {
                    moveTowardsDefault();
                }
            }
            index = Random.Range(0, hasCandidates ? candidates.Count : fails.Count);
        }
        ((int, int), Character[]) info = hasCandidates ? candidates[index] : fails[index];
        //Debug.Log(info);
        useAbilityFromIndex(info.Item1.Item1, info.Item2);
        //memory[info.Item1.Item1][info.Item1.Item2].Add((temp, Random.Range(0, 2)));
        lastMemory = (info.Item1, temp);
    }

    public abstract Character[][][] getValidTargets(Party enemies);

    public void modifySanity(int amount)
    {
        sanity = Mathf.Clamp(sanity + amount, 0, 100);
    }

    public async Task train()
    {
        Debug.Log("Began training");
        for(int i = 0; i < decisionTrees.Length; i++)
        {
            DecisionTree[] abilityTrees = decisionTrees[i];
            List<(List<int>, int)>[] abilityMemory = memory[i];
            for(int j = 0; j < abilityTrees.Length; j++)
            {
                if(abilityMemory[j].Count < 1)
                {
                    continue;
                }
                double[][] memoryArray = abilityMemory[j].Select(a => a.Item1.Select(b => (double)b).ToArray()).ToArray();
                Matrix<double> memoryMatrix = CreateMatrix.DenseOfRowArrays<double>(memoryArray);
                Vector<double> labelVector = CreateVector.Dense<double>(abilityMemory[j].Select(a => (double)a.Item2).ToArray());
                await abilityTrees[j].build(memoryMatrix, labelVector, maxDepth: (level + 1) * 10);
            }
        }
        Debug.Log("Finished training");
    }

    public int getSanity()
    {
        return sanity;
    }

    public Character[][][] removeInvalidTargets(Character[][][] targets)
    {
        for(int i = 0; i < targets.Length; i++)
        {
            Character[][] abilityTargets = targets[i];
            for(int j = 0; j < abilityTargets.Length; j++)
            {
                List<Character> validTargets = new List<Character>();
                Character[] t = abilityTargets[j];
                foreach(Character c in t)
                {
                    if(c != null && c.getStatusEffect("invisible").Item2 <= 0)
                    {
                        validTargets.Add(c);
                    }
                }
                if(validTargets.Count > 0)
                {
                    targets[i][j] = validTargets.ToArray();
                }
                else
                {
                    targets[i][j] = null;
                }
            }
        }
        return targets;
    }
}


