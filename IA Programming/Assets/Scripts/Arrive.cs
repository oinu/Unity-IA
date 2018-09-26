using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        desiredVelocity = target.transform.position - this.transform.position;
        this.transform.forward = desiredVelocity;
        

        steeringForce = desiredVelocity;
        float distance = Vector3.Distance(target.transform.position, this.transform.position);
        if(distance> target.transform.localScale.x)
        {
            steeringForce /= maxSpeed;
        }
        
        steeringForce *= maxForce;

        this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);
    }
}
