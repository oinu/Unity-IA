using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATES { FIRSTAID,BULLETS};

/// <summary>
/// A class that represents a base State
/// </summary>
public class State {
    private STATES currentState;

    public STATES CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
        }
    }

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

    /// <summary>
    /// Move the agent to the next node in the path, using Seek Steering Behaviors formula
    /// </summary>
    /// <param name="path">The path to follow</param>
    /// <param name="pj">The agent</param>
    /// <param name="velocity">The velocity of the agent</param>
    /// <param name="position">The position of the agent</param>
    /// <param name="maxSpeed">The maxSpeed of the agent</param>
    /// <param name="maxForce">The maxForce of the agent</param>
    /// <param name="mass">The mass of the agent</param>
    protected void Movement(ref List<NodeGraph> path, ref GameObject pj,ref Vector3 velocity,ref Vector3 position, float maxSpeed, float maxForce, float mass)
    {
        Vector3 target, desiredVelocity, steeringForce, acceleration;
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

/// <summary>
/// A state for move the agent to the current position to a goal.
/// </summary>
public class GoTo : State
{
    private NodeGraph goal;
    private GameObject pj;
    private NodeGraph[,] grid;
    private List<NodeGraph> path, fronter, visited;
    private Vector3 acceleration, position, velocity, desiredVelocity, steeringForce, target;
    private float maxSpeed, maxForce, mass;
    private int gridSize;

    public GoTo( STATES aState, ref NodeGraph aGoal, ref GameObject aPj, ref NodeGraph[,] aGrid, float aSpeed, float aForce, float aMass, int aGridSize)
    {
        CurrentState = aState;
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
        fronter.Add(NearestNode(ref grid, pj.transform.position, gridSize));
        pj.transform.position = fronter[0].position;
        path = BreathFirstSearchPathFinding(ref fronter, ref visited, ref goal);
    }

    public override void Update()
    {
        Movement(ref path, ref pj, ref velocity, ref position, maxSpeed, maxForce, mass);
    }

    public override void Exit()
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
        fronter = null;
        visited.Clear();
        visited = null;
    }
}