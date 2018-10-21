﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example1 : MonoBehaviour {

    public GameObject pj;
    public float maxForce;
    public float maxSpeed;

    private NodeGraph[] area;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;

    private List<NodeGraph> fronter;
    private List<NodeGraph> visited;
    private List<NodeGraph> path;
	// Use this for initialization
	void Start () {

        #region CREATE THE GRAPH
        area = new NodeGraph[6];
        for(int i=0; i<6; i++)
        {
            area[i] = new NodeGraph();
        }

        area[0].bottom = area[1];
        area[0].position = new Vector3(-3.0f, 0.75f, 8.5f);

        area[1].top = area[0];
        area[1].right = area[4];
        area[1].bottom = area[2];
        area[1].position = new Vector3(-3.0f, 0.75f, 5.0f);

        area[2].top = area[1];
        area[2].position = new Vector3(-3.0f, 0.75f, 1.5f);

        area[3].bottom = area[4];
        area[3].position = new Vector3(3.0f, 0.75f, 8.5f);

        area[4].top = area[3];
        area[4].left = area[1];
        area[4].bottom = area[5];
        area[4].position = new Vector3(3.0f, 0.75f, 5.0f);

        area[5].top = area[4];
        area[5].position = new Vector3(3.0f, 0.75f, 1.5f);
        #endregion

        fronter = new List<NodeGraph>();
        visited = new List<NodeGraph>();
        path = new List<NodeGraph>();

        path.Add(area[1]);
        path.Add(area[0]);
        path.Add(area[1]);
        path.Add(area[2]);
        path.Add(area[1]);
        path.Add(area[4]);
        path.Add(area[3]);
        path.Add(area[4]);
        path.Add(area[5]);
    }
	
	// Update is called once per frame
	void Update () {
        
        Movement();
	}

    private void Movement()
    {
        if(path.Count>0)
        {
            float distance = Vector3.Distance(path[0].position, pj.transform.position);

            if (distance <= 0.1f)
            {
                path.RemoveAt(0);
            }
            else
            {
                desiredVelocity = path[0].position - pj.transform.position;
                desiredVelocity = desiredVelocity.normalized;
                pj.transform.forward = desiredVelocity;

                desiredVelocity *= maxSpeed;

                steeringForce = desiredVelocity;
                steeringForce /= maxSpeed;
                steeringForce *= maxForce;

                if (distance > 0.1)
                    pj.transform.position += steeringForce;
            }
        }
    }
}

public class NodeGraph
{
    public NodeGraph top;
    public NodeGraph right;
    public NodeGraph bottom;
    public NodeGraph left;
    public NodeGraph parent;

    public Vector3 position;

    public float heuristicCost;

    public NodeGraph()
    {
        top = null;
        right = null;
        bottom = null;
        left = null;
        parent = null;
        heuristicCost = 0;
        position = new Vector3();
    }
}
