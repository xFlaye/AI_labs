using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // reference the Player
    public Player player;

    public int health;
    public int damage;

    // when enemy is defeated, drop something
    public GameObject deathDropPrefab;
    public SpriteRenderer sr; // reference the enemies sprite renderer to add damage effect

    public LayerMask moveLayerMask;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // the EnemyManager class controls the movement of all enemies

    public void Move()
    {
        // randomly move
        if (Random.value < 0.5f) // %50 chance it won't move, so return
        {
            return;
        }
        // otherwise
        Vector3 dir = Vector3.zero; // easier to work in V3 rather than convert later from V2 to V3

        bool canMove = false;

        // check which direction the enemy can move

        while(canMove == false)
        {
            // first direction to check
            dir = GetRandomDirection();

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.0f, moveLayerMask);

            //! check if enemy hits an object. If nothing 
            if(hit.collider == null)
            {
                canMove = true;
            }
        }
        // move to next direction
        transform.position += dir;
    }

    Vector3 GetRandomDirection()
    {
        int ran = Random.Range(0, 4); // total maximum - 1

        if (ran == 0)
        {
            return Vector3.up;
        }
        else if(ran == 1)
        {
            return Vector3.down;
        }
        else if (ran == 2)
        {
            return Vector3.right;
        }
        else if (ran == 3)
        {
            return Vector3.left;
        }

        // error check if none
        return Vector3.zero;
    }

}
