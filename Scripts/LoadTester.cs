using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTester : MonoBehaviour
{
    public bool loadTest;
    public bool combatTest;
    public CombatOrganizer combatOrganizer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (loadTest)
        {
            loadTest = false;
            StaticInformationHolder.createPartyGameObject();
        }
        if (combatTest)
        {
            combatTest = false;
            Party enemyParty = GameObject.Find("Test Enemies").GetComponent<Party>();
            combatOrganizer.enemyParty = enemyParty;
            combatOrganizer.turn = true;
        }
    }
}
