using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public bool menuOpen = false;
    private bool letGo = true;
    public string prompt = "Are you sure you want to exit?";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Open Menu") > 0 && letGo)
        {
            if (menuOpen)
            {
                closeMenu();
            }
            else
            {
                PopupSystem ps = gameObject.GetComponent<PopupSystem>();
                ps.popup(prompt);
                menuOpen = true;
            }
            letGo = false;
        }
        else if(Input.GetAxis("Open Menu") <= 0)
        {
            letGo = true;
        }
    }

    public void exitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit();
    }

    public async void exitExpedition()
    {
        Debug.Log("Exiting Expedition");
        StaticInformationHolder.createdParty = false;
        Party playerParty = GameObject.Find("Player party").GetComponent<Party>();
        foreach(Character c in playerParty.order)
        {
            Explorer e = (Explorer)c;
            await e.writeToCSV();
        }
        SceneManager.LoadScene("Hub");
    }

    public async void closeMenu()
    {
        PopupSystem ps = gameObject.GetComponent<PopupSystem>();
        await ps.close();
        menuOpen = false;
    }
}
