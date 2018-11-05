using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour {

    //Creation of Scene
    public GameObject terrainPrefab;
    public GameObject targetPrefab;
    public GameObject pjPrefab;
    public int minWeight, mediumWeight, maxWeight;

    private NodeGraph[,] area;
    private NodeGraph target;
    private NodeGraph pj;
    private int size;

    //Greedy Best First Search
    //Use a SortedList because the lowest cost will be the first
    //The int will represents the currentCost of the node.
    private List<NodeGraph> fronter;
    private List<NodeGraph> visited;
    private List<NodeGraph> path;
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
        area = new NodeGraph[size, size]();
        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                area[i, j] = new NodeGraph();
                area[i, j].SetPosition(new Vector3(terrainPrefab.transform.lossyScale.x * i - size / 2, 0, terrainPrefab.transform.lossyScale.z * j));
            }
        }


        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        DrawTerrain();
    }

    /// <summary>
    /// The function take a size and the terrain prefab, to put on the area the target
    /// </summary>
    private void NewTargetPosition()
    {
        int randX = Random.Range(0, size);
        int randZ = Random.Range(0, size);
    }

    //REFORMULATE
    private void DijkstraBubbleSort()
    {
        //Create and Add in the index list the index of the current fronter
        int[,] indexList = new int[fronter.Count, 2];
        for (int i = 0; i < fronter.Count; i++)
        {
            //First element is the heuristic cost and the second the index in the array.
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

    private void DrawTerrain()
    {
        foreach(NodeGraph n in area)
        {
            if(n.weight<=minWeight)
            {
                n.obj.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
            }
            else if(n.weight>minWeight && n.weight<=mediumWeight)
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
}
