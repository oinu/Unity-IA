using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combine the Area Avoidance, Predicted Path Following and Wander Steering Behaviors
/// </summary>
/// 
public class Comination1 : MonoBehaviour {

    //Area Avoidance
    public GameObject area;

    //Movement
    public GameObject target;
    public float maxSpeed;
    public float maxForce;
    public float mass;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 acceleration, velocity, position, t;


    //Predicted Path Following
    public GameObject nodePrefab;
    private List<GameObject> nodeList;
    private float predictedRadius;
    private float timer;

    //Wander
    private float wanderAngle;
    private float wanderRadius;
    private float wanderMaxChange;
    private float randomRange;
    private bool wanderTrigger;

	// Use this for initialization
	void Start () {

        position = this.transform.position;
        velocity = this.transform.forward;

        //Predicted Path Following inital variable
        nodeList = new List<GameObject>();
        predictedRadius = 1;
        timer = Time.deltaTime;

        //Wander initial variables
        wanderRadius = 0.5f;
        randomRange = 10;
        wanderMaxChange = 5;
        wanderTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {

        //Increment the timer
        timer += Time.deltaTime;

        //Every 0.1 seconds if the player press jump button,
        //create a node on the position.
        if(Input.GetAxis("Jump")!=0 && timer>=0.1f)
        {
            GameObject n = GameObject.Instantiate<GameObject>(nodePrefab);
            n.transform.position = target.transform.position;
            nodeList.Add(n);
            timer = Time.deltaTime;
        }

        //If exist a node, move to there
        if(nodeList.Count>0)
        {
            //If is on the node position 
            if (Vector3.Distance(nodeList[0].transform.position, this.transform.position) <= wanderRadius*2)
            {
                GameObject n = nodeList[0];
                nodeList.RemoveAt(0);
                Destroy(n);
                wanderTrigger = true;
            }
            else
            {
                t = nodeList[0].transform.position;
                t.y = this.transform.position.y;

                if (Vector3.Distance(t, position) > 0.1)
                {
                    Vector3 targetToSeek = Wander(t);
                    desiredVelocity = targetToSeek - this.transform.position;
                    desiredVelocity = desiredVelocity.normalized;
                    desiredVelocity *= maxSpeed;

                    steeringForce = desiredVelocity - velocity;

                    steeringForce /= maxSpeed;
                    steeringForce *= maxForce;

                    acceleration = steeringForce * mass;
                    velocity += acceleration * Time.deltaTime;
                    position += velocity * Time.deltaTime;

                    this.transform.position = position;
                    this.transform.forward = velocity.normalized;

                    AreaAvoidance();

                    PredictedPosition(position + velocity * Time.deltaTime);
                }
            }
        }
    }

    /// <summary>
    /// Find a node near the predicted position
    /// </summary>
    /// <param name="position"></param>
    private void PredictedPosition(Vector3 position)
    {
        int index = -1, count = 0;

        //Searching a node near the position
        foreach (GameObject n in nodeList)
        {
            //If we find one, save the index
            if (Vector3.Distance(n.transform.position, position) < predictedRadius)
            {
                index = count;
            }
            count++;
        }

        //If we have one 
        if (index > 0)
        {
            //Destroy all the nodes of the list.
            for (int i = 0; i <= index; i++)
            {
                GameObject n = nodeList[0];
                nodeList.RemoveAt(0);
                Destroy(n);
            }

            wanderTrigger = true;
        }
    }

    private Vector3 Wander(Vector3 targetPos)
    {
        Vector3 targetToSeek = targetPos;
        Vector3 distance = targetPos - this.transform.position;
        float rand = 0;

        //When the object arrive to the target, reroll the random number
        if (wanderTrigger)
        {
            rand = Random.Range(-randomRange, randomRange);
            wanderTrigger = false;
        }
        
        wanderAngle = Mathf.Atan(wanderRadius / distance.magnitude) + rand * wanderMaxChange;
        targetToSeek.x = targetPos.x + wanderRadius * Mathf.Cos(wanderAngle);
        targetToSeek.z = targetPos.z + wanderRadius * Mathf.Sin(wanderAngle);

        return targetToSeek;
    }

    private void AreaAvoidance()
    {
        float left = area.transform.position.x - area.transform.lossyScale.x / 2;
        float right = area.transform.position.x + area.transform.lossyScale.x / 2;

        float front = area.transform.position.z - area.transform.lossyScale.z / 2;
        float back = area.transform.position.z + area.transform.lossyScale.z / 2;

        Vector3 pos = this.transform.position;

        if (this.transform.position.x < left)
        {
            pos.x = left;
        }
        else if (this.transform.position.x > right)
        {
            pos.x = right;
        }

        if (this.transform.position.z < front)
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
