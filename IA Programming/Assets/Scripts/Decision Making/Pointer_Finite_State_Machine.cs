using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer_Finite_State_Machine : MonoBehaviour {
    public GameObject pj;
    public GameObject area;
    public GameObject obstacle;

    private NodeGraph[,] grid;
    private int size;

	// Use this for initialization
	void Start () {
        size = 20;
        grid = new NodeGraph[size, size];

        grid[0, 0] = new NodeGraph();
        grid[0, 0].obj = new GameObject();
        Vector3 p = (area.transform.position - area.transform.localScale / 2) + (area.transform.localScale / 20) * 5;//i & j ;
        p.y = pj.transform.position.y;
        grid[0, 0].SetPosition(p);
        pj.transform.position = grid[0, 0].position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
