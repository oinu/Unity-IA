using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greedy_Best_First_Search : MonoBehaviour {

    //Creation of Scene
    public GameObject terrainPrefab;
    public GameObject targetPrefab;
    public GameObject pjPrefab;

    private Position[,] area;
    private Position target;
    private Position pj;
    private int size;

    //Greedy Best First Search
    //Use a SortedList because the lowest cost will be the first
    //The int will represents the currentCost of the node.
    private List<Position> fronter;
    private List<Position> visited;
    private List<Position> path;
    private float timer;
    private bool found;

    //Movement of the Agent
    public float maxForce;
    public float maxSpeed;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;


    // Use this for initialization
    void Start()
    {

        #region CREATION SCENE
        size = 10;
        area = new Position[size, size];

        //Instance a target
        target = new Position();
        target.obj = GameObject.Instantiate<GameObject>(targetPrefab);

        //Move to random position
        NewTargetPosition();

        //Creating the terrain with diferents cubes.
        //Every cube have a property of the terrain (Weight, Cost, Distance, Color, etc).
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                area[i, j] = new Position();
                area[i, j].obj = GameObject.Instantiate<GameObject>(terrainPrefab);
                area[i, j].obj.transform.position = new Vector3(terrainPrefab.transform.lossyScale.x * i - size / 2, 0, terrainPrefab.transform.lossyScale.z * j);
                area[i, j].x = i;
                area[i, j].z = j;
                area[i, j].visited = false;

                Vector3 t = target.obj.transform.position;
                t.y = area[i, j].obj.transform.position.y;

                area[i, j].heuristicCost = (int) Vector3.Distance(area[i, j].obj.transform.position, t);

                //Change the color of the cube
                Color c = new Color(0.5f, 0.5f, 1);
                area[i, j].obj.GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }

        //Instance a PJ
        pj = new Position();
        pj.obj = GameObject.Instantiate<GameObject>(pjPrefab);

        //X position is 0 because the random range is -5 and 5.
        pj.obj.transform.position = new Vector3(0.0f, 0.75f, size / 2.0f);
        pj.x = 5;
        pj.z = 5;

        area[pj.x, pj.z].visited = true;
        #endregion

        #region GREEDY BEST FIRST SEARCH

        fronter = new List<Position>();
        visited = new List<Position>();
        path = new List<Position>();

        visited.Add(area[pj.x, pj.z]);

        area[pj.x + 1, pj.z].parent = area[pj.x, pj.z];
        area[pj.x - 1, pj.z].parent = area[pj.x, pj.z];
        area[pj.x, pj.z + 1].parent = area[pj.x, pj.z];
        area[pj.x, pj.z - 1].parent = area[pj.x, pj.z];

        fronter.Add(area[pj.x, pj.z + 1]);
        fronter.Add(area[pj.x + 1, pj.z]);
        fronter.Add(area[pj.x, pj.z - 1]);
        fronter.Add(area[pj.x - 1, pj.z]);
        HeuristicBubbleSort();

        timer = Time.deltaTime;
        found = false;
        #endregion

    }

    // Update is called once per frame
    void Update()
    {

        #region GREEDY BEST FIRST SEARCH
        timer += Time.deltaTime;

        //Every second...
        if (timer > 1 && !found && fronter.Count>0)
        {
            //Copy list
            List<Position> auxList = new List<Position>();

            //Search all actually fronter nodes
            Position p = fronter[0];
            //If the current node is not visited,
            //find their neighbord and put in the fronter.
            if (!p.visited)
            {
                //We visiting the actual node, and add a visited list.
                p.visited = true;
                visited.Add(p);

                //If is the target location, we found it!
                if (p.x == target.x && p.z == target.z)
                {
                    found = true;
                    auxList.Clear();
                    Position current = p;
                    Stack<Position> stack = new Stack<Position>();

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
                }
                else
                {
                    //The current node is on the Left side
                    if (p.x == 0)
                    {
                        if (p.x < size - 1 && !area[p.x + 1, p.z].visited)
                        {
                            area[p.x + 1, p.z].parent = p;
                            auxList.Add(area[p.x + 1, p.z]);
                        }
                        if (p.z < size - 1 && !area[p.x, p.z + 1].visited)
                        {
                            area[p.x, p.z + 1].parent = p;
                            auxList.Add(area[p.x, p.z + 1]);
                        }
                        if (p.z > 0 && !area[p.x, p.z - 1].visited)
                        {
                            area[p.x, p.z - 1].parent = p;
                            auxList.Add(area[p.x, p.z - 1]);
                        }
                    }

                    //The current node is on the Right side
                    else if (p.x == size - 1)
                    {
                        if (p.x > 0 && !area[p.x - 1, p.z].visited)
                        {
                            area[p.x - 1, p.z].parent = p;
                            auxList.Add(area[p.x - 1, p.z]);
                        }
                        if (p.z < size - 1 && !area[p.x, p.z + 1].visited)
                        {
                            area[p.x, p.z + 1].parent = p;
                            auxList.Add(area[p.x, p.z + 1]);
                        }
                        if (p.z > 0 && !area[p.x, p.z - 1].visited)
                        {
                            area[p.x, p.z - 1].parent = p;
                            auxList.Add(area[p.x, p.z - 1]);
                        }
                    }

                    //The current node is between Right and Left
                    else
                    {
                        if (p.x < size - 1 && !area[p.x + 1, p.z].visited)
                        {
                            area[p.x + 1, p.z].parent = p;
                            auxList.Add(area[p.x + 1, p.z]);
                        }
                        if (p.x > 0 && !area[p.x - 1, p.z].visited)
                        {
                            area[p.x - 1, p.z].parent = p;
                            auxList.Add(area[p.x - 1, p.z]);
                        }
                    }

                    //The current node is on the Bottom (nearest at the camera)
                    if (p.z == 0)
                    {
                        if (p.z < size - 1 && !area[p.x, p.z + 1].visited)
                        {
                            area[p.x, p.z + 1].parent = p;
                            auxList.Add(area[p.x, p.z + 1]);
                        }
                        if (p.x < size - 1 && !area[p.x + 1, p.z].visited)
                        {
                            area[p.x + 1, p.z].parent = p;
                            auxList.Add(area[p.x + 1, p.z]);
                        }
                        if (p.x > 0 && !area[p.x - 1, p.z].visited)
                        {
                            area[p.x - 1, p.z].parent = p;
                            auxList.Add(area[p.x - 1, p.z]);
                        }
                    }

                    //The current node is on the Top (farest at the camera)
                    else if (p.z == size - 1)
                    {
                        if (p.z > 0 && !area[p.x, p.z - 1].visited)
                        {
                            area[p.x, p.z - 1].parent = p;
                            auxList.Add(area[p.x, p.z - 1]);
                        }
                        if (p.x < size - 1 && !area[p.x + 1, p.z].visited)
                        {
                            area[p.x + 1, p.z].parent = p;
                            auxList.Add(area[p.x + 1, p.z]);
                        }
                        if (p.x > 0 && !area[p.x - 1, p.z].visited)
                        {
                            area[p.x - 1, p.z].parent = p;
                            auxList.Add(area[p.x - 1, p.z]);
                        }
                    }

                    //The current node is between the Top and the Bottom
                    else
                    {
                        if (p.z < size - 1 && !area[p.x, p.z + 1].visited)
                        {
                            area[p.x, p.z + 1].parent = p;
                            auxList.Add(area[p.x, p.z + 1]);
                        }
                        if (p.z > 0 && !area[p.x, p.z - 1].visited)
                        {
                            area[p.x, p.z - 1].parent = p;
                            auxList.Add(area[p.x, p.z - 1]);
                        }
                    }
                }
            }

            //Clean the memory fronter and assign the auxList in there
            fronter.Clear();
            fronter = auxList;

            //Order the nodes for heuristic cost
            HeuristicBubbleSort();
            timer = Time.deltaTime;
        }


        //Paint the visited terrain with red color
        foreach (Position p in fronter)
        {
            p.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        foreach (Position p in visited)
        {
            p.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        #endregion

        #region SIMPLE PATH FOLLOWING
        foreach (Position p in path)
        {
            p.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }

        if (path.Count > 0)
        {
            Vector3 t = path[0].obj.transform.position;
            t.y = 0.75f;
            Vector3 p = pj.obj.transform.position;

            if (Vector3.Distance(t, p) <= 0.1)
            {
                path.RemoveAt(0);
            }
            else
            {
                desiredVelocity = t - p;
                desiredVelocity = desiredVelocity.normalized;
                desiredVelocity *= maxSpeed;

                steeringForce = desiredVelocity;

                steeringForce /= maxSpeed;
                steeringForce *= maxForce;

                pj.obj.transform.position = new Vector3(pj.obj.transform.position.x + steeringForce.x, pj.obj.transform.position.y, pj.obj.transform.position.z + steeringForce.z);

                //The actual velocity is the forward;
                pj.obj.transform.forward = desiredVelocity.normalized;
            }
        }

        #endregion
    }

    /// <summary>
    /// The function take a size and the terrain prefab, to put on the area the target
    /// </summary>
    private void NewTargetPosition()
    {
        int randX = Random.Range(0, size);
        int randZ = Random.Range(0, size);

        target.obj.transform.position = new Vector3(randX - size / 2,
            targetPrefab.transform.position.y, randZ);
        target.x = randX;
        target.z = randZ;
    }

    private void HeuristicBubbleSort()
    {
        //Create and Add in the index list the index of the current fronter
        int[,] indexList = new int[fronter.Count,2];
        for(int i=0; i <fronter.Count; i ++)
        {
            //First element is the heuristic cost and the second the index in the array.
            indexList[i,0] = fronter[i].heuristicCost;
            indexList[i, 1] = i;

        }

        //Bubble Sort
        int index = 0;
        for(int i=0; i< fronter.Count; i++)
        {
            index = i;
            for( int j=i; j< fronter.Count; j++)
            {
                if(indexList[j,0]<indexList[index,0])
                {
                    index = j;
                }
            }

            //If we find someone smaller than the current
            //change it!
            if(index!=i)
            {
                int auxIndex,auxValue;
                auxValue = indexList[i, 0];
                auxIndex = indexList[i, 1];

                indexList[i, 0] = indexList[index, 0];
                indexList[i, 1] = indexList[index, 1];

                indexList[index, 0] = auxValue;
                indexList[index, 1] = auxIndex;
            }
        }

        //Create a auxiliar fronter list
        List<Position> auxFronter = new List<Position>();
        for(int i=0; i< fronter.Count; i++)
        {
            auxFronter.Add(fronter[indexList[i, 1]]);
        }
        fronter.Clear();
        fronter = auxFronter;
    }
}
