using UnityEngine;

public class Move : MonoBehaviour {

    public Vector3 goal = new Vector3(5, 0, 4);
    public float speed = 1f;
    

    void Start() {

        //goal = goal * 0.001f;

       
    }

    private void LateUpdate() { // calculates after physics are done, instead of Update()
         this.transform.Translate(goal.normalized*speed* Time.deltaTime);

    }
}
