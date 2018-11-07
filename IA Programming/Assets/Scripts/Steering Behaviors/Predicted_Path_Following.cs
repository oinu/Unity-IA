using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predicted_Path_Following : MonoBehaviour {

    public GameObject target;
    public GameObject node;
    public float maxForce;
    public float maxSpeed;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private List<GameObject> nodeList;
    private float predictedRadius;

    private Vector3 acceleration, velocity, position, t;
    public float mass;

    // Use this for initialization
    void Start()
    {
        nodeList = new List<GameObject>();
        predictedRadius = 1;
        position = this.transform.position;
        velocity = this.transform.forward.normalized;
    }

    // Update is called once per frame
    void Update()
    {

        //Create a node on the target position
        if (Input.GetAxis("Jump") != 0)
        {
            GameObject n = GameObject.Instantiate<GameObject>(node);
            n.transform.position = target.transform.position;
            nodeList.Add(n);
        }

        //If exist a node, move to there
        if (nodeList.Count > 0)
        {
            //If is on the node position 
            if (Vector3.Distance(nodeList[0].transform.position, this.transform.position) <= 0.5)
            {
                GameObject n = nodeList[0];
                nodeList.RemoveAt(0);
                Destroy(n);
            }
            else
            {
                t = nodeList[0].transform.position;
                t.y = position.y;
                desiredVelocity = t - this.transform.position;
                desiredVelocity = desiredVelocity.normalized;
                desiredVelocity *= maxSpeed;

                steeringForce = desiredVelocity - velocity;

                steeringForce /= maxSpeed;
                steeringForce *= maxForce;

                acceleration = steeringForce / mass;
                velocity += acceleration * Time.deltaTime;
                position += velocity * Time.deltaTime;

                this.transform.position = position;

                //Future position
                PredictedPosition(position+velocity*Time.deltaTime);

                //The actual velocity is the forward;
                this.transform.forward = velocity.normalized;
            }
        }
    }

    private void PredictedPosition(Vector3 position)
    {
        int index=-1,count=0;
        foreach(GameObject n in nodeList)
        {
            if(Vector3.Distance(n.transform.position,position)<predictedRadius)
            {
                index = count;
            }
            count++;
        }

        if(index>0)
        {
            for (int i=0; i<=index;i++)
            {
                GameObject n = nodeList[0];
                nodeList.RemoveAt(0);
                Destroy(n);
            }
        }
    }
}
