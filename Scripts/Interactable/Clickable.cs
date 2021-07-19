using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Clickable : MonoBehaviour
{
    public bool hovered;
    public float zoomSpeed = 10;
    public Vector3 defaultScale;

    public abstract void onInteract();

    public void OnMouseOver()
    {
        hovered = true;
    }

    public void OnMouseExit()
    {
        hovered = false;
    }

    public void OnMouseDown()
    {
        onInteract();
    }

    public void Start()
    {
        defaultScale = gameObject.transform.localScale;
        gameObject.AddComponent<PolygonCollider2D>();
    }

    public void Update()
    {
        Vector3 targetScale = defaultScale;

        if (hovered)
        {
            targetScale = defaultScale * 1.2f;  
        }

        Vector3 diff = targetScale - gameObject.transform.localScale;
        gameObject.transform.localScale += diff * zoomSpeed * Time.deltaTime;
    }
}
