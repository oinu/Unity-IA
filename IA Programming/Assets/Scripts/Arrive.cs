using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    private float radius;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the new vector. That will be the desired velocity
        desiredVelocity = target.transform.position - this.transform.position;
        desiredVelocity = desiredVelocity.normalized;

        //Getting the distance between the target and the agent position
        float distance = Vector3.Distance(target.transform.position, this.transform.position);

        //Getting the Sphere that represents a radius
        GameObject radiusObject = target.transform.GetChild(0).gameObject;

        //Getting the radius of the target
        radius = radiusObject.transform.lossyScale.x / 2;

        float speed;
        if (distance <= radius)
        {
            float factor = distance / radius;
            speed = maxSpeed * factor;
        }
        else
        {
            speed = maxSpeed;
        }
        //Discoment to see the decrease of the speed when the agent enter in the radius.
        //Debug.Log(speed);

        desiredVelocity *= speed;

        Vector3 velocity = this.transform.forward.normalized;

        steeringForce = desiredVelocity;// - velocity;

        steeringForce /= maxSpeed;
        steeringForce *= maxForce;

        this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);
        
        //The actual velocity is the forward;
        this.transform.forward = desiredVelocity.normalized;

    }
}
