using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search : MonoBehaviour {

    //Creation of Scene
    public GameObject terrainPrefab;
    public GameObject targetPrefab;
    public GameObject pjPrefab;

    private GameObject[,] area;
    private GameObject target;
    private GameObject pj;
    private int size;

    //Breadth First Search
    //Dictionary? problem with valors?
    private List<GameObject> fronter;
    private List<GameObject> visited;

	// Use this for initialization
	void Start () {

        #region CREATION SCENE
        size = 10;
        area = new GameObject[size, size];

        //Creating the terrain with diferents cubes.
        //Every cube have a property of the terrain (Weight, Cost, Distance, Color, etc).
        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                area[i, j] = GameObject.Instantiate<GameObject>(terrainPrefab);
                area[i, j].transform.position = new Vector3(terrainPrefab.transform.lossyScale.x * i - size/2, 0, terrainPrefab.transform.lossyScale.z * j);

                //Change the color of the cube
                Color c;
                if ((j+i) % 2 != 0) c = new Color(1, 1, 1);
                else c = new Color(0.5f, 0.5f, 1);
                area[i, j].GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }
        //Instance a target
        target = GameObject.Instantiate<GameObject>(targetPrefab);

        //Move to random position
        NewTargetPosition();

        //Instance a PJ
        pj = GameObject.Instantiate<GameObject>(pjPrefab);

        //X position is 0 because the random range is -5 and 5.
        pj.transform.position = new Vector3(0, 0.75f, size/2);
        #endregion

    }

    // Update is called once per frame
    void Update () {

        //Change the target position
		if(Input.GetAxis("Jump")!=0)
        {
            NewTargetPosition();
        }
	}

    /// <summary>
    /// The function take a size and the terrain prefab, to put on the area the target
    /// </summary>
    private void NewTargetPosition()
    {
        int randX = Random.Range(-size / 2, size / 2);
        int randZ = Random.Range(0, size);

        target.transform.position = new Vector3(randX,
            targetPrefab.transform.position.y, randZ);
    }
}
