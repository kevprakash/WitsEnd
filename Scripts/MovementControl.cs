using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{

    public Camera mainCam;
    public float moveSpeed = 1f;
    public GameObject[] backgroundRefs;
    public float parallaxSpeedFactor = 2f;
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            mainCam.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * Input.GetAxis("Move Right"));
            for (int i = 1; i < backgroundRefs.Length; i++)
            {
                int parallaxExp = i - (backgroundRefs.Length);
                backgroundRefs[i].transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * Input.GetAxis("Move Right") * Mathf.Pow(parallaxSpeedFactor, parallaxExp));
            }
        }
    }
}
