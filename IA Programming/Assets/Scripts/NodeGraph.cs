using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph
{
    public NodeGraph top;
    public NodeGraph right;
    public NodeGraph bottom;
    public NodeGraph left;
    public NodeGraph parent;

    public Vector3 position;

    public int heuristicCost;
    public int weight;
    public int acomulatedCost;

    public bool visited;

    public GameObject obj;

    public NodeGraph()
    {
        top = null;
        right = null;
        bottom = null;
        left = null;
        parent = null;
        heuristicCost = 0;
        weight = 0;
        acomulatedCost = 0;
        visited = false;
        position = new Vector3();
    }

    /// <summary>
    /// Set a position in the position and GameObject variables
    /// </summary>
    /// <param name="pos">Is the new position</param>
    public void SetPosition(Vector3 pos)
    {
        position = pos;
        if(obj!=null)obj.transform.position = pos;
    }
    public Vector3 GetPosition()
    {
        return position;

    }

}
