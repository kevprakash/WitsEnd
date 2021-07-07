using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class Party : MonoBehaviour
{
    public Character[] order = new Character[4];
    public Character[] defaultOrder = null;
    public Vector3[] targetLocations = new Vector3[]
    {
        new Vector3(-1.5f, -2, 0),
        new Vector3(-3.35f, -2, 0),
        new Vector3(-5.2f, -2, 0),
        new Vector3(-7.05f, -2, 0)
    };
    public float movementSpeed = 10f;
    public Vector3[] focusLocations = new Vector3[]
    {
        new Vector3(-1.5f, -3, -1),
        new Vector3(-3.6f, -3, -1),
        new Vector3(-5.7f, -3, -1),
        new Vector3(-8.05f, -3, -1)
    };
    public Vector3 defaultScale = new Vector3(1, 1, 1);
    public Vector3 focusScale = new Vector3(1.5f, 1.5f, 1.5f);
    public float zoomSpeed = 10f;
    public bool[] inFocus = new bool[4];

    public Party enemies = null;

    public bool movementTest = false;
    public bool zoomTest = false;
    public bool dualTest = false;
    public bool defaultTest = false;

    public bool ownedByPlayer = false;
    public bool flipSprites = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Character c in order)
        {
            if (c != null) { 
                c.setParty(this);
                c.gameObject.GetComponent<SpriteRenderer>().flipX = flipSprites;
            }
        }
    }

    public void setFlip()
    {
        foreach (Character c in order)
        {
            if (c != null)
            {
                c.gameObject.GetComponent<SpriteRenderer>().flipX = flipSprites;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(defaultOrder == null || defaultOrder.Length == 0)
        {
            defaultOrder = new Character[order.Length];
            for(int i = 0; i < defaultOrder.Length; i++)
            {
                defaultOrder[i] = order[i];
            }
        }

        for(int i = 0; i < order.Length; i++)
        {
            if(order[i] == null) continue;

            Transform t = order[i].gameObject.transform;
            Vector3 target = inFocus[i] ? focusLocations[i] : targetLocations[i];

            if(Vector3.Distance(t.position, target) > 0.001f)
            {
                Vector3 difference =  target + transform.position - t.position;
                t.position += difference * Time.deltaTime * (inFocus[i] ? zoomSpeed : movementSpeed);
            }

            Vector3 targetScale = inFocus[i] ? focusScale : defaultScale;

            if(Vector3.Distance(t.localScale, targetScale) > 0.001f)
            {
                Vector3 difference = targetScale - t.localScale;
                t.localScale += difference * Time.deltaTime * zoomSpeed;
            }

            if(order[i].GetComponent<SpriteRenderer>().sprite.texture != order[i].targetSprite && order[i].targetSprite != null)
            {
                Texture2D tex = order[i].targetSprite;
                Sprite s = order[i].GetComponent<SpriteRenderer>().sprite;
                Rect r = new Rect(0, 0, tex.width, tex.height);
                order[i].GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, r, new Vector2(0.5f, 0f), 750);
            }
        }

        if (movementTest)
        {
            movementTest = false;
            move(Random.Range(0, 4), Random.Range(0, 4));
        }
        if (zoomTest)
        {
            zoomTest = false;
            int testLength = Random.Range(1, 5);
            int[] testIndices = new int[testLength];
            for(int i = 0; i < testIndices.Length; i++)
            {
                testIndices[i] = Random.Range(0, 4);
            }
            focus(testIndices);
        }
        if (dualTest)
        {
            dualTest = false;
            int from = Random.Range(0, 4);
            int to = Random.Range(0, 4);
            while(to == from)
            {
                to = Random.Range(0, 4);
            }
            moveAndFocus(from, to, new int[] { to });
        }
        if (defaultTest)
        {
            defaultTest = false;
            goToDefaultOrder();
        }
        
    }

    public bool hasCharacter(Character c)
    {
        return order.Contains(c);
    }

    public void move(int from, int to)
    {
        Character c = order[from];
        int direction = from < to ? 1 : -1;
        for(int i = from; i != to; i += direction)
        {
            order[i] = order[i + direction];
        }
        order[to] = c;
    }

    public async void focus(int[] positions, float duration=1.5f)
    {
        foreach(int i in positions)
        {
            //Debug.Log(i);
            inFocus[i] = true;
            if (order[i] != null)
            {
                order[i].setFocused(true);
            }
        }
        await Task.Delay((int)(duration * 1000));
        inFocus = new bool[4];
        foreach(Character c in order)
        {
            if (c != null)
            {
                c.setFocused(false);
                c.targetSprite = c.idleSprite;
            }
        }
    }

    public void moveAndFocus(int from, int to, int[] positions, float duration = 1f)
    {
        move(from, to);
        focus(positions, duration: duration);
    }

    public void addCharacter(Character c)
    {
        if (getCharacterCount() < order.Length) {
            order[getCharacterCount()] = c;
            c.setParty(this);
        }
    }

    public int getPositionOfCharacter(Character c)
    {
        //Debug.Log(c);
        int i = 0;
        for (; i < order.Length && order[i] != c; i++) ;
        return i == order.Length ? -1 : i;
    }

    public int getDefaultPositionOfCharacter(Character c)
    {
        int i = 0;
        for (; defaultOrder[i] != c && i < defaultOrder.Length; i++) ;
        return i == defaultOrder.Length ? -1 : i;
    }

    public void removeCharacter(int index)
    {
        move(index, order.Length - 1);
        Character c = order[order.Length - 1];
        order[order.Length - 1] = null;
        c.setParty(null);
    }

    public void removeCharacter(Character c)
    {
        removeCharacter(getPositionOfCharacter(c));
    }

    public int getCharacterCount()
    {
        int i = 0;
        for (; i < order.Length && order[i] != null; i++);
        return i;
    }

    public void goToDefaultOrder()
    {
        Character[] temp = new Character[order.Length];
        Queue<Character> newCharacters = new Queue<Character>();
        foreach(Character c in order)
        {
            if (c == null) continue;
            int p = getDefaultPositionOfCharacter(c);
            if(p == -1)
            {
                newCharacters.Enqueue(c);
            }
            else
            {
                temp[p] = c;
            }
        }
        for(int i = 0; i < temp.Length && newCharacters.Count > 0; i++)
        {
            if (temp[i] != null) continue;
            temp[i] = newCharacters.Dequeue();
        }
        order = temp;
        defaultOrder = new Character[order.Length];
        for (int i = 0; i < defaultOrder.Length; i++)
        {
            defaultOrder[i] = order[i];
        }
    }

    public List<int> getPartyData()
    {
        List<int> data = new List<int>();
        int dataLength = -1;
        if (allDead())
        {
            data.AddRange(new int[400]);    //Default value
            return data;
        }
        for(int i = 0; i < order.Length; i++)
        {
            if(order[i] != null)
            {
                List<int> charData = order[i].getData();
                data.AddRange(charData);
                if(dataLength < 0)
                {
                    dataLength = charData.Count;
                }
            }
            else
            {
                data.AddRange(new int[dataLength]);
            }
        }
        return data;
    }

    public void endTurn()
    {
        bool hasDivine = false;
        foreach(Character c in order)
        {
            hasDivine = c.getStatusEffect("divine").Item2 > 0;
            if (hasDivine) break;
        }

        foreach(Character c in order)
        {
            if(c is Explorer)
            {
                ((Explorer)c).modifySanity(-5);
            }
        }
    }

    public bool allDead()
    {
        return getCharacterCount() <= 0;
    }
}
