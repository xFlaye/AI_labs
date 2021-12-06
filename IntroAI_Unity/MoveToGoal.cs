using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGoal : MonoBehaviour
{
    
    public float speed = 2.0f;
    public float accuracy = 0.01f;
    public Transform goal; // asks for an object and extracts x,y,z transform position from it
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // LookAt() is a function that focuses object to another one
        // by placing it here rather that in the start, it will always update the rotation whenever the cube is positioned
        this.transform.LookAt(goal.position);

        // rotate towards goal        
        Vector3 direction = goal.position - this.transform.position;

        //debug vector lines
        Debug.DrawRay(this.transform.position,direction,Color.red);

        if(direction.magnitude > accuracy ){
            // leaving it like below, will make the objects spiral until the reach the goal because it's in localSpace
            // ! this.transform.Translate(direction.normalized * speed * Time.deltaTime);

            // instead, make the direction look at worldSpace coordinates instead of localSpace
            this.transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
        
    }
}
