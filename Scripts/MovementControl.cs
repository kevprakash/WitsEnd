using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{

    public Camera mainCam;
    public float moveSpeed = 1f;
    public GameObject[] backgroundRefs;
    public float parallaxSpeedFactor = 2f;
    public float nextCombatDistance;
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        nextCombatDistance = Random.Range(1.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (mainCam.transform.position.x + Time.deltaTime * moveSpeed * Input.GetAxis("Move Right") > 0)
            {
                mainCam.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * Input.GetAxis("Move Right"));
                for (int i = 1; i < backgroundRefs.Length; i++)
                {
                    int parallaxExp = i - (backgroundRefs.Length);
                    backgroundRefs[i].transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * Input.GetAxis("Move Right") * Mathf.Pow(parallaxSpeedFactor, parallaxExp));
                }
                nextCombatDistance -= Time.deltaTime * moveSpeed * Input.GetAxis("Move Right");
                if(nextCombatDistance <= 0)
                {
                    GameObject.Find("Combat Organizer").GetComponent<CombatOrganizer>().spawnEnemies(new string[] { "Slug", "Slug", "Slug", "Slug" }, new int[] {0, 0, 0, 0});
                    nextCombatDistance = Random.Range(10.0f, 20.0f);
                }
            }
        }
    }
}
