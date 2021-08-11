using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;

public class PopupSystem : MonoBehaviour
{

    public GameObject popupBox;
    public Animator animator;
    public TMP_Text popUpText;

    public void Start()
    {
        popupBox.SetActive(false);
    }

    public void popup(string text)
    {
        popupBox.SetActive(true);
        popUpText.text = text;
        animator.SetTrigger("pop");
    }

    public async Task close()
    {
        animator.SetTrigger("close");
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        popupBox.SetActive(false);
    }
}
