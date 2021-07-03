using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionLoadManager : MonoBehaviour
{
    public MovementControl moveControl;
    public LevelBackgroundLoader backgroundLoader;
    public bool loadingFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        moveControl.canMove = false;
        backgroundLoader.createBackground(10);
    }

    // Update is called once per frame
    void Update()
    {
        if (!loadingFinished)
        {
            if (backgroundLoader.created)
            {
                loadingFinished = true;
            }
        }
        else
        {
            moveControl.canMove = true;
            Destroy(gameObject);
        }
    }
}
