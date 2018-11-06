using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    public float mass;

    private float radius;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 acceleration;
    private Vector3 t;

    // Use this for initialization
    void Start()
    {
        position = this.transform.position;
        velocity = desiredVelocity = target.transform.position - this.transform.position;
        velocity = velocity.normalized;
        velocity.y = this.transform.forward.y;
        this.transform.forward = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the new vector. That will be the desired velocity
        t = target.transform.position;
        t.y = position.y;
        desiredVelocity = t - this.transform.position;
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

        steeringForce = desiredVelocity - velocity;

        steeringForce /= maxSpeed;
        steeringForce *= maxForce;

        acceleration = steeringForce / mass;
        velocity = velocity + acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position += velocity * Time.deltaTime;

        this.transform.position = position;
        this.transform.forward = velocity.normalized;
    }
}
