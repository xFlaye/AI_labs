using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // Create variables to store the various wall objects
    [Header("Door Objects")]
    public Transform northDoor;
    public Transform southDoor;
    public Transform eastDoor;
    public Transform westDoor;

    [Header("Wall Objects")]
    public Transform nortWall;
    public Transform southWall;
    public Transform eastWall;
    public Transform westWall;

    [Header("Values")]
    // size of room
    public int insideWidth;
    public int insideHeight;

    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public GameObject healthPrefab;
    public GameObject keyPrefab;
    public GameObject exitDoorPrefab;

    // create a list to keep track of what spawns where
    private List<Vector3> usedPositions = new List<Vector3>();

    public void GenerateInterior()
    {
        //! function that creates content in room
    }

    public void SpawnPrefab(GameObject prefab, int min = 0, int max = 0)
    {
        // specifies wich object to spawn, including min & max. By setting them to 0, it error proofs if nothing is fed.
    }
}
