using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Script : MonoBehaviour {

    public float speed;
    private Vector3 f;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        f = this.transform.forward;

       if(Input.GetAxis("Horizontal")>0)
        {
            this.transform.position = new Vector3(this.transform.position.x + speed, this.transform.position.y, this.transform.position.z);
            f.x = 1;
        }
       else if(Input.GetAxis("Horizontal") < 0)
        {
            this.transform.position = new Vector3(this.transform.position.x - speed, this.transform.position.y, this.transform.position.z);
            f.x = -1;
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + speed);
            f.z = 1;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - speed);
            f.z = -1;
        }

        this.transform.forward = f;
    }
}
