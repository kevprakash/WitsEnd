using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public static readonly string[] statusEffectNames = new string[] { 
        "health", "prot", "dodge", "dmg", "acc", "crit", "heal", "aux", "speed", "bleed", "poison", "stun", "divine", "invisible", "riposte" 
    };

    protected int level;
    protected bool alive = true;
    protected int marked = 0;
    protected int survivalChance = 10;

    public int health;
    protected int maxHealth;
    protected int baseMaxHealth;

    protected int prot;
    protected int baseProt;

    protected int dodge;
    protected int baseDodge;

    protected int damage;
    protected int baseDamage;

    protected int accuracy;
    protected int baseAccuracy;

    protected int critChance;
    protected int baseCritChance;

    protected int heal;
    protected int baseHeal;

    protected int auxiliary;
    protected int baseAuxiliary;

    protected int speed;
    protected int baseSpeed;

    protected int[] characteristics;

    protected Party party;

    protected Dictionary<string, List<(int, int)>> statusEffects = new Dictionary<string, List<(int, int)>>();


    public void Start()
    {
        foreach (string n in statusEffectNames)
        {
            statusEffects.Add(n, new List<(int, int)>());
        }
        setLevel(0);

        characteristics = initCharacteristics();
    }

    public abstract int[] calculateInitStats();

    public abstract int[] initCharacteristics();

    public void takeDamage(int amount, bool ignoreProt = false)
    {
        int modAmount = (int)(amount * (1.0 - (prot / 100.0)));
        if (ignoreProt)
        {
            modAmount = amount;
        }
        Debug.Log("Incoming damag: " + modAmount);
        health = Mathf.Clamp(health - modAmount, 0, maxHealth);
        if(health <= 0)
        {
            if(UnityEngine.Random.value * 100 < survivalChance)
            {
                health = 1;
                atDeathsDoor();
            }
            else
            {
                alive = false;
                onDeath();
            }
        }
        Debug.Log("New Health: " + this.health);
    }

    public void getHealed(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public abstract void atDeathsDoor();

    public abstract void onDeath();

    public void tickStatusEffect(string effectName, bool recalculate = true)
    {
        List<(int, int)> SE = statusEffects[effectName];
        int i = 0;
        bool changed = false;
        while(i < SE.Count)
        {
            SE[i] = (SE[i].Item1, SE[i].Item2 - 1);
            if(SE[i].Item2 <= 0)
            {
                SE.RemoveAt(i);
                changed = true;
            }
            else
            {
                i++;
            }
        }
        if (recalculate && changed)
        {
            recalculateStats();
        }
    }

    //I will literally cry if this is the most efficient way to do this
    public void recalculateStats()
    {
        int maxHealthBonus = 0;
        foreach((int, int) h in statusEffects["health"])
        {
            maxHealthBonus += h.Item1;
        }
        maxHealth = maxHealthBonus + baseMaxHealth;
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        int protBonus = 0;
        foreach ((int, int) p in statusEffects["prot"])
        {
            protBonus += p.Item1;
        }
        prot = Math.Min(100, baseProt + protBonus);

        int dodgeBonus = 0;
        foreach ((int, int) d in statusEffects["dodge"])
        {
            protBonus += d.Item1;
        }
        dodge = Math.Max(0, dodgeBonus + baseDodge);

        int damageBonus = 0;
        foreach((int, int) d in statusEffects["dmg"])
        {
            damageBonus += d.Item1;
        }
        damage = baseDamage + damageBonus;

        int accBonus = 0;
        foreach((int, int) a in statusEffects["acc"])
        {
            accBonus += a.Item1;
        }
        accuracy = baseAccuracy + accBonus;

        int critBonus = 0;
        foreach((int, int) c in statusEffects["crit"])
        {
            critBonus += c.Item1;
        }
        critChance = baseCritChance + critBonus;

        int healBonus = 0;
        foreach((int, int) h in statusEffects["heal"])
        {
            healBonus += h.Item1;
        }
        heal = baseHeal + healBonus;

        int auxBonus = 0;
        foreach((int, int) a in statusEffects["aux"])
        {
            auxBonus += a.Item1;
        }
        auxiliary = baseAuxiliary + auxBonus;

        int speedBonus = 0;
        foreach((int, int) s in statusEffects["speed"])
        {
            speedBonus += s.Item1;
        }
        speed = baseSpeed + speedBonus;
    }

    public void bleed()
    {
        int bleedSum = 0;
        foreach((int, int) b in statusEffects["bleed"])
        {
            bleedSum += b.Item1;
        }
        if(bleedSum > 0)
        {
            int bleedDMG = (int)(bleedSum * 0.05f * maxHealth);
            takeDamage(bleedDMG, ignoreProt: true);
            tickStatusEffect("bleed", recalculate: false);
        }
    }

    public void poison()
    {
        int poisonSum = 0;
        foreach ((int, int) p in statusEffects["poison"])
        {
            poisonSum += p.Item1;
        }
        if (poisonSum > 0)
        {
            takeDamage(poisonSum, ignoreProt: true);
            tickStatusEffect("poison", recalculate: false);
        }
    }

    public void tickEndTurnEffects()
    {
        foreach(string s in new String[] { "health", "prot", "dodge", "dmg", "acc", "crit", "heal", "aux", "speed" })
        {
            tickStatusEffect(s);
        }
        foreach(string s in new String[] { "stun", "divine", "invisible", "riposte" })
        {
            tickStatusEffect(s, recalculate: false);
        }
        marked = Math.Max(0, marked-1);
    }

    //This is technically a mutator, but since the character is getting marked, it's called getMarked instead of setMarked
    public void getMarked(int duration)
    {
        marked = duration;
    }

    public void addStatusEffect(string name, int amount, int duration)
    {
        statusEffects[name.ToLower()].Add((amount, duration));
        recalculateStats();
    }

    public void removeStatusEffect(string name)
    {
        statusEffects[name].Clear();
        recalculateStats();
    }

    public List<int> compileStatusEffects()
    {
        List<int> compiled = new List<int>();
        foreach(string name in statusEffectNames)
        {
            List<(int, int)> SE = statusEffects[name];
            if(SE.Count < 1)
            {
                compiled.Add(0);
                compiled.Add(0);
            }
            else
            {
                int amount = 0;
                int duration = 0;
                foreach((int, int) e in SE)
                {
                    amount += e.Item1;
                    duration += e.Item2;
                }
                compiled.Add(amount);
                compiled.Add(duration);
            }
        }

        return compiled;
    }

    public abstract List<int> compileClassSpecificData();

    public List<int> getData()
    {
        List<int> data = compileClassSpecificData();
        data.AddRange(new int[] {alive ? 1 : 0, level, health, maxHealth, prot, dodge, damage, accuracy, critChance, heal, auxiliary, speed});
        data.AddRange(characteristics);
        data.AddRange(compileStatusEffects());

        return data;
    }

    public void setLevel(int level)
    {
        this.level = level;
        int[] initStats = calculateInitStats();

        health = initStats[0];
        maxHealth = initStats[0];
        baseMaxHealth = initStats[0];

        prot = initStats[1];
        baseProt = initStats[1];

        dodge = initStats[2];
        baseDodge = initStats[2];

        damage = initStats[3];
        baseDamage = initStats[3];

        accuracy = initStats[4];
        baseAccuracy = initStats[4];

        critChance = initStats[5];
        baseCritChance = initStats[5];

        heal = initStats[6];
        baseHeal = initStats[6];

        auxiliary = initStats[7];
        baseAuxiliary = initStats[7];

        speed = initStats[8];
        baseSpeed = initStats[8];

        setLevelClassSpecific(level);
    }

    public abstract void setLevelClassSpecific(int level);

    public void setParty(Party p)
    {
        party = p;
    }

    public void focus(Character[] allies, Character[] enemies, float duration = 1f)
    {
        int[] allyIndices = new int[allies.Length + 1];
        int[] enemyIndices = new int[enemies.Length];
        allyIndices[0] = party.getPositionOfCharacter(this);

        for(int i = 0; i < allies.Length; i++)
        {
            allyIndices[i + 1] = party.getPositionOfCharacter(allies[i]);
        }
        for(int i = 0; i < enemies.Length; i++)
        {
            enemyIndices[i] = party.getPositionOfCharacter(enemies[i]);
        }

        party.focus(allyIndices, duration: duration);
        enemies[0].party.focus(enemyIndices, duration: duration);
    }

    public void move(int amount)
    {
        int position = party.getPositionOfCharacter(this);
        amount = Mathf.Clamp(amount, -position, party.getCharacterCount() - position - 1);
        party.move(position, position + amount);
    }

    public void moveTowardsDefault()
    {
        int defaultPos = party.getDefaultPositionOfCharacter(this);
        int position = party.getPositionOfCharacter(this);
        move(defaultPos > position ? 1 : defaultPos < position ? 1 : 0);
        party.focus(new int[] { party.getPositionOfCharacter(this) });
    }

    public abstract void useAbility(Party enemies);
 
    public bool isHit(int acc)
    {
        int mod = acc - this.dodge;
        return UnityEngine.Random.Range(0, 100) < mod;
    }

    public Party getParty()
    {
        return party;
    }

    public (int, int) getStatusEffect(string name)
    {
        List<(int, int)> SE = statusEffects[name];
        int amount = 0;
        int duration = 0;
        foreach ((int, int) e in SE)
        {
            amount += e.Item1;
            duration += e.Item2;
        }
        return (amount, duration);
    }

    public bool isNullTargets(Character[] targets)
    {
        bool isNull = true;
        foreach (Character c in targets)
        {
            isNull &= c == null || c.getStatusEffect("invisible").Item2 > 0;
            if (!isNull) break;
        }
        return isNull;
    }
}
