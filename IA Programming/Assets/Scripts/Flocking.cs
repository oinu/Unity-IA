using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    public float neighborRadius;
    public float kSeparationForce;
    public float kCohesionForce;
    public float kAligmentForce;
    public float kMaxFlockingForce;
    public List<GameObject> neighbors;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 flockingForce;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        desiredVelocity = target.transform.position - this.transform.position;
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= maxSpeed;

        FlockingForceUpdate();
        steeringForce = desiredVelocity + flockingForce;

        steeringForce /= maxSpeed;
        steeringForce *= maxForce;

        this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);

        //The actual velocity is the forward;
        this.transform.forward = desiredVelocity.normalized;
    }

    void FlockingForceUpdate()
    {
        flockingForce = Separation() * kSeparationForce + Cohesion() * kCohesionForce + Aligment() * kAligmentForce;
        flockingForce *= kMaxFlockingForce;
    }

    private Vector3 Separation()
    {
        Vector3 s = new Vector3(0,0,0);
        float index = 0;

        foreach(GameObject n in neighbors)
        {
            if(Vector3.Distance(n.transform.position,this.transform.position)<neighborRadius)
            {
                s += (this.transform.position - n.transform.position);
                index++;
            }

        }
        s /= index;

        return s.normalized;
    }

    private Vector3 Cohesion()
    {
        Vector3 c = new Vector3(0, 0, 0);
        float index = 0;

        foreach(GameObject n in neighbors)
        {
            if(Vector3.Distance(n.transform.position,this.transform.position)<neighborRadius)
            {
                c += n.transform.position;
                index++;
            }
        }
        c /= index;
        c -= this.transform.position;
        return c.normalized;
    }

    private Vector3 Aligment()
    {
        Vector3 a = new Vector3(0, 0, 0);
        float index = 0;

        foreach(GameObject n in neighbors)
        {
            if(Vector3.Distance(n.transform.position,this.transform.position)<neighborRadius)
            {
                a += n.transform.forward;
                index++;
            }
        }
        a /= index;
        return a.normalized;
    }
}
