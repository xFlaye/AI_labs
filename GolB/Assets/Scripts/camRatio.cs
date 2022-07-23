using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRatio : MonoBehaviour
{

    
    public float errorMargin; 
    public float camDepth = -10;

    // Start is called before the first frame update
    void Start()
    {
        //! for Hexagons
        // call within the method; don't define outside
        float hexPos = HexGlobals.Radius;

        float aspectRatio = Camera.main.aspect; // width divided by height
        float camSize = Camera.main.orthographicSize; 
        float correctPositionX = aspectRatio * camSize;
        //! square
        // Camera.main.transform.position = new Vector3(correctPositionX + errorMargin, camSize + errorMargin, camDepth);

        //! hex
        Camera.main.transform.position = new Vector3(correctPositionX - hexPos, camSize - hexPos, camDepth);
        
        
        
        
        // Value checks
        // Debug.Log("Screen Width : " + Screen.width);
        // Debug.Log("Screen Height : " + Screen.height);
        // Debug.Log("aspect ratio : " + aspectRatio);
        // Debug.Log("hexpos : " + hexPos);
    }
    
}
