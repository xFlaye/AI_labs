using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridOverlay : MonoBehaviour
{
    // create the material for the lines using GL
    private Material lineMaterial;

    // set colors
    public Color mainColor = new Color(0f, 1f, 0f, 1f); // green
    public Color subColor = new Color(0f, 0.5f, 0f, 1f);

     // cam ratios
    public float errorMargin;
    public float camDepth = -10;

    //! Grid
    // grid size
    public int gridSizeX = Screen.width;
    public int gridSizeY = Screen.height;

    // grid starting point
    public float startX;
    public float startY;
    public float startZ; // required for GL


    //! HEXAGON SETUP

    public int hexRadius; // https://www.omnicalculator.com/math/hexagon from centre to points or each side of hexagon
    
    private float vertex0X, vertex0Y, temp_vertex0Y,  
					vertex1X, vertex1Y, temp_vertex1Y, 
					vertex2X, vertex2Y, temp_vertex2Y,
					vertex3X, vertex3Y, temp_vertex3Y, 
					vertex4X, vertex4Y, temp_vertex4Y, 
					vertex5X, vertex5Y, temp_vertex5Y;

      public bool showHex = false;

    // Start is called before the first frame update
    void Start()
    {
         float aspectRatio = Camera.main.aspect; // width divided by height
        float camSize = Camera.main.orthographicSize; 
        float correctPositionX = aspectRatio * camSize;
        Camera.main.transform.position = new Vector3(correctPositionX + errorMargin, camSize + errorMargin, camDepth);
        // Debug.Log("Screen Width : " + Screen.width);
        // Debug.Log("Screen Height : " + Screen.height);
        // Debug.Log("aspect ratio : " + aspectRatio);
        
    }

    void CreateLineMaterial()
    {
        // check if lineMaterial is already created. If not, do below
        if (!lineMaterial)
        {
            var shader = Shader.Find("Hidden/Internal-Colored"); // one of the hidden shaders unity has
            lineMaterial = new Material(shader); // instantiate the shader into the Material

            // set hide flags to hide from scene and don't save it from the Garbage Collector
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            // Turn on Alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // turn off depth writing
            lineMaterial.SetInt("_ZWrite",0);

            // turn off backface culling
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);


        }
    }

     //! Inhereted methods
    // Manually destroy lineMaterial
    private void OnDisable()
    {
        DestroyImmediate(lineMaterial);
    }

    //! REQUIRED TO RENDER GL

    private void OnPostRender()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        //! code to create grid

        if (showHex)
        {
            // color grid
            GL.Color(mainColor);
            
            // int radius = (int)hexRadius;
            float height = ((float)(Mathf.Sqrt(3) * hexRadius));

            // Debug.Log("radius in int: " + radius);
            float nVert = numberOfVertHexagons(hexRadius);
            float nHort = numberofHorzHexagons(hexRadius);
                        
            CalculateVerts(hexRadius,height);

            for (int j = 0; j < nHort; j++){
					ResetTempVerts();

					for (int i = 0; i < nVert; i++){

                        // NW line
                        GL.Vertex3(vertex0X, temp_vertex0Y, startZ);
                        GL.Vertex3(vertex5X, temp_vertex5Y, startZ);

                        // SW line
                        GL.Vertex3(vertex5X, temp_vertex5Y, startZ);
                        GL.Vertex3(vertex4X, temp_vertex4Y, startZ);

                        // Top line
                        GL.Vertex3(vertex0X, temp_vertex0Y, startZ);
                        GL.Vertex3(vertex1X,temp_vertex1Y, startZ);

                        // NE line
                        GL.Vertex3(vertex1X, temp_vertex1Y, startZ);
                        GL.Vertex3(vertex2X,temp_vertex2Y, startZ);

                        if (j != nHort -1) // remove the last drawn line
                        {
                            // draw top of the next column
                            GL.Vertex3(vertex2X,temp_vertex2Y,startZ);
                            GL.Vertex3(vertex2X + hexRadius, temp_vertex2Y, startZ);
                        }

                        // SE line
                        GL.Vertex3(vertex2X,temp_vertex2Y, startZ);
                        GL.Vertex3(vertex3X, temp_vertex3Y, startZ);

                        temp_vertex5Y += height;
						temp_vertex0Y += height;
						temp_vertex4Y += height;
						temp_vertex1Y += height;
						temp_vertex3Y += height;
						temp_vertex2Y += height;
                    }
                    // Draw last line to join bottom hexagons
                    GL.Vertex3(vertex0X,temp_vertex0Y, startZ);
                    GL.Vertex3(vertex1X,temp_vertex1Y, startZ);

                    incrementX(hexRadius);


            }
            Debug.Log("Height and width: " + height + "," + hexRadius*2);
            


        }
       

        GL.End();
        
    }

    //! HEXAGON UITLITIES

        public void CalculateVerts(int radius, float height)
        {
            // Debug.Log("start hex grid");
            //! Hexagon flat-topped
            float hexSmallWidth = radius * 0.5f;
            float hexWidth = radius * 2f; // distance at it's widest (point to point E/W on flat-topped)
            // float hexHeight = hexWidth * 0.866025404f; // distance at it's shortest (N/S on flat topped)

            // clockwise from top left vertex
            vertex0X = hexSmallWidth;
            vertex0Y = height;

            vertex1X = radius + hexSmallWidth;
            vertex1Y = height;

            vertex2X = hexWidth;
            vertex2Y = height * 0.5f;

            vertex3X = radius + hexSmallWidth;
            vertex3Y = 0;

            vertex4X = hexSmallWidth;
            vertex4Y = 0;

            vertex5X = 0;
            vertex5Y = height * 0.5f;

            
        }

        public void ResetTempVerts()
        {
           
			temp_vertex0Y = vertex0Y;
			temp_vertex1Y = vertex1Y;
            temp_vertex2Y = vertex2Y;			
			temp_vertex3Y = vertex3Y;
			temp_vertex4Y = vertex4Y;
            temp_vertex5Y = vertex5Y;
        }

        public float numberOfVertHexagons(int radius)
        {
            // int maxHeight = gridSizeY -1;
            float maxHeight = gridSizeY - Mathf.Abs(startY);
            return maxHeight / (2 * radius);            

        }

        public int numberofHorzHexagons(int radius)
        {
            // Two columns of hexagons make one unit
			// Width of one hexagon = 2 x radius
			// There is an overlap of (1/2) * radius between the two columns
            int maxWidth = gridSizeX - 1;
            if (maxWidth % 7 == 0)
            {
                return maxWidth / ((7/2) * radius);
            }
            else 
            {
                return (maxWidth -1) / ((7 / 2) * radius);
            }
        }

        public void incrementX(int radius){

			vertex5X += 3 * radius;
			vertex0X += 3 * radius;
			vertex4X += 3 * radius;
			vertex1X += 3 * radius;
			vertex3X += 3 * radius;
			vertex2X += 3 * radius;

		}

    // Update is called once per frame
    void Update()
    {
        
    }
}
