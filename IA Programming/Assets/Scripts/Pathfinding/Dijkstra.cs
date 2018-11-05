using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour {

    //Creation of Scene
    public GameObject terrainPrefab;
    public GameObject targetPrefab;
    public GameObject pjPrefab;
    public GameObject nodePrefab;
    public int minWeight, mediumWeight, maxWeight;

    private NodeGraph[,] area;
    private NodeGraph target;
    private NodeGraph pj;
    private int size;

    //Dijkstra
    private List<NodeGraph> fronter;
    private List<NodeGraph> visited;
    private List<NodeGraph> path;
    NodeGraph node;
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
        #region CREATION_TERRAIN
        size = 10;

        //CREATE THE GRID
        area = new NodeGraph[size, size];
        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                area[i, j] = new NodeGraph();
                area[i, j].obj = GameObject.Instantiate<GameObject>(terrainPrefab);
                area[i, j].SetPosition(new Vector3(terrainPrefab.transform.lossyScale.x * i - size / 2, 0, terrainPrefab.transform.lossyScale.z * j));

                int rand = Random.Range(1, 11);
                if (rand <= 4) area[i, j].weight = minWeight;
                else if(rand <=8) area[i, j].weight = mediumWeight;
                else area[i, j].weight = maxWeight;
            }
        }

        //ASSOCIATE THE NEIGHBORS
        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size;j++)
            {
                if (i > 0)
                {
                    area[i, j].left = area[i - 1, j];
                }
                if (i < size - 1)
                {
                    area[i, j].right = area[i + 1, j];
                }

                if (j > 0)
                {
                    area[i, j].top = area[i, j - 1];
                }
                if (j < size - 1)
                {
                    area[i, j].bottom = area[i, j + 1];
                }
            }
        }
        #endregion

        fronter = new List<NodeGraph>();
        visited = new List<NodeGraph>();
        path = new List<NodeGraph>();
        target = new NodeGraph();
        pj = new NodeGraph();
        found = false;

        //PJ
        pj.obj = GameObject.Instantiate<GameObject>(pjPrefab);
        Vector3 p = area[size / 2, size / 2].GetPosition();
        p.y = 0.8f;
        pj.SetPosition(p);

        //Target
        target.obj = GameObject.Instantiate<GameObject>(targetPrefab);
        int randX = Random.Range(0, size);
        int randZ = Random.Range(0, size);

        p = area[randX, randZ].position;
        p.y = target.obj.transform.position.y;
        target.SetPosition(p);

        #region ADD_FRONTER
        area[size / 2, size / 2].visited = true;
        visited.Add(area[size / 2, size / 2]);

        area[size / 2, size / 2].right.acomulatedCost = area[size / 2, size / 2].right.weight;
        area[size / 2, size / 2].right.parent = area[size / 2, size / 2];
        fronter.Add(area[size / 2, size / 2].right);

        area[size / 2, size / 2].bottom.acomulatedCost = area[size / 2, size / 2].bottom.weight;
        area[size / 2, size / 2].bottom.parent = area[size / 2, size / 2];
        fronter.Add(area[size / 2, size / 2].bottom);

        area[size / 2, size / 2].left.acomulatedCost = area[size / 2, size / 2].left.weight;
        area[size / 2, size / 2].left.parent = area[size / 2, size / 2];
        fronter.Add(area[size / 2, size / 2].left);

        area[size / 2, size / 2].top.acomulatedCost = area[size / 2, size / 2].top.weight;
        area[size / 2, size / 2].top.parent = area[size / 2, size / 2];
        fronter.Add(area[size / 2, size / 2].top);
        #endregion

        timer = Time.deltaTime;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>=1.0f)
        {
            if (!found) DijkstraPathFinding();
            DijkstraBubbleSort();
            timer = Time.deltaTime;
        }
        DrawTerrain();
    }

    /// <summary>
    /// Use a Dijkstra algorism
    /// </summary>
    private void DijkstraPathFinding()
    {
        node = fronter[0];
        node.visited = true;

        //If the current node is in the target position
        if(node.position.x==target.position.x && node.position.z == target.position.z)
        {
            //Create a stack for more ease re-order the path
            Stack<NodeGraph> stack = new Stack<NodeGraph>();

            //Indicate is founded
            found = true;
            while(node.parent!=null)
            {
                stack.Push(node);
                node = node.parent;
            }

            int count = stack.Count;
            for(int i=0; i<count; i++)
            {
                path.Add(stack.Pop());
            }

            //See the path
            GameObject g;
            foreach (NodeGraph n in path)
            {
                g = GameObject.Instantiate<GameObject>(nodePrefab);
                g.transform.position = new Vector3(n.position.x, 1.0f, n.position.z);
            }

            //Clean the fronter
            fronter.Clear();
        }

        //Some neighbor in right and is not the parent
        if (node.right != null && node.right!=node.parent)
        {
            //If is not visited
            if (!node.right.visited)
            {
                //Calculate the new acomulated cost
                node.right.acomulatedCost = node.acomulatedCost + node.right.weight;

                //Indicate how is the parent
                node.right.parent = node;

                //If not exist in the fornter, add on there.
                if (!IsOnFronter(ref node.right))
                {
                    fronter.Add(node.right);
                }
            }

            //If is visited
            else
            {
                //If the actual cost is smaller than the old cost
                if (node.right.acomulatedCost > node.acomulatedCost + node.right.weight)
                {
                    //Calculate the new acomulated cost
                    node.right.acomulatedCost = node.acomulatedCost + node.right.weight;

                    //Indicate how is the parent
                    node.right.parent = node;

                    //If not exist in the fornter, add on there.
                    if (!IsOnFronter(ref node.right))
                    {
                        fronter.Add(node.right);
                    }
                }
            }
        }
        //Some neighbor in left and is not the parent
        if (node.left != null && node.left != node.parent)
        {
            //If is not visited
            if (!node.left.visited)
            {
                //Calculate the new acomulated cost
                node.left.acomulatedCost = node.acomulatedCost + node.left.weight;

                //Indicate how is the parent
                node.left.parent = node;

                //If not exist in the fornter, add on there.
                if (!IsOnFronter(ref node.left))
                {
                    fronter.Add(node.left);
                }
            }
            //If is visited
            else
            {
                //If the actual cost is smaller than the old cost
                if (node.left.acomulatedCost > node.acomulatedCost + node.left.weight)
                {
                    //Calculate the new acomulated cost
                    node.left.acomulatedCost = node.acomulatedCost + node.left.weight;

                    //Indicate how is the parent
                    node.left.parent = node;

                    //If not exist in the fornter, add on there.
                    if (!IsOnFronter(ref node.left))
                    {
                        fronter.Add(node.left);
                    }
                }
            }
        }
        //Some neighbor in top and is not the parent
        if (node.top != null && node.top != node.parent)
        {
            //If is not visited
            if (!node.top.visited)
            {
                //Calculate the new acomulated cost
                node.top.acomulatedCost = node.acomulatedCost + node.top.weight;

                //Indicate how is the parent
                node.top.parent = node;

                //If not exist in the fornter, add on there.
                if (!IsOnFronter(ref node.top))
                {
                    fronter.Add(node.top);
                }
            }
            //If is visited
            else
            {
                //If the actual cost is smaller than the old cost
                if (node.top.acomulatedCost > node.acomulatedCost + node.top.weight)
                {
                    //Calculate the new acomulated cost
                    node.top.acomulatedCost = node.acomulatedCost + node.top.weight;

                    //Indicate how is the parent
                    node.top.parent = node;

                    //If not exist in the fornter, add on there.
                    if (!IsOnFronter(ref node.top))
                    {
                        fronter.Add(node.top);
                    }
                }
            }
        }
        //Some neighbor in bottom and is not the parent
        if (node.bottom != null && node.bottom != node.parent)
        {
            if (!node.bottom.visited)
            {
                //Calculate the new acomulated cost
                node.bottom.acomulatedCost = node.acomulatedCost + node.bottom.weight;

                //Indicate how is the parent
                node.bottom.parent = node;

                //If not exist in the fornter, add on there.
                if (!IsOnFronter(ref node.bottom))
                {
                    fronter.Add(node.bottom);
                }
            }
            else
            {
                //If the actual cost is smaller than the old cost
                if (node.bottom.acomulatedCost > node.acomulatedCost + node.bottom.weight)
                {
                    //Calculate the new acomulated cost
                    node.bottom.acomulatedCost = node.acomulatedCost + node.bottom.weight;

                    //Indicate how is the parent
                    node.bottom.parent = node;

                    //If not exist in the fornter, add on there.
                    if (!IsOnFronter(ref node.bottom))
                    {
                        fronter.Add(node.bottom);
                    }
                }
            }
        }

        //Add to visited node
        visited.Add(node);

        //Remove the visited node
        fronter.RemoveAt(0);
    }

    /// <summary>
    /// Order the list for acomulated cost
    /// </summary>
    private void DijkstraBubbleSort()
    {
        //Create and Add in the index list the index of the current fronter
        int[,] indexList = new int[fronter.Count, 2];
        for (int i = 0; i < fronter.Count; i++)
        {
            //First element is the acoumlated cost and the second the index in the array.
            indexList[i, 0] = fronter[i].acomulatedCost;
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
    /// Draw the colors of the terrain
    /// </summary>
    private void DrawTerrain()
    {
        foreach(NodeGraph n in area)
        {
            if(n.weight==minWeight)
            {
                n.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
            }
            else if(n.weight==mediumWeight)
            {
                n.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            }
            else
            {
                n.obj.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f,0.2f));
            }
        }

        if(fronter.Count>0)
        {
            foreach (NodeGraph n in fronter)
            {
                n.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
    }

    /// <summary>
    /// Indicate if exists the node on the fronter list
    /// </summary>
    /// <param name="n">Node to find</param>
    /// <returns>True if exist</returns>
    private bool IsOnFronter( ref NodeGraph n)
    {
        bool found = false;

        foreach(NodeGraph nod in fronter)
        {
            if(GameObject.Equals(nod.obj, n.obj))
            {
                found = true;
            }
        }

        return found;
    }
}
