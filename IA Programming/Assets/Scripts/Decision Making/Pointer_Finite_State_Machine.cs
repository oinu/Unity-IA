using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//----- THINGS I DISCARD ------//
// 1. Implement a A* or Dijkstra algorithm, because all the node have the same weight.
// 2. Implement a Greedy Best First Search algorithm. The movement is more erratic like Breadth First Search.
// 3. Implement a Predicted Path Following, because without it is the same path, more or less.
// 4. At the beginnig, the pathfinding and the pathfollowing was in this script. Now is on the state.
// 5. For shooting bullets and the bullets collision in this script.
// 6. The movement and destroy of every bullet in this script.

public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject enemyPrefab;
    public GameObject area;
    public GameObject obstacle;
    public GameObject firstAidKit;
    public GameObject munitions;
    public GameObject lifeText, munitionText;
    public int gridSize;
    public float maxSpeed, maxForce, mass;

    private GameObject enemy;
    private NodeGraph[,] grid;
    private NodeGraph goal,firstAidNode, bulletsNode, startPatrolNode, endPatrolNode;
    private State pjState;
    private float timeElipsed, randomTime;

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
        
        // Define the start and end patrol nodes
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

        //The agent start in patrol state
        pjState = new Patrol(STATES.PATROL, ref pj, ref grid, ref startPatrolNode, ref endPatrolNode, maxSpeed, maxForce, mass, gridSize);
        pjState.Start();

        //Force to first of all go to take munition
        pj.GetComponent<CollisionAgent>().munition = 0;

        //Initialize the time enemy variables
        timeElipsed = -1;
        randomTime = -1;
    }
    
    // Update is called once per frame
    void Update () {
        pjState.Update();

        //Change the state of the agent
        switch (pjState.CurrentState)
        {
            //If the current state is First Aid
            case STATES.FIRSTAID:

                //If the agent is in the destination
                if (Vector3.Distance(pj.transform.position, goal.position) <= 0.1f)
                {
                    //Change state for patrol state
                    pjState.Exit();
                    pj.GetComponent<CollisionAgent>().life = 10;
                    pjState = new Patrol(STATES.PATROL, ref pj, ref grid, ref startPatrolNode, ref endPatrolNode, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }
                break;

            //If the current state is Bullets
            case STATES.BULLETS:

                //If the agent is in the destination
                if (Vector3.Distance(pj.transform.position, goal.position) <= 0.1f)
                {
                    //Change state for patrol state
                    pjState.Exit();
                    pj.GetComponent<CollisionAgent>().munition = 10;
                    pjState = new Patrol(STATES.PATROL, ref pj, ref grid, ref startPatrolNode, ref endPatrolNode, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }
                break;

            //If the current state is Patrol
            case STATES.PATROL:

                //If exist a enemy and is the patrol line
                if (enemy != null && startPatrolNode.position.z - pj.transform.position.z <= 0.2f && startPatrolNode.position.z - pj.transform.position.z >= -0.2f)
                {
                    //Change state for fire state
                    pjState.Exit();
                    pjState = new Fire(STATES.FIRE, ref pj, ref enemy);
                    pjState.Start();
                }

                //If the agent has a 5 or less life points
                else if (pj.GetComponent<CollisionAgent>().life <= 5)
                {
                    //Change state for Firs Aid.
                    pjState.Exit();
                    goal = firstAidNode;
                    pjState = new GoTo(STATES.FIRSTAID, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }

                //If the agent has a 5 or less bullets
                else if (pj.GetComponent<CollisionAgent>().munition <= 5)
                {
                    //Change state for bullets.
                    pjState.Exit();
                    goal = bulletsNode;
                    pjState = new GoTo(STATES.BULLETS, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }
                break;

            //If the current state is Fire
            case STATES.FIRE:
                //If the agent has a 3 or less life points
                if (pj.GetComponent<CollisionAgent>().life <= 3)
                {
                    //Change state for Firs Aid.
                    pjState.Exit();
                    goal = firstAidNode;
                    pjState = new GoTo(STATES.FIRSTAID, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }
                //If the agent has a 0 bullets
                else if (pj.GetComponent<CollisionAgent>().munition == 0)
                {
                    //Change state for bullets.
                    pjState.Exit();
                    goal = bulletsNode;
                    pjState = new GoTo(STATES.BULLETS, ref goal, ref pj, ref grid, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }

                //If not exist a enemy
                else if (enemy == null)
                {
                    //Change state for patrol.
                    pjState.Exit();
                    pjState = new Patrol(STATES.PATROL, ref pj, ref grid, ref startPatrolNode, ref endPatrolNode, maxSpeed, maxForce, mass, gridSize);
                    pjState.Start();
                }
                break;

        }

        //If not exist a enemy in scene.
        if (enemy == null)
        {
            //Create a new enemey.
            NewEnemy();
        }

        //Update the UI
        lifeText.GetComponent<Text>().text = "Life: " + pj.GetComponent<CollisionAgent>().life.ToString();
        munitionText.GetComponent<Text>().text = "Bullets: " + pj.GetComponent<CollisionAgent>().munition.ToString();
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
    /// Add a new enemy in a random position in a random time.
    /// </summary>
    private void NewEnemy()
    {
        //Generate a new random time elipsed
        if (randomTime == -1)
        {
            randomTime = Random.Range(10.0f, 20.0f);
            timeElipsed = Time.deltaTime;
        }
        else timeElipsed += Time.deltaTime;

        //If the time is done
        if(timeElipsed- Time.deltaTime>=randomTime)
        {
            //Instantiate a new enemy
            enemy = GameObject.Instantiate<GameObject>(enemyPrefab);
            
            //Assign the new position
            enemy.transform.position = new Vector3(Random.Range(-4, 5), 0.75f, -4.361f);
            randomTime = -1;
        }
    }
}
