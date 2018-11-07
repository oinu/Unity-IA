using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Move_Script : MonoBehaviour {
    public GameObject area;
    public float maxSpeed;
    public float maxForce;

    private Vector3 desiredVelocity;
    private Vector3 target;
    private Vector3 steeringForce;
    public float mass;

    private Vector3 acceleration, velocity, position;
	// Use this for initialization
	void Start () {
        position = this.transform.position;
        velocity = this.transform.forward.normalized;
	}
	
	// Update is called once per frame
	void Update () {

        #region MOVEMENT
        //To move the object i create a invisible target.
        //The target is the current position + the input. 
        target = this.transform.position;

        target.x -= Input.GetAxis("Horizontal");
        target.z -= Input.GetAxis("Vertical");

        if (Vector3.Distance(position, target) > 0.1)
        {

            desiredVelocity = this.transform.position - target;
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

            AreaAvoidance();
        }
        #endregion
    }

    private void AreaAvoidance()
    {
        float left = area.transform.position.x - area.transform.lossyScale.x / 2;
        float right = area.transform.position.x + area.transform.lossyScale.x / 2;

        float front = area.transform.position.z - area.transform.lossyScale.z / 2;
        float back = area.transform.position.z + area.transform.lossyScale.z / 2;

        Vector3 pos = this.transform.position;

        if(this.transform.position.x<left )
        {
            pos.x = left;
        }
        else if(this.transform.position.x > right)
        {
            pos.x = right;
        }

        if(this.transform.position.z<front)
        {
            pos.z = front;
        }
        else if (this.transform.position.z > back)
        {
            pos.z = back;
        }

        this.transform.position = pos;
    }
}
