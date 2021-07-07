using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticInformationHolder : MonoBehaviour
{
    public enum ExplorerClass
    {
        Medic, Ranger, Sniper, Valkyrie
    }

    public static Dictionary<string, (ExplorerClass, int)> roster = new Dictionary<string, (ExplorerClass, int)>();
    public static Dictionary<string, (ExplorerClass, int)> recruits = new Dictionary<string, (ExplorerClass, int)>();
    public static (ExplorerClass, int, string)[] party = new (ExplorerClass, int, string)[4];

    static StaticInformationHolder()
    {
        roster["Tracy"] = (ExplorerClass.Ranger, 0);
        roster["Anastacia"] = (ExplorerClass.Valkyrie, 0);
        addToParty("Anastacia", 0);
        addToParty("Tracy", 1);
    }


    public static void addToParty(string name, int index)
    {
        (ExplorerClass, int) exp = roster[name];
        party[index] = (exp.Item1, exp.Item2, name);
    }

    public static void createPartyGameObject()
    {
        GameObject partyObj = Instantiate(Resources.Load<GameObject>("Empty"));
        Party partyComp = partyObj.AddComponent(typeof(Party)) as Party;
        partyObj.name = "Player party";
        partyObj.transform.SetParent(GameObject.Find("Main Camera").transform);
        partyObj.transform.localPosition = new Vector3(0, 0, 2);
        partyComp.ownedByPlayer = true;

        initializePartyInfo(partyComp);
    }

    public static void initializePartyInfo(Party partyRef)
    {
        for (int i = 0; i < party.Length; i++)
        {
            if(party[i].Item3 == null)
            {
                continue;
            }
            (ExplorerClass, int, string) e = party[i];
            GameObject explorerRef = Resources.Load<GameObject>(e.Item1.ToString());
            GameObject explorerObj = Instantiate(explorerRef);
            Explorer eComp = explorerObj.GetComponent(typeof(Explorer)) as Explorer;
            eComp.setLevel(e.Item2);
            eComp.setName(e.Item3);
            explorerObj.name = e.Item3;
            partyRef.addCharacter(eComp);
            explorerObj.transform.SetParent(partyRef.gameObject.transform);
        }
    }

    public static void recruit(string name)
    {
        roster[name] = recruits[name];
        recruits.Remove(name);
    }

    public static void dismiss(string name)
    {
        roster.Remove(name);
    }

    public static void generateRecruits(int num)
    {
        recruits.Clear();
        for(int i = 0; i < num; i++)
        {
            ExplorerClass eClass = (ExplorerClass)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ExplorerClass)).Length);
            string name = getRandomName(eClass);
            recruits[name] = (eClass, 0);
        }
    }

    public static string getRandomName(ExplorerClass eClass, int counter = 0)
    {
        string[] nameList = null;
        switch (eClass)
        {
            case ExplorerClass.Ranger:
                nameList = new string[]
                {
                        "Tina", "Sarah", "Eve", "Artemis", "Lily", "Tiffany"
                };
                break;
            case ExplorerClass.Valkyrie:
                nameList = new string[]
                {
                        "Joan", "Mary", "Athena", "Eleanor", "Elizabeth"
                };
                break;
        }
        string name = "";
        int i = 1;
        while(counter < 5 * i)
        {
            i++;
        }
        name = nameList[UnityEngine.Random.Range(0, nameList.Length)];
        for(int rep = 1; rep < i; rep++)
        {
            name += " " + nameList[UnityEngine.Random.Range(0, nameList.Length)];
        }
        if (roster.ContainsKey(name) || recruits.ContainsKey(name))
        {
            return getRandomName(eClass, counter: counter + 1);
        }
        else
        {
            return name;
        }
    }
}
