using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject area;
    public GameObject obstacle;

    private NodeGraph[,] grid;
    private List<NodeGraph> path, fronter,visited;
    private NodeGraph goal;
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

        //remove for a dinamic scene
        goal = grid[0, gridSize - 1]; 
        NearestNodePJ();
        CalculateHeuristicCost();
    }

    // Update is called once per frame
    void Update () {
        PathFinding();
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

    /// <summary>
    /// Use Greedy Best First Search algorithms
    /// </summary>
    private void PathFinding()
    {
        if (fronter.Count > 0)
        {
            NodeGraph p;
            while (!found)
            {
                List<NodeGraph> auxList = new List<NodeGraph>();

                p = fronter[0];
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

                fronter.Clear();
                fronter = auxList;

                HeuristicBubbleSort();
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
    /// Sort the fronter using the heuristic cost with bubblesort method
    /// </summary>
    private void HeuristicBubbleSort()
    {
        //Create and Add in the index list the index of the current fronter
        int[,] indexList = new int[fronter.Count, 2];
        for (int i = 0; i < fronter.Count; i++)
        {
            //First element is the heuristic cost and the second the index in the array.
            indexList[i, 0] = fronter[i].heuristicCost;
            indexList[i, 1] = i;

        }

        //Bubble Sort
        int index = 0;
        for (int i = 0; i < fronter.Count; i++)
        {
            index = i;
            for (int j = i; j < fronter.Count; j++)
            {
                if (indexList[j, 0] < indexList[index, 0])
                {
                    index = j;
                }
            }

            //If we find someone smaller than the current
            //change it!
            if (index != i)
            {
                int auxIndex, auxValue;
                auxValue = indexList[i, 0];
                auxIndex = indexList[i, 1];

                indexList[i, 0] = indexList[index, 0];
                indexList[i, 1] = indexList[index, 1];

                indexList[index, 0] = auxValue;
                indexList[index, 1] = auxIndex;
            }
        }

        //Create a auxiliar fronter list
        List<NodeGraph> auxFronter = new List<NodeGraph>();
        for (int i = 0; i < fronter.Count; i++)
        {
            auxFronter.Add(fronter[indexList[i, 1]]);
        }
        fronter.Clear();
        fronter = auxFronter;
    }

    /// <summary>
    /// Calculate the new heuristic cost. Based in Manhattan heuristic.
    /// </summary>
    private void CalculateHeuristicCost()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {

                if (grid[i, j] != null)
                {
                    grid[i, j].heuristicCost = (int)(Mathf.Abs(grid[i, j].position.x - goal.position.x)
                        + Mathf.Abs(grid[i, j].position.z - goal.position.z));
                }
            }
        }
    }
}
