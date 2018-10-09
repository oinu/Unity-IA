using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Path_Following : MonoBehaviour {

    public GameObject target;
    public GameObject node;
    public float maxForce;
    public float maxSpeed;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private List<GameObject> nodeList;

	// Use this for initialization
	void Start () {
        nodeList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

        //Create a node on the target position
        if(Input.GetAxis("Jump")!=0)
        {
            GameObject n = GameObject.Instantiate<GameObject>(node);
            n.transform.position = target.transform.position;
            nodeList.Add(n);
        }

        //If exist a node, move to them
		if(nodeList.Count>0)
        {
            //If is on the node position 
            if(Vector3.Distance(nodeList[0].transform.position, this.transform.position)<=0.5)
            {
                GameObject n = nodeList[0];
                nodeList.RemoveAt(0);
                Destroy(n);
            }
            else
            {
                desiredVelocity = nodeList[0].transform.position - this.transform.position;
                desiredVelocity = desiredVelocity.normalized;
                desiredVelocity *= maxSpeed;

                steeringForce = desiredVelocity;

                steeringForce /= maxSpeed;
                steeringForce *= maxForce;

                this.transform.position = new Vector3(this.transform.position.x + steeringForce.x, this.transform.position.y, this.transform.position.z + steeringForce.z);

                //The actual velocity is the forward;
                this.transform.forward = desiredVelocity.normalized;
            }
        }
	}
}
