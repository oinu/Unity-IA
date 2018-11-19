using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {

    }

    /// <summary>
    /// Search the nearest node in the grid of the postion.
    /// </summary>
    /// <param name="grid">The grid of nodes</param>
    /// <param name="postion">The position to find</param>
    /// <param name="gridSize">The size of the grid</param>
    /// <returns>Return the reference of node.</returns>
    protected NodeGraph NearestNode(ref NodeGraph[,] grid, Vector3 position, int gridSize)
    {
        //Who node on the area list is the nearest.
        int index = 0;
        int index2 = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] != null && Vector3.Distance(position, grid[i, j].position)
                    < Vector3.Distance(position, grid[index, index2].position))
                {
                    index = i;
                    index2 = j;
                }
            }
        }
       return grid[index, index2];
    }

    /// <summary>
    /// Use the Breath First Search algorithms
    /// </summary>
    /// <param name="fronter">The list of nodes how represents the fronter</param>
    /// <param name="visited">The list of visited nodes</param>
    /// <param name="goal">The goal to find</param>
    /// <returns>Return the path</returns>
    protected List<NodeGraph> BreathFirstSearchPathFinding(ref List<NodeGraph> fronter, ref List<NodeGraph> visited, ref NodeGraph goal)
    {
        bool found = false;
        List<NodeGraph> path = new List<NodeGraph>();
        if (fronter.Count > 0)
        {
            while (!found)
            {
                if (fronter.Count == 0) break;
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

        return path;
    }
}

public class FirstAid : State
{
    private NodeGraph goal;
    private GameObject pj;
    private NodeGraph[,] grid;
    private List<NodeGraph> path, fronter, visited;
    private Vector3 acceleration, position, velocity, desiredVelocity, steeringForce, target;
    private float maxSpeed, maxForce, mass;
    private int gridSize;

    public FirstAid(ref NodeGraph aGoal, ref GameObject aPj, ref NodeGraph[,] aGrid, float aSpeed, float aForce, float aMass, int aGridSize)
    {
        goal = aGoal;
        pj = aPj;
        grid = aGrid;
        maxForce = aForce;
        maxSpeed = aSpeed;
        mass = aMass;
        gridSize = aGridSize;

        position = pj.transform.position;
        velocity = pj.transform.forward;

        fronter = new List<NodeGraph>();
        visited = new List<NodeGraph>();
    }

    public override void Start()
    {
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

        fronter.Add(NearestNode(ref grid, pj.transform.position, gridSize));
        path = BreathFirstSearchPathFinding(ref fronter, ref visited, ref goal);
    }
}
