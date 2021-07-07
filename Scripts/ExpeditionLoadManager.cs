using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionLoadManager : MonoBehaviour
{
    public MovementControl moveControl;
    public LevelBackgroundLoader backgroundLoader;
    public CombatOrganizer combatOrganizer;
    public bool loadingFinished = false;
    public int loadingState = 0;

    // Start is called before the first frame update
    async void Start()
    {
        moveControl.canMove = false;
        await backgroundLoader.createBackground(5);
    }

    // Update is called once per frame
    void Update()
    {
        if (!loadingFinished)
        {
            switch (loadingState) {
                case 0:
                    if (backgroundLoader.created)
                    {
                        StaticInformationHolder.createPartyGameObject();
                        loadingState++;
                    }
                    break;
                case 1:
                    if(combatOrganizer.playerParty != null)
                    {
                        loadingState++;
                    }
                    break;
                default:
                    loadingFinished = true;
                    break;
            }
        }
        else
        {
            moveControl.canMove = true;
            Destroy(gameObject);
        }
    }
}
