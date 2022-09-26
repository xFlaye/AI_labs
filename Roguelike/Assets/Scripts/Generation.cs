using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    // how large is the map going to be (number of rooms width and height)
    public int mapWidth = 7;
    public int mapHeight = 7;
    public int roomsToGenerate = 12; // rooms to generate every round

    private int roomCount;
    private bool roomInstantiated; // keep tracks of room instatiated

    // position of the first room
    private Vector2 firstRoomPos; // create the branching from this point

    // map of rooms using a 2D boolean array to check if the room exists

    private bool[,] map;

    // keep track of the room instatiated
    public GameObject roomPrefab;

    private List<Room> roomObject = new List<Room>();

    //! create a Singleton to access it at any point in the project. The Singleton is named after the 
    //! class above
    public static Generation instance;

    void Awake() 
    {
        instance = this;
    }

    void Start() 
    {
        Random.InitState(765); // establish starting seed generation number
        Generate();
    }

    public void Generate()
    {
        // start the process of creating the level at the start of the game

        map = new bool[mapWidth, mapHeight];
        //! call the checkroom function to place the first room (center of screen)
        CheckRoom((mapWidth/2), (mapHeight/2), 0, Vector2.zero, true);

        // generate the following rooms
        InstantiateRooms();

        Debug.LogFormat("first room pos: x {0}, y{1}", firstRoomPos.x, firstRoomPos.y);

        Vector2 newPos = firstRoomPos*12;

        //! find player in the scene and position Player in first room (the 12 is the width of the room prefab...although it's a total of 13 sprites?)
        FindObjectOfType<Player>().transform.position = firstRoomPos * 12;

        Debug.LogFormat("first room pos: x {0}, y{1}", newPos.x, newPos.y);

    }

    void CheckRoom (int x, int y, int remaining, Vector2 generalDirection, bool firstRoom = false) 
    {
        // create a branching algorithm to set the general generation direction and checks if room can be placed there

        //! set return conditions to initialize room checks
        // check if we reached the maximum number of rooms to generate. If yes, return
        if (roomCount >= roomsToGenerate)
        {
            return;
        }

        // if room is outside bounds of the map, then stop
        if (x < 0 || x > mapWidth -1 || y < 0 || y > mapHeight -1)
        {
            return;
        }

        // if remaining is <= 0, then return, except for first room
        if (firstRoom == false && remaining <= 0)
        {
            return;
        }

        // does this room already exist?
        if (map[x,y] == true)
        {
            return;
        }

        //! set position of first room
        if (firstRoom == true)
        {
            firstRoomPos = new Vector2(x,y);
        }
        // add 1 to the roomcount
        roomCount++;
        // set the room as existing on the map
        map[x,y] = true;

        //! Which direction are we branching towards?

        bool north = Random.value > (generalDirection == Vector2.up ? 0.2f: 0.8);
        bool south = Random.value > (generalDirection == Vector2.down ? 0.2f: 0.8);
        bool east = Random.value > (generalDirection == Vector2.right ? 0.2f: 0.8);
        bool west = Random.value > (generalDirection == Vector2.left ? 0.2f: 0.8);

        int maxRemaining = roomsToGenerate / 4;

        //! algorithm that generates the rooms with checks if it's the first room or not (cellular automata like)
        if (north || firstRoom)
        {
            CheckRoom(x, y + 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.up : generalDirection);
        }
        if (south || firstRoom)
        {
            CheckRoom(x, y - 1, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.down : generalDirection);
        }
        if (east || firstRoom)
        {
            CheckRoom(x + 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.right : generalDirection);
        }
        if (west || firstRoom)
        {
            CheckRoom(x - 1, y, firstRoom ? maxRemaining : remaining - 1, firstRoom ? Vector2.left : generalDirection);
        }
        

    }

    void InstantiateRooms()
    {
        // once the rooms have been decided, they will be spawned in and setup
        //! check if rooms have already been instantiated. If yes, return
        if (roomInstantiated) { return; }

        // initialize variable
        roomInstantiated = true;

        // loop through each element in the map[] array
        //! notice the end of the for-loop --> ++x instead of x++
        /*
            i++ means 'tell me the value of i, then increment'

            ++i means 'increment i, then tell me the value'
        */

        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                if(map[x, y] == false)
                {
                    continue; //! continue to the next loop
                }
                // otherwise, if it's true, instantiate another room prefab
                GameObject roomObj = Instantiate(roomPrefab, new Vector3(x, y, 0) * 12 , Quaternion.identity);

                // access the room script on the above object
                Room room = roomObj.GetComponent<Room>();

                // if room above, enable north gate, if there's no room, enable wall
                // if we're within the boundary of the map, AND if there is room above us
                if (y < mapHeight - 1 && map[x, y+1] ==  true)
                {
                    room.northDoor.gameObject.SetActive(true);
                    room.nortWall.gameObject.SetActive(false);
                }
                // if hugging bottom layout
                // if we're within the boundary of the map, AND if there is room below us
                if (y > 0 && map[x, y - 1] == true)
                {
                    room.southDoor.gameObject.SetActive(true);
                    room.southWall.gameObject.SetActive(false);
                }
                // if we're within the boundary of the map, AND if there is room to the right of us
                if(x < mapWidth - 1 && map[x + 1, y] == true)
                {
                    // enable the east door and disable the east wall.
                    room.eastDoor.gameObject.SetActive(true);
                    room.eastWall.gameObject.SetActive(false);
                }
                // if we're within the boundary of the map, AND if there is room to the left of us
                if(x > 0 && map[x - 1, y] == true)
                {
                    // enable the west door and disable the west wall.
                    room.westDoor.gameObject.SetActive(true);
                    room.westWall.gameObject.SetActive(false);
                }

                // Generate room's interior, unless it's the first room (so there are no enemies)
                if (firstRoomPos != new Vector2(x,y))
                {
                    room.GenerateInterior();
                }
                // add the room to the list
                roomObject.Add(room);
            }       
        }

        CalculateKeyAndExit();

    }

    void CalculateKeyAndExit()
    {
        // Ensure that key and exit are a far distance away from each other in the game
    }



}
