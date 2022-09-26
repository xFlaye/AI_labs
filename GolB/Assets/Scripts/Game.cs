using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//! to output files in XML
using System.IO;
using System.Xml.Serialization;

public class Game : MonoBehaviour
{
    //! change this to a dynamic setup using camRatio 
    
    private static int SCREEN_HEIGHT;
    private static int SCREEN_WIDTH;

    // Set timer
    public float speed = 0.1f; // define speed variable. Higher value means slower
    private float timer = 0;

    public Cell cellObject;

    public int startType;

    public bool simulationEnabled = false;

    //! send references to HexGlobals static class
    public static float _hexWidth;
    public static float _hexHeight;


    //! call the Cell class and have it hold the screen dimensions
    // Cell[,] grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];
    Cell[,] grid;

    RaycastHit hitInfo;

  

    void Awake() 
    {
        //! establish the radius of the hexes based on the size of the object
        //! and pass that to the HexGlobals.
        _hexWidth = cellObject.GetComponent<MeshRenderer>().bounds.size.x;
        _hexHeight = cellObject.GetComponent<MeshRenderer>().bounds.size.y;

        SCREEN_HEIGHT = (int)Camera.main.orthographicSize;
        SCREEN_WIDTH =   (int)SCREEN_HEIGHT * Screen.width / Screen.height; // basically SCREEN_HEIGTH * aspect ratio   

                
    }
    // Start is called before the first frame update
    void Start()
    {
        //! DYNAMIC CAMERA SETUP       

        // Screen Width
        float properHexWidth = SCREEN_WIDTH / HexGlobals.HalfWidth;
        SCREEN_WIDTH = Mathf.CeilToInt(properHexWidth);
        
        // Screen Height
        float properHexHeight = (SCREEN_HEIGHT * HexGlobals.Height) / HexGlobals.RowHeight;       
        SCREEN_HEIGHT = (int)properHexHeight;
     
        Debug.LogFormat("width {0} , height {1}, proper hexWidth {2}" , SCREEN_WIDTH, SCREEN_HEIGHT, properHexWidth);
        
        
        // Step 1
        grid = new Cell[SCREEN_WIDTH, SCREEN_HEIGHT];   

        // PlaceCells(startType);
        
        for (int x=0; x < SCREEN_WIDTH; x++)
                    {
                        for (int y=0; y < SCREEN_HEIGHT; y++)
                        {
                            CreateCells(startType, x, y);
                        }
                    }

    }

   
    // Update is called once per frame
    void Update()
    {
        if (simulationEnabled)
        {
             // check the timer every frame
            if (timer >= speed)
            {
                timer = 0f;
                // Step 2
                CountNeighbors();
                // Step 3
                PopulationControl();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        //! check UserInput for state of board
        UserInput();      
        
    }

    // doesn't get accessed outside the class
    //! ===========>        SAVE PATTERN =======================================================================
    private void SavePattern()
    {
        string path = "patterns"; // create directory in root of project
        // Check if directory already exists
        if (!Directory.Exists(path))
        {
            // create directory
            Directory.CreateDirectory(path);
        }

        // instantiate pattern class to serialize
        Pattern pattern = new Pattern();

        string patternString = null;

        // iterate over cells
        for (int y=0; y < SCREEN_HEIGHT; y++)
        {
            for (int x=0; x < SCREEN_WIDTH; x++)
            {
                if (grid[x,y].isAlive == false)
                {
                    // append a 0 to the string when cell is dead
                    patternString +="0";
                }
                else
                {
                    patternString += "1";
                }
            }    
        }

        pattern.patternString = patternString;

        // initialize XML and write to test.xml
        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        StreamWriter writer = new StreamWriter(path + "/test.xml");
        serializer.Serialize(writer.BaseStream, pattern);

        writer.Close();

        Debug.Log(pattern.patternString);

    }

    //! ===========>        LOAD PATTERN =======================================================================
    private void LoadPattern()
    {
        string path = "patterns";

        // error check
        if (!Directory.Exists(path))
        {
            Debug.Log("Patterns directory doesn't exist!");
            return;
        }
        //! LOOK INTO THIS CODE FURTHER...
        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        path += "/test.xml";

        StreamReader reader = new StreamReader(path);
        Pattern pattern = (Pattern)serializer.Deserialize(reader.BaseStream);
        reader.Close();

        // iterate over string length according to screen width

        bool isAlive = false;
        // public bool IsAlive { get; private set; } <------------------    LOOK INTO THIS

        int x = 0, y = 0;

        foreach (char c in pattern.patternString)
        {
            if (c.ToString() == "1")
            {
                isAlive = true;
            }
            else
            {
                isAlive = false;
            }

            grid[x,y].SetAlive(isAlive);
            // increase x by 1, meaning move over each character in string
            x++;
            // calculate if it reached the screen width, if yes, reset to 0 and increase y by 1
            if ( x == SCREEN_WIDTH)
            {
                x = 0;
                y++;
            }
        }
    }

    //! ===========> Add user input =======================================================================
    void UserInput()
    {

        // Prevent outOfBounds errors
		try
        {
           
			//! Allow user to toggle between alive and dead cells with LMB
            if(Input.GetMouseButtonDown(0))
            {         
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // feed into physics system
                Physics.Raycast(ray,out hitInfo);
                
				// Select the prefab components of each hex and check the SetAlive function in Cell class
                GameObject cell = hitInfo.collider.gameObject;
                MeshRenderer mr = cell.GetComponent<MeshRenderer>();
                Cell hexAlive = cell.GetComponent<Cell>();

                //! this toggles the cell on the grid to the inverse of what it is. Elegant code
                // grid[x,y].SetAlive(!grid[x,y].isAlive);

                Debug.Log("Raycast hit: " + cell.name);
                if(hexAlive.isAlive==true)
                {
                    hexAlive.isAlive = false;
                    mr.enabled = false;

                }
                else
                {
                    hexAlive.isAlive = true;
                    mr.enabled = true;
 
                }
                    
                }
   
        }
        //! error catching if clicking outside of game board
        catch (NullReferenceException) 
        {
            Debug.Log("Out of bounds");
        }
        
        //! create a toggle using the spacebar to start/pause simulation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            simulationEnabled = !simulationEnabled;
        }

        // Save pattern
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePattern();
        }

        // Save pattern
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPattern();
        }



