using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----- THINGS I DISCARD ------//
// 1. Implement a A* or Dijkstra algorithm, because all the node have the same weight.
// 2. Implement a Greedy Best First Search algorithm. The movement is more erratic like Breadth First Search.
// 3. Implement a Predicted Path Following, because without it is the same path, more or less.
// 4. At the beginnig, the pathfinding and the pathfollowing was in this script. Now is on the state.

//Actually working in patrol state. Some problems to implement a patrol enemy using the same states (gird)

public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject enemy;
    public GameObject area;
    public GameObject obstacle;
    public GameObject firstAidKit;
    public GameObject munitions;
    public GameObject bullet;
    public int gridSize;
    public float maxSpeed, maxForce, mass;

    private NodeGraph[,] grid;
    private NodeGraph goal,firstAidNode, bulletsNode, startPatrolNode, endPatrolNode;
    private State pjState;

    // Use this for initialization
    void Start () {
        #region INITIALIZE VARIABLES
        Vector3 p = new Vector3();
        grid = new NodeGraph[gridSize, gridSize];
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

        #region MOVE FIRST AID & BULLETS
        int index = 0;
        int index2 = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] != null && Vector3.Distance(firstAidKit.transform.position, grid[i, j].position)
                    < Vector3.Distance(firstAidKit.transform.position, grid[index, index2].position))
                {
                    index = i;
                    index2 = j;
                }
            }
        }
        firstAidKit.transform.position = grid[index, index2].position;
        firstAidNode = grid[index, index2];

        index = 0;
        index2 = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] != null && Vector3.Distance(munitions.transform.position, grid[i, j].position)
                    < Vector3.Distance(munitions.transform.position, grid[index, index2].position))
                {
                    index = i;
                    index2 = j;
                }
            }
        }
        munitions.transform.position = grid[index, index2].position;
        bulletsNode = grid[index, index2];
        #endregion

        goal = firstAidNode;
        

        index = 0;
        for(int i=0; i<gridSize; i++)
        {
            if(Mathf.Abs(grid[0,i].position.z - pj.transform.position.z) <= Mathf.Abs(grid[0, index].position.z - pj.transform.position.z))
            {
                index = i;
            }
        }

        startPatrolNode = grid[0, index];
        endPatrolNode = grid[gridSize - 1, index];

        //pjState = new Patrol(STATES.PATROL, ref pj, ref grid, ref startPatrolNode, ref endPatrolNode, maxSpeed, maxForce, mass, gridSize);
        pjState = new Fire(STATES.FIRE, ref pj, ref enemy, ref bullet);
        pjState.Start();
    }

    // Update is called once per frame
    void Update () {
        pjState.Update();

        //Basic Change State
        if(Vector3.Distance(pj.transform.position,goal.position)<=0.1f)
        {
            
            if (pjState.CurrentState==STATES.FIRSTAID)
            {
                pjState.Exit();
                goal = bulletsNode;
                pjState = new GoTo( STATES.BULLETS, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                pjState.Start();
            }
            else
            {
                pjState.Exit();
                goal = firstAidNode;
                pjState = new GoTo(STATES.FIRSTAID, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                pjState.Start();
            }
        }
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
}
