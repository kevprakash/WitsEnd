using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Campfire : Clickable
{

    public override void onInteract()
    {
        Debug.Log("Campfire clicked on");
        loadExpedition();
    }

    public void loadExpedition()
    {
        SceneManager.LoadScene("Expedition");
    }
}
