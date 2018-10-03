using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;

    private float radius;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 predictedTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float t = Vector3.Distance(target.transform.position, this.transform.position) / maxSpeed;

        //Multiply the forward vector with the speed, because the forward vector is normalized
        predictedTarget = target.transform.position + (target.transform.forward * target.GetComponent<Seek>().maxSpeed) * t;

        desiredVelocity = this.transform.position - predictedTarget;
        desiredVelocity = desiredVelocity.normalized;

        desiredVelocity *= maxSpeed;

        Vector3 velocity = this.transform.forward.normalized;

        steeringForce = desiredVelocity;// - velocity;

        steeringForce /= maxSpeed;
        steeringForce *= maxForce;

        this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);

        //The actual velocity is the forward;
        this.transform.forward = desiredVelocity.normalized;
    }
}
