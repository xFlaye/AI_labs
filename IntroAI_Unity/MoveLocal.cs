using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLocal : MonoBehaviour
{
    public Transform goal;

    public float speed = 0.5f;
    public float accuracy = 1.0f;

    public float rotSpeed = 1.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // rotate the object towards the goal
        //! problem with this code is that it rotates the object so all 3 axis point towards goal
        //this.transform.LookAt(goal.position);

        // instead this applies only to X & Z axis, while keeping the object's Y axis level
        Vector3 lookAtGoal = new Vector3(goal.position.x,
                                         this.transform.position.y,
                                         goal.position.z);

        // figure out direction
        Vector3 direction = lookAtGoal - this.transform.position;
        //! without slerp
        //this.transform.LookAt(lookAtGoal);

        // todo Read up on Quaternion Class

        //! use Quaternion.Slerp to set the rotation
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                    Quaternion.LookRotation(direction),
                                                    Time.deltaTime * rotSpeed);

        

        // prevent the object from jittering when reaching goal
        // use Vector.Distance(VecA, VecB)
        if (Vector3.Distance(transform.position, lookAtGoal) > accuracy){
            // move the object in the direction along the Z-axis
            this.transform.Translate(0,0,speed * Time.deltaTime);

        }

        
        
    }
}
