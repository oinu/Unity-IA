using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search : MonoBehaviour {

    public GameObject terrain;
    public GameObject target;

    private GameObject[,] area;
    private GameObject currenTarget;
    private int size;

	// Use this for initialization
	void Start () {
        size = 10;
        area = new GameObject[size, size];

        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                area[i, j] = GameObject.Instantiate<GameObject>(terrain);
                area[i, j].transform.position = new Vector3(terrain.transform.lossyScale.x * i - size/2, 0, terrain.transform.lossyScale.z * j);

                //Change the color of the cube
                Color c;
                if ((j+i) % 2 != 0) c = new Color(1, 1, 1);
                else c = new Color(0.5f, 0.5f, 1);
                area[i, j].GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }
        currenTarget = GameObject.Instantiate<GameObject>(target);
        NewCurrenTargetPosition();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Jump")!=0)
        {
            NewCurrenTargetPosition();
        }
	}

    private void NewCurrenTargetPosition()
    {
        int randX = Random.Range(-size / 2, size / 2);
        int randZ = Random.Range(0, size);

        currenTarget.transform.position = new Vector3(randX,
            target.transform.position.y, randZ);
    }
}
