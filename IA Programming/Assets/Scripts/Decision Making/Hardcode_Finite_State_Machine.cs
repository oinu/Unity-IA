using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hardcode_Finite_State_Machine : MonoBehaviour {
    enum STATE { WORK, SLEEP, EAT, BATH };

    public GameObject pj;
    public float maxForce;
    public float maxSpeed;

    private NodeGraph[] area;

    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private Vector3 acceleration, velocity, position, t;
    public float mass;

    private List<NodeGraph> fronter;
    private List<NodeGraph> visited;
    private List<NodeGraph> path;
    private NodeGraph target;
    private bool found;

    public int decreaseSleep, decreaseEat, decreaseBath, increaseSleep, increaseEat, increaseBath;
    private int sleep, eat, bath;
    private int xp, maxXp, lvl;
    private float timer;

    public GameObject bathText,foodText,sleepText,lvlText;

    private STATE state;

	// Use this for initialization
	void Start () {

        #region CREATE THE GRAPH
        area = new NodeGraph[7];
        for(int i=0; i<7; i++)
        {
            area[i] = new NodeGraph();
        }

        area[0].bottom = area[1];
        area[0].position = new Vector3(-3.0f, 0.75f, 8.5f);

        area[1].top = area[0];
        area[1].right = area[6];
        area[1].bottom = area[2];
        area[1].position = new Vector3(-3.0f, 0.75f, 5.0f);

        area[2].top = area[1];
        area[2].position = new Vector3(-3.0f, 0.75f, 1.5f);

        area[3].bottom = area[4];
        area[3].position = new Vector3(3.0f, 0.75f, 8.5f);

        area[4].top = area[3];
        area[4].left = area[6];
        area[4].bottom = area[5];
        area[4].position = new Vector3(3.0f, 0.75f, 5.0f);

        area[5].top = area[4];
        area[5].position = new Vector3(3.0f, 0.75f, 1.5f);

        area[6].left = area[1];
        area[6].right = area[4];
        area[6].position= new Vector3(0.0f, 0.75f, 5.0f);
        #endregion

        fronter = new List<NodeGraph>();
        visited = new List<NodeGraph>();
        path = new List<NodeGraph>();

        found = false;
        state = STATE.WORK;
        target = area[5];

        NearestNodePJ();

        sleep = 100;
        eat = 100;
        bath = 100;

        xp = 0;
        maxXp = 100;
        lvl = 1;

        timer = Time.deltaTime;
        position = this.transform.position;
        velocity = this.transform.forward;
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if(timer>=1)
        {
            BasicStateMachine();
            timer = Time.deltaTime;
            bathText.GetComponent<Text>().text = "Bath: " + bath.ToString() + "%";
            foodText.GetComponent<Text>().text = "Food: " + eat.ToString() + "%";
            sleepText.GetComponent<Text>().text = "Sleep: " + sleep.ToString() + "%";
            lvlText.GetComponent<Text>().text = "Lvl "+lvl+"\n " + xp.ToString() + "/" + maxXp.ToString();
        }
        PathFinding();
        Movement();
	}

    //Actually is a Breadth First Search Algorithm.
    private void PathFinding()
    {
        while(!found)
        {
            List<NodeGraph> auxList = new List<NodeGraph>();

            foreach(NodeGraph p in fronter)
            {
                if(!p.visited)
                {
                    p.visited = true;
                    visited.Add(p);

                    if (p.position == target.position)
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
                        if (p.top!=null && !p.top.visited)
                        {
                             n= p.top;
                            n.parent = p;
                            auxList.Add(n);
                        }
                        if(p.right != null && !p.right.visited)
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
                t = path[0].position;

                desiredVelocity = t - pj.transform.position;
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

    private void BasicStateMachine()
    {
        //Decrease the values
        if (Vector3.Distance(pj.transform.position, area[2].position) > 0.1) sleep -= decreaseSleep;
        if (Vector3.Distance(pj.transform.position, area[3].position) > 0.1) eat -= decreaseEat;
        if (Vector3.Distance(pj.transform.position, area[0].position) > 0.1) bath -= decreaseBath;
        if (Vector3.Distance(pj.transform.position, area[5].position) < 0.1) xp += 10;

        if(xp>=maxXp)
        {
            lvl++;
            xp = 0;
            double ex=maxXp *1.5;
            maxXp = (int)ex;
        }

        switch (state)
        {
            case STATE.WORK:
                if (bath <= 50)
                {
                    state = STATE.BATH;
                    target = area[0];
                    RestartPathfinding();
                }
                else if (eat <= 30)
                {
                    state = STATE.EAT;
                    target = area[3];
                    RestartPathfinding();
                }
                else if (sleep <= 10)
                {
                    state = STATE.SLEEP;
                    target = area[2];
                    RestartPathfinding();
                }
                break;
            case STATE.SLEEP:
                if (Vector3.Distance(pj.transform.position, area[2].position) < 0.1)sleep += increaseSleep;
                if (sleep > 100) sleep = 100;
                if (sleep == 100)
                {
                    state = STATE.WORK;
                    target = area[5];

                    RestartPathfinding();
                }
                break;
            case STATE.EAT:
                if (Vector3.Distance(pj.transform.position, area[3].position) < 0.1) eat += increaseEat;
                if (eat > 100) eat = 100;
                if (eat == 100)
                {
                    state = STATE.WORK;
                    target = area[5];

                    RestartPathfinding();
                }
                break;
            case STATE.BATH:
                if (Vector3.Distance(pj.transform.position, area[0].position) < 0.1) bath += increaseBath;
                if (bath > 100) bath = 100;
                if(bath ==100)
                {
                    state = STATE.WORK;
                    target = area[5];

                    RestartPathfinding();
                }
                break;
        }
    }

    private void NearestNodePJ()
    {
        //Who node on the area list is the nearest.
        int index = 0;
        for (int i = 0; i < area.Length; i++)
        {
            if (Vector3.Distance(pj.transform.position, area[i].position) < Vector3.Distance(pj.transform.position, area[index].position))
            {
                index = i;
            }
        }
        fronter.Add(area[index]);
    }

    private void RestartPathfinding()
    {
        found = false;

        //Change to no visited and eliminate the parent
        for (int i = 0; i < area.Length; i++)
        {
            area[i].visited = false;
            area[i].parent = null;
        }

        //Restart the fronter, visited list.
        fronter.Clear();
        visited.Clear();
        path.Clear();
        NearestNodePJ();
    }
}