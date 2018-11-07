using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    public float wanderMaxChange;
    public float randomRange;

    private float radius;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 targetToSeek;
    private float wanderAngle;

    private Vector3 acceleration, velocity, position;
    public float mass;

	// Use this for initialization
	void Start () {

        //Getting the Sphere that represents a radius
        GameObject radiusObject = target.transform.GetChild(0).gameObject;

        //Getting the radius of the target
        radius = radiusObject.transform.lossyScale.x / 2;

        Vector3 d = target.transform.position - this.transform.position;
        float rand = Random.Range(-randomRange, randomRange);
        wanderAngle = Mathf.Atan(radius / d.magnitude) + rand * wanderMaxChange;

        velocity = target.transform.position - this.transform.position;
        position = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        //Change the angle
        if(Input.GetAxis("Jump")!=0)
        {
            Vector3 d = target.transform.position - this.transform.position;
            float rand = Random.Range(-randomRange, randomRange);
            wanderAngle = Mathf.Atan(radius / d.magnitude) + rand * wanderMaxChange;
        }

        //Target to Seek
        targetToSeek.x = target.transform.position.x + radius * Mathf.Cos(wanderAngle);
        targetToSeek.y = this.transform.position.y;
        targetToSeek.z = target.transform.position.z + radius * Mathf.Sin(wanderAngle);

        if (Vector3.Distance(targetToSeek, position) > 0.1)
        {

            desiredVelocity = targetToSeek - this.transform.position;
            desiredVelocity = desiredVelocity.normalized;
            desiredVelocity *= maxSpeed;

            steeringForce = desiredVelocity - velocity;

            steeringForce /= maxSpeed;
            steeringForce *= maxForce;

            acceleration = steeringForce / mass;
            velocity += acceleration * Time.deltaTime;
            position += velocity * Time.deltaTime;

            this.transform.position = position;

            this.transform.forward = velocity.normalized;
        }
    }
}
