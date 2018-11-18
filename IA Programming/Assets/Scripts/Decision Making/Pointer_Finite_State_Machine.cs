using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject area;
    public GameObject obstacle;

    private NodeGraph[,] grid;
    private List<NodeGraph> path;
    public int gridSize;

    private Vector3 acceleration, position, velocity, desiredVelocity, steeringForce, target;
    public float maxSpeed, maxForce, mass;

	// Use this for initialization
	void Start () {
        #region INITIALIZE VARIABLES
        Vector3 p = new Vector3();
        position = pj.transform.position;
        velocity = pj.transform.forward;

        grid = new NodeGraph[gridSize, gridSize];
        path = new List<NodeGraph>();
        #endregion

        #region GRID CREATION
        //Create the Grid
        for (int i =0; i<gridSize; i++)
        {
            for(int j=0; j<gridSize; j++)
            {

                p.x = (area.transform.position.x - area.transform.localScale.x / 2) + (area.transform.localScale.x / gridSize)/2 
                    + (area.transform.localScale.x / gridSize) * i;
                p.y = pj.transform.position.y;
                p.z = (area.transform.position.z - area.transform.localScale.z / 2) + (area.transform.localScale.x / gridSize)/2 
                    + (area.transform.localScale.z / gridSize) * j;
                if(!InObstacle(p))
                {
                    grid[i, j] = new NodeGraph();
                    grid[i, j].SetPosition(p);
                }
                else
                {
                    grid[i, j] = null;
                }
            }
        }

        //Link the neighbors
        for (int i =0; i<gridSize; i++)
        {
            for(int j=0; j<gridSize; j++)
            {
                
                if (grid[i, j]!=null)
                {
                   if(i>0 && grid[i-1, j]!=null)
                   {
                        grid[i, j].left = grid[i-1, j];
                   }

                   if(i<gridSize-1 && grid[i+1, j]!=null)
                   {
                        grid[i, j].right = grid[i+1, j];
                   }

                   if(j>0 && grid[i,j-1]!=null)
                   {
                        grid[i, j].bottom = grid[i, j - 1];
                   }

                    if (j <gridSize-1 && grid[i, j + 1] != null)
                    {
                        grid[i, j].top = grid[i, j + 1];
                    }
                }
            }
        }
        #endregion

    }

    // Update is called once per frame
    void Update () {
        Movement();
	}

    /// <summary>
    /// Return if the Position is to near or inside the obstacle
    /// </summary>
    /// <param name="position">The position wants check</param>
    /// <returns></returns>
    private bool InObstacle(Vector3 position)
    {
        return position.x <= (obstacle.transform.position.x + obstacle.transform.localScale.x / 2 + pj.transform.localScale.x / 2) &&
            position.x >= (obstacle.transform.position.x - obstacle.transform.localScale.x / 2 - pj.transform.localScale.x / 2) &&
            position.z <= (obstacle.transform.position.z + obstacle.transform.localScale.z / 2 + pj.transform.localScale.z / 2) &&
            position.z >= (obstacle.transform.position.z - obstacle.transform.localScale.z / 2 - pj.transform.localScale.z / 2);
    }

    /// <summary>
    /// Move the pj for the path. Need a NodeGraph List named path.
    /// </summary>
    private void Movement()
    {
        if (path.Count > 0)
        {
            float distance = Vector3.Distance(path[0].position, pj.transform.position);

            if (distance <= 0.1f)
            {
                path.RemoveAt(0);
            }
            else
            {
                target = path[0].position;

                desiredVelocity = target - pj.transform.position;
                desiredVelocity = desiredVelocity.normalized;
                pj.transform.forward = desiredVelocity;

                desiredVelocity *= maxSpeed;

                steeringForce = desiredVelocity - velocity;
                steeringForce /= maxSpeed;
                steeringForce *= maxForce;

                acceleration = steeringForce / mass;
                velocity += acceleration * Time.deltaTime;
                position += velocity * Time.deltaTime;

                if (distance > 0.1)
                {
                    pj.transform.position = position;
                    pj.transform.forward = velocity.normalized;
                }

            }
        }
    }
}
