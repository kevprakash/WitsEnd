using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class LevelBackgroundLoader : MonoBehaviour
{
    public Texture2D[] backgroundLayers;
    public float[] backgroundLoopOffset;
    public float[] initOffset;
    public GameObject emptyReference;
    public GameObject spriteRenderer;
    public float[] layerYOffset;
    public float[] layerMinY;
    public float[] layerMaxY;
    public float scaling;
    public float[] backgroundItemScaling;
    public bool created = false;
    public bool loading = false;
    public Animator loadingAnimator;

    public bool test;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            test = false;
            //createBackground(10);
        }
    }

    public async Task createBackground(int length)
    {
        loading = true;
        MovementControl MC = GameObject.Find("Movement Control").GetComponent<MovementControl>();
        GameObject[] controlRefs = new GameObject[backgroundLayers.Length];
        controlRefs[0] = Instantiate(emptyReference, new Vector3(0, 0, (backgroundLayers.Length + 1) * 5), new Quaternion());
        await createLayer(backgroundLayers[0], length, 0, controlRefs[0], false, (backgroundLayers.Length + 1) * 5);
        for (int i = 1; i < backgroundLayers.Length; i++)
        {
            controlRefs[i] = Instantiate(emptyReference, new Vector3(0, 0, i * 5), new Quaternion());
            await createLayer(backgroundLayers[i], length, i, controlRefs[i], i != backgroundLayers.Length - 1);
        }
        MC.backgroundRefs = controlRefs;
        loading = false;
        created = true;
        loadingAnimator.SetTrigger("Close Loading");
    }

    public async Task createLayer(Texture2D texture, int length, int slot, GameObject controller, bool populate, float zOverride=-1)
    {
        GameObject[] SOs = new GameObject[length];
        for (int i = 0; i < length; i++) 
        {
            GameObject spriteObj = Instantiate(spriteRenderer, new Vector3(backgroundLoopOffset[slot] * i * scaling + initOffset[slot], layerYOffset[slot] * scaling, zOverride > 0 ? slot * 5 : zOverride), new Quaternion());
            SpriteRenderer sprite = spriteObj.GetComponent<SpriteRenderer>();
            Texture2D tex = backgroundLayers[slot];
            Rect r = new Rect(0, 0, tex.width, tex.height);
            sprite.sprite = Sprite.Create(tex, r, new Vector2(0, 0));
            spriteObj.transform.SetParent(controller.transform);
            spriteObj.transform.localPosition = new Vector3(spriteObj.transform.localPosition.x, spriteObj.transform.localPosition.y, 0);

            float minX = backgroundLoopOffset[slot] * i * scaling + initOffset[slot];
            float maxX = backgroundLoopOffset[slot] * i * scaling +  initOffset[slot] + backgroundLoopOffset[slot];

            if(populate)
                await populateLayer(spriteObj, minX, maxX, layerMinY[slot], layerMaxY[slot], (int)(2.5/backgroundItemScaling[slot]), slot);

            SOs[i] = spriteObj;
            
            spriteObj.transform.localScale = new Vector3(scaling, scaling, 1);
        }

        //foreach(GameObject g in SOs)
        //{
            //g.transform.localScale = new Vector3(scaling, scaling, 1);
        //}
    }

    public async Task populateLayer(GameObject layer, float minX, float maxX, float minY, float maxY, int numItems, int slot)
    {
        for(int i = 0; i < numItems; i++)
        {
            string spriteFileName = "";
            if(UnityEngine.Random.value > 0.5f)
            {
                spriteFileName = "Bush" + UnityEngine.Random.Range(1, 8);
            }
            else
            {
                spriteFileName = "Tree" + UnityEngine.Random.Range(1, 13);
            }

            float x = UnityEngine.Random.Range(minX, maxX);
            float y = UnityEngine.Random.Range(minY, maxY);
            float z = (y - minY) / (maxY - minY) * 4 + layer.transform.position.z - 5;

            await loadBackgroundObject(spriteFileName, layer, new Vector3(x, y, z), slot, i % 2 == 1, (1 - (y - minY) / (maxY - minY)) * -4);
        }
    }

    public async Task loadBackgroundObject(string spriteFileName, GameObject layer, Vector3 position, int slot, bool shouldSkipFrame, float zOffset)
    {
        GameObject spriteObj = Instantiate(spriteRenderer, position, new Quaternion());
        SpriteRenderer sprite = spriteObj.GetComponent<SpriteRenderer>();
        Texture2D tex = Resources.Load<Texture2D>(spriteFileName);
        Rect r = new Rect(0, 0, tex.width, tex.height);
        sprite.sprite = Sprite.Create(tex, r, new Vector2(0, 0));
        spriteObj.transform.SetParent(layer.transform);
        spriteObj.transform.localScale = new Vector3(backgroundItemScaling[slot], backgroundItemScaling[slot], 1);

        spriteObj.transform.localPosition = new Vector3(spriteObj.transform.localPosition.x, spriteObj.transform.localPosition.y, zOffset);

        if (shouldSkipFrame)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1)); ;
        }
    }
}
