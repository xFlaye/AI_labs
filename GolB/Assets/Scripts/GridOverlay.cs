using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    // create the material for the lines using GL
    private Material lineMaterial;

    // main and subd lines
    public bool showMain = true;
    public bool showSub = false;

    // grid size
    public int gridSizeX;
    public int gridSizeY;

    // grid starting point
    public float startX;
    public float startY;
    public float startZ; // required for GL

    // grid step
     public float smallStep; // size of unit 
    public float largeStep; // size of subdivision

    // set colors
    public Color mainColor = new Color(0f, 1f, 0f, 1f); // green
    public Color subColor = new Color(0f, 0.5f, 0f, 1f);



    // cam ratios
    public float errorMargin;
    public float camDepth = -10;


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

    private void OnPostRender()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        //! code to create grid

        if (showSub)
        {
            // set color of subs before drawing
            GL.Color(subColor);

            // loops to iterate over the lines between origina and destination
            //! HORIZONTAL LINES
            for (float y = 0; y <= gridSizeY; y+= smallStep)
            {
                GL.Vertex3 (startX, startY + y, startZ);// starting point (i.e. 0,0,0)
                GL.Vertex3 (startX + gridSizeX, startY + y, startZ); // end point (i.e. 64, 0, 0)
            }
            //! Vertical lines
            for (float x = 0; x <= gridSizeX; x += smallStep)
            {
                GL.Vertex3(startX + x, startY, startZ);
                GL.Vertex3(startX + x, startY + gridSizeY, startZ);
            }
        }

        if (showMain)
        {
            // set color of main
            GL.Color(mainColor);

            //! Horizontal lines
            for (float y  = 0; y <= gridSizeY; y += largeStep)
            {
                GL.Vertex3(startX,startY + y, startZ); // start point (0,0,0)
                GL.Vertex3(startX + gridSizeX, startY + y, startZ); // next point
            }

            //! Vertical lines
            for (float x = 0; x <= gridSizeX; x += largeStep)
            {
                GL.Vertex3(startX + x, startY, startZ);
                GL.Vertex3(startX + x, startY + gridSizeY, startZ);
            }
        }

       

        GL.End();
    }

     
}
