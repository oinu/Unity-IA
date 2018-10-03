using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {

    public GameObject target;
    public float maxForce;
    public float maxSpeed;

    private float radius;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 targetToSeek;
    private float wanderAngle;

	// Use this for initialization
	void Start () {

        //Getting the Sphere that represents a radius
        GameObject radiusObject = target.transform.GetChild(0).gameObject;

        //Getting the radius of the target
        radius = radiusObject.transform.lossyScale.x / 2;

        Vector3 d = target.transform.position - this.transform.position;
        wanderAngle = Mathf.Atan(radius / d.x);

        //Target to Seek
        targetToSeek.x = target.transform.position.x + radius * Mathf.Cos(wanderAngle);
        targetToSeek.y = target.transform.position.y + radius * Mathf.Cos(wanderAngle);
    }
	
	// Update is called once per frame
	void Update () {

        desiredVelocity = targetToSeek - this.transform.position;
        desiredVelocity = desiredVelocity.normalized;

    }
}
