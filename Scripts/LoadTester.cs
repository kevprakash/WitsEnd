using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTester : MonoBehaviour
{
    public bool loadTest;

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
    }
}
