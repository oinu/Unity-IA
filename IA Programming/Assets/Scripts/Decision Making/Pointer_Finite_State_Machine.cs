using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----- THINGS I DISCARD ------//
// 1. Implement a A* or Dijkstra algorithm, because all the node have the same weight.
// 2. Implement a Greedy Best First Search algorithm. The movement is more erratic like Breadth First Search.
// 3. Implement a Predicted Path Following, because without it is the same path, more or less.


public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject area;
    public GameObject obstacle;
    public GameObject firstAidKit;
    public GameObject bullets;

    private NodeGraph[,] grid;
    private List<NodeGraph> path, fronter,visited;
    private NodeGraph goal,firstAidNode, bulletsNode;
    public int gridSize;
    private bool found;

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
        fronter = new List<NodeGraph>();
        visited = new List<NodeGraph>();
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
                    //grid[i, j].obj = GameObject.Instantiate<GameObject>(pj);
                    //grid[i, j].obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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
                if (grid[i, j] != null && Vector3.Distance(bullets.transform.position, grid[i, j].position)
                    < Vector3.Distance(bullets.transform.position, grid[index, index2].position))
                {
                    index = i;
                    index2 = j;
                }
            }
        }
        bullets.transform.position = grid[index, index2].position;
        bulletsNode = grid[index, index2];
        #endregion

        //remove for a dinamic scene
        goal = firstAidNode; 
        NearestNodePJ();
    }

    // Update is called once per frame
    void Update () {
        PathFinding();
        Movement();
        RestartPathFinding();
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

    /// <summary>
    /// Use Breath First Search algorithms
    /// </summary>
    private void PathFinding()
    {
        if (fronter.Count > 0 && !found)
        {
            //NodeGraph p;
            while (!found)
            {
                List<NodeGraph> auxList = new List<NodeGraph>();

                foreach (NodeGraph p in fronter)
                {
                    if (!p.visited)
                    {
                        p.visited = true;
                        visited.Add(p);

                        if (p.position == goal.position)
                        {
                            found = true;
                            auxList.Clear();
                            NodeGraph current = p;
                            Stack<NodeGraph> stack = new Stack<NodeGraph>();

                            while (current.parent != null)
                            {
                                stack.Push(current);
                                current = current.parent;
                            }

                            int count = stack.Count;
                            for (int i = 0; i < count; i++)
                            {
                                path.Add(stack.Pop());
                            }

                            stack.Clear();
                            break;
                        }
                        else
                        {
                            NodeGraph n;
                            if (p.top != null && !p.top.visited)
                            {
                                n = p.top;
                                n.parent = p;
                                auxList.Add(n);
                            }
                            if (p.right != null && !p.right.visited)
                            {
                                n = p.right;
                                n.parent = p;
                                auxList.Add(n);
                            }
                            if (p.bottom != null && !p.bottom.visited)
                            {
                                n = p.bottom;
                                n.parent = p;
                                auxList.Add(n);
                            }
                            if (p.left != null && !p.left.visited)
                            {
                                n = p.left;
                                n.parent = p;
                                auxList.Add(n);
                            }
                        }
                    }
                }

                fronter.Clear();
                fronter = auxList;
            }
        }
    }

    /// <summary>
    /// Add in the fronter the nearest node at pj
    /// </summary>
    private void NearestNodePJ()
    {
        //Who node on the area list is the nearest.
        int index = 0;
        int index2 = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j=0; j<gridSize; j++)
            {
                if (grid[i, j]!=null && Vector3.Distance(pj.transform.position, grid[i,j].position)
                    < Vector3.Distance(pj.transform.position, grid[index, index2].position))
                {
                    index = i;
                    index2 = j;
                }
            }
        }
        fronter.Add(grid[index,index2]);
    }

    /// <summary>
    /// Prepare all for a new PathFinding
    /// </summary>
    private void RestartPathFinding()
    {
        if (found && path.Count == 0)
        {
            if (goal == firstAidNode)
            {
                goal = bulletsNode;
            }
            else
            {
                goal = firstAidNode;
            }
            found = false;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (grid[i, j] != null)
                    {
                        grid[i, j].visited = false;
                        grid[i, j].parent = null;
                    }
                }
            }

            fronter.Clear();
            visited.Clear();
            path.Clear();
            NearestNodePJ();

        }
    }
}
