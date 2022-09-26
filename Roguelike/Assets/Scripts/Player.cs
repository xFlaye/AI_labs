using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//! For the new Input System
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    // General components
    public int curHp;
    public int maxHp;
    public int coins;

    public bool hasKey;

    public SpriteRenderer sr; // to flash when Player gets hit

    public LayerMask moveLayerMask;


    //! Mover function that drives the movement of the Input Actions below
    void Move (Vector2 dir)
    {
        //! fire a raycast to check what's in the direction the Player is moving
        // the moveLayerMask will check if it's blocked by wall or NPC; check lesson #6
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, moveLayerMask); // transform.position refers to where the Player currently is at; layermask specifies the exact layer

        // check if Player hits something
        if(hit.collider == null) // hits nothing
        {
            //transform.position += dir; // <----- gives error because transform.position requires a Vector3. Use the following instead....
            transform.position += new Vector3(dir.x, dir.y, 0);
        }
    }

    
    //! these functions are connected to the Palyer Input component attached to the Player prefab.
    //! They are invoking Unity Events
    public void onMoveUp(InputAction.CallbackContext context)
    {
        // the phase option is the phase of the click (held down, released, etc.) <=== related to the Input Actions. Read docs
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.up);
        }
    }

    public void onMoveDown(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.down);
        }
    }

    public void onMoveLeft(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.left);
        }
    }

    public void onMoveRight(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            Move(Vector2.right);
        }                    
    }

    public void onAttackUp(InputAction.CallbackContext context)
    {
        
    }

    public void onAttackDown(InputAction.CallbackContext context)
    {
        
    }

    public void onAttackLeft(InputAction.CallbackContext context)
    {
        
    }

    public void onAttackRight(InputAction.CallbackContext context)
    {
        
    }
}
