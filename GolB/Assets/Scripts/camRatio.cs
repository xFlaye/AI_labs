using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camRatio : MonoBehaviour
{
    public float errorMargin;
    public float camDepth = -10;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
