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
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        #region MOVEMENT
        //To move the object i create a invisible target.
        //The target is the current position + the input. 
        target = this.transform.position;

        target.x -= Input.GetAxis("Horizontal");
        target.z -= Input.GetAxis("Vertical");

        desiredVelocity = this.transform.position - target;
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= maxSpeed;

        //If don't move, is not necessary change the forward.
        //If i do, forward is 0.
        if(this.transform.position!=target)
        {
            this.transform.forward = desiredVelocity.normalized;
        }

        //On the formula is desired velocity - velocity.
        //In Unity is not necessary if change the forward vector.
        steeringForce = desiredVelocity;
        steeringForce /= maxSpeed;
        steeringForce *= maxForce;

        //Only increment the X and Z axis because i move the
        //object like a 2D on a 3D world.
        this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);

        AreaAvoidance();
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