        //! clear screen
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearBoard();            
        }
        
    }

    void ClearBoard()
    {
         // iterate over cells
        for (int y=0; y < SCREEN_HEIGHT; y++)
        {
            for (int x=0; x < SCREEN_WIDTH; x++)
            {
                grid[x,y].SetAlive(false);
            }
        }
    }

    //! STEP 1: iterate over area and place cells randomly, either alive or dead <===========
    void PlaceCells(int type)
    // works on SQUARE GRID
    {
        switch(type)
        {
            case 1:
            // Random
                for (int y=0; y < SCREEN_HEIGHT; y++)
                    {
                        for (int x=0; x < SCREEN_WIDTH; x++)
                        {
                            // instantiate cell on every loop
                            Cell cell = Instantiate(cellObject, new Vector3(x,y,0), Quaternion.identity);
                            cell.name = "x: "+ x + " y: "+ y;
                            grid[x,y] = cell;
                            //! call info from Cell script
                            
                            // grid[x,y].SetAlive(RandomAliveCell());
                            grid[x,y].SetAlive(true); //!  <============ fill screen completely
                        }
                    }


            break;

            case 2:
                for (int y=0; y < SCREEN_HEIGHT; y++)
                {
                    for (int x=0; x < SCREEN_WIDTH; x++)
                    {
                        // instantiate cell on every loop
                        Cell cell = Instantiate(cellObject, new Vector3(x,y,0), Quaternion.identity);
                        cell.name = "x: "+ x + " y: "+ y;
                        grid[x,y] = cell;
                        //! call info from Cell script
                        grid[x,y].SetAlive(false);
                    }
                }

                for (int y = 21; y < 24; y++)
                {
                    for (int x = 31; x < 38; x++)
                    {
                        /*
                            Do nothing at x = 34, x = 32 & y = 22, x = 36 & y = 22
                        */
                        if (x != 34)
                        {
                            if (y == 21 || y == 23)
                            {
                                grid[x,y].SetAlive(true);
                            }
                            else if (y == 22 && (( x != 32) && (x != 36)))                        
                            {
                                grid[x,y].SetAlive(true);
                            }
                        }
                    }
                }

            break;

            default:
                // Empty screen
                for (int x = 0; x < SCREEN_WIDTH; x++)
                {
                    for (int y = 0; y < SCREEN_HEIGHT; y++)
                    {
                        // instantiate a cell for every iteration
                        Cell cell = Instantiate(cellObject, new Vector3(x,y,0), Quaternion.identity);
                        cell.name = "x: "+ x + " y: "+ y;
                        grid[x,y] = cell;
                        // call from cell.cs
                        grid[x,y].SetAlive(false);
                    }
                }
            break;



        }
    }

    void CreateCells(int type, int x, int y)
    // works on HEXAGONAL GRID
    {
        //! Creating the hex grid
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * HexGlobals.Width;
        position.y = y * HexGlobals.RowHeight;
        

        Cell cell = Instantiate(cellObject, new Vector3(position.x,position.y,0), Quaternion.identity);
        cell.name = "x: "+ x + " y: "+ y;

        // Parent each instantiated cell to the main board Game object
        cell.transform.SetParent(this.transform);

        // make cells static. This helps optimize the playback a bit
        cell.gameObject.isStatic = true;

        
        grid[x,y] = cell;
        //! call info from Cell script

        // type options

        switch(type)
        {
            case 1:
            // Random
            grid[x,y].SetAlive(RandomAliveCell());
            // grid[x,y].SetAlive(true); //!  <============ fill screen completely
            break;

            case 2:
            grid[x,y].SetAlive(true); //!  <============ fill screen completely
            break;

            default: 
            // empty
            grid[x,y].SetAlive(false);

            break;

        }
    }

    bool RandomAliveCell()
    {
        // use the random generator with the unity namespace
        int rand = UnityEngine.Random.Range (0, 100);

        if (rand > 75)
        {
            return true;
        }
        return false;
    }

    //! Neigbhors <===================================  GAME LOGIC
    void CountNeighbors()
    {
        for (int x = 0; x < SCREEN_WIDTH; x++)
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                int numNeighbors = 0; // num of LIVE cells
                
                // check boundaries of screen. If greater than SCREEN_HEIGHT

                /* ===================================================
                // NORTH direction    
                //? NE            
                if (y + 1 < SCREEN_HEIGHT)
                {
                    //! references Cell class, thus the .isAlive works
                    if (grid[x, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                

                // EAST direction
                if (x + 1 < SCREEN_WIDTH)
                {
                    if (grid[x + 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // SOUTH direction                
                if (y - 1 >= 0)
                {
                    //! references Cell class, thus the .isAlive works
                    if (grid[x, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                
                // WEST direction
                if (x - 1 >= 0)
                {
                    if (grid[x - 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // NE
                if (y + 1 < SCREEN_HEIGHT && x + 1 < SCREEN_WIDTH)
                {
                    if (grid[x + 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // NW
                if (y + 1 < SCREEN_HEIGHT && x - 1 >= 0)
                {
                    if (grid[x - 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // SE
                if (y - 1 >= 0 && x + 1 < SCREEN_WIDTH)
                {
                    if (grid[x + 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // SW
                if (y - 1 >= 0 && x - 1 >= 0)
                {
                    if (grid[x - 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                ============================================= */

               
                // LEFT
                if (x - 1 >= 0)
                {
                    if (grid[x - 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // RIGHT
                if (x + 1 < SCREEN_WIDTH)
                {
                    if (grid[x + 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // UP
                         
                if (y + 1 < SCREEN_HEIGHT)
                {
                    //! references Cell class, thus the .isAlive works
                    if (grid[x, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // DOWN             
                if (y - 1 >= 0)
                {
                    //! references Cell class, thus the .isAlive works
                    if (grid[x, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                if(x % 2 == 0) {
                    if (y - 1 >= 0){
                        if (x - 1 >= 0){
                            if (grid[x - 1, y - 1].isAlive){
                                numNeighbors++;
                            }
                        if (x + 1 < SCREEN_WIDTH){
                            if (grid[x + 1, y - 1].isAlive){
                                numNeighbors++;
                            }
                        }
                        }
                    }
                }
                else {
                    if(y + 1 < SCREEN_HEIGHT){
                        if(x - 1 >= 0){
                            if (grid[x - 1, y + 1].isAlive){
                                numNeighbors++;
                            }
                        }
                        if(x + 1 < SCREEN_WIDTH){
                            if (grid[x + 1, y + 1].isAlive){
                                numNeighbors++;
                            }
                        }
                    }
                }

                

                 
                
                

                

                // establish the number of numNeigbors each cell has and display in Inspector
                //! forward it to the referenced Cell i.e. grid, global var numNeighbors in Cell.cs
                grid[x,y].numNeighbors = numNeighbors;

            }
        }
    }

    //! GOL Rules <================================

    void PopulationControl()
    {
        for (int x = 0; x < SCREEN_WIDTH; x++)
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                //! APPLY LOGIC to  square grid
                /*
                    - Any live cell with 2 or 3 live neighbors, survives
                    - Any dead cell with 3 live neighbors becomes a live cell
                    - All other live cells die in the next generation; all other dead cells remain dead
                */

                // if (grid[x,y].isAlive)
                // {
                //     // - Cell is alive
                //     //! check numNeighbors; if not 2 or 3 live neighbors, set alive to false
                //     if (grid[x,y].numNeighbors != 2 && grid[x,y].numNeighbors != 3)
                //     {
                //         grid[x,y].SetAlive(false);
                //     }

                // }
                // else
                // {
                //     // - Cell is dead
                //     if (grid[x,y].numNeighbors == 3)
                //     {
                //         grid[x,y].SetAlive(true);
                //     }

                // }


                //! HEX Rules
                /*
                    	1. Live Cells with 1 or 0 neighbors - dies 
	                    2. Live Cells with 2 neighbors - lives
	                    3. Live Cells with >= 3 cells - dies
                        4. Dead Cells with 2 Live neighbors - lives



                */
                 if (grid[x,y].isAlive)
                {
                    // - Cell is alive
                    //! check numNeighbors; if not exactly 2 live neighbors, set alive to false
                    if (grid[x,y].numNeighbors != 2 || grid[x,y].numNeighbors > 2)
                    {
                        grid[x,y].SetAlive(false);
                    }

                }
                else
                {
                    // - Cell is dead
                    if (grid[x,y].numNeighbors == 2)
                    {
                        grid[x,y].SetAlive(true);
                    }

                }

            }
        }
    }
}
