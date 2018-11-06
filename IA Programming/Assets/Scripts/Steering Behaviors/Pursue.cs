using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;
    public float mass;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 predictedTarget;
    private Vector3 velocity;
    private Vector3 position;
    private Vector3 tar;
    private Vector3 acceleration;

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

        tar = target.transform.position;
        tar.y = position.y;
        if (Vector3.Distance(tar, this.transform.position) > 0.2)
        {
            float t = Vector3.Distance(tar, this.transform.position) / target.GetComponent<Seek>().maxSpeed;

            predictedTarget = tar + target.GetComponent<Seek>().velocity * t;

            desiredVelocity = predictedTarget - this.transform.position;
            desiredVelocity = desiredVelocity.normalized;

            desiredVelocity *= maxSpeed;

            steeringForce = desiredVelocity - velocity;

            steeringForce /= maxSpeed;
            steeringForce *= maxForce;

            acceleration = steeringForce / mass;
            velocity += acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            position += velocity * Time.deltaTime;

            this.transform.position = position;
            this.transform.forward = velocity.normalized;
        }
    }
}
