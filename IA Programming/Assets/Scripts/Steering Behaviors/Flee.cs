using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    public float mass;

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
        t = target.transform.position;
        t.y = position.y;

        if (Vector3.Distance(t, position) > 0.1)
        {
            desiredVelocity = this.transform.position - t;
            desiredVelocity = desiredVelocity.normalized;
            desiredVelocity *= maxSpeed;

            steeringForce = desiredVelocity - velocity;
            steeringForce /= maxSpeed;
            steeringForce *= maxForce;

            acceleration = steeringForce / mass;
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            position = position + velocity * Time.deltaTime;

            this.transform.position = position;
            this.transform.forward = velocity.normalized;
        }
    }
}
