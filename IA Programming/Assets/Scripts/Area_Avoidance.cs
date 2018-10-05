using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area_Avoidance : MonoBehaviour {
    public GameObject area;

    private float left, right, front, back;

	// Use this for initialization
	void Start () {
        left = area.transform.position.x - area.transform.lossyScale.x / 2;
        right = area.transform.position.x + area.transform.lossyScale.x / 2;
        front = area.transform.position.z - area.transform.lossyScale.z / 2;
        back = area.transform.position.z + area.transform.lossyScale.z / 2;
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 p = this.transform.position;

        if(this.transform.position.x<left)
        {
            p.x = left;
        }
        else if(this.transform.position.x > right)
        {
            p.x = right;
        }

        if (this.transform.position.z < front)
        {
            p.z = front;
        }
        else if (this.transform.position.z > back)
        {
            p.z = back;
        }

        this.transform.position = p;
    }
}
