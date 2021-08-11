using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;

public class StaticInformationHolder : MonoBehaviour
{
    public enum ExplorerClass
    {
        Medic, Ranger, Sniper, Valkyrie
    }

    public static Dictionary<string, (ExplorerClass, int)> roster = new Dictionary<string, (ExplorerClass, int)>();
    public static Dictionary<string, (ExplorerClass, int)> recruits = new Dictionary<string, (ExplorerClass, int)>();
    public static (ExplorerClass, int, string)[] party = new (ExplorerClass, int, string)[4];
    public static bool creatingParty = false;
    public static bool createdParty = false;
    public static int expeditionLength = 20;

    static StaticInformationHolder()
    {}


    public static void addToParty(string name, int index)
    {
        (ExplorerClass, int) exp = roster[name];
        party[index] = (exp.Item1, exp.Item2, name);
    }

    public static async void createPartyGameObject()
    {
        creatingParty = true;
        createdParty = false;

        GameObject partyObj = Instantiate(Resources.Load<GameObject>("Empty"));
        Party partyComp = partyObj.AddComponent(typeof(Party)) as Party;
        partyObj.name = "Player party";
        partyObj.transform.SetParent(GameObject.Find("Main Camera").transform);
        partyObj.transform.localPosition = new Vector3(0, 0, 2);
        partyComp.ownedByPlayer = true;

        await initializePartyInfo(partyComp);

        creatingParty = false;
        createdParty = true;
    }

    public static async Task initializePartyInfo(Party partyRef)
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
            await eComp.readFromCSV();
            await eComp.train();
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
        writeParty();
        bool shift = false;
        for(int i = 0; i < party.Length; i++)
        {
            if (shift)
            {
                party[i - 1] = party[i];
                party[i] = (ExplorerClass.Valkyrie, 0, null);
            }
            if(party[i].Item3 == name)
            {
                shift = true;
            }
        }
        if (shift)
        {
            writeParty();
        }
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

    public static string getFilePath(string fileName)
    {
        string path = Application.persistentDataPath + "/Data/" + fileName;
        if (!File.Exists(path))
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Data/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Data/");
            }
            File.Create(path).Dispose();
            switch (fileName)
            {
                case "Roster.csv":
                    roster = new Dictionary<string, (ExplorerClass, int)>();
                    roster["Tracy"] = (ExplorerClass.Ranger, 0);
                    roster["Anastacia"] = (ExplorerClass.Valkyrie, 0);
                    roster["Elizabeth"] = (ExplorerClass.Valkyrie, 0);
                    roster["Sarah"] = (ExplorerClass.Ranger, 0);
                    writeRoster();
                    break;
                case "Party.csv":
                    addToParty("Anastacia", 0);
                    addToParty("Tracy", 1);
                    addToParty("Elizabeth", 2);
                    addToParty("Sarah", 3);
                    writeParty();
                    break;
                default:
                    break;
            }
        }
        return path;
    }

    public static void readRoster()
    {
        roster = new Dictionary<string, (ExplorerClass, int)>();
        string path = getFilePath("Roster.csv");
        StreamReader reader = new StreamReader(path);
        string line = "";
        while ((line = reader.ReadLine()) != null)
        {
            string[] lineSplit = line.Split(',');
            roster[lineSplit[0]] = ((ExplorerClass) Enum.Parse(typeof(ExplorerClass), lineSplit[1]), int.Parse(lineSplit[2]));
        }
        reader.Close();
    }

    public static void writeRoster()
    {
        File.WriteAllText(getFilePath("Roster.csv"), string.Empty);
        string path = getFilePath("Roster.csv");
        StreamWriter writer = new StreamWriter(path, true);
        foreach(string eName in roster.Keys)
        {
            (ExplorerClass, int) explorerData = roster[eName];
            string line = eName + "," + explorerData.Item1 + "," + explorerData.Item2;
            writer.WriteLine(line);
        }
        writer.Close();
    }

    public static void readParty()
    {
        string path = getFilePath("Party.csv");
        StreamReader reader = new StreamReader(path);
        string line = reader.ReadLine();
        int index = 0;
        if (line != null)
        {
            foreach (string eName in line.Split(','))
            {
                addToParty(eName, index);
                index++;
                if (index >= 4)
                {
                    break;
                }
            }
        }
        reader.Close();
    }

    public static void writeParty()
    {
        File.WriteAllText(getFilePath("Party.csv"), string.Empty);
        string path = getFilePath("Party.csv");
        StreamWriter writer = new StreamWriter(path, true);
        string line = party[0].Item3;
        for(int i = 1; i < party.Length; i++)
        {
            line = line + "," + party[i].Item3;
        }
        writer.WriteLine(line);
        writer.Close();
    }
}
