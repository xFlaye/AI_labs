using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    //! bool sets checkbox; true = checked
    public bool isAlive = false;
    public int numNeighbors = 0;

    public static float _updRadius;

    void Awake() {
        _updRadius = GetComponent<MeshRenderer>().bounds.size.x;        
    }

    

    //! enables the positioning of the cell sprite if the alive param is enabled
    // use the boolean to toggle
    public void SetAlive(bool alive)
    {

        //! This method apparently works better because it loads all the cells at once, and simply turns on/off their state
        //! instead of destroying GameObjects all the time.
        
        isAlive = alive;
        //todo see how to recreate this with code instead of graphic
        if(alive)
        // enable spriteRenderer
        {
            // GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
        }
        else // disable spriteRenderer
        {
            // GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
