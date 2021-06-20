﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityBar : MonoBehaviour
{

    public Explorer explorer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = new Vector3(Mathf.Clamp(explorer.getSanity(), 0, 100)/100.0f, 1, 1);
        transform.localScale = newScale;
    }
}
