using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshScript : MonoBehaviour {

    public Camera cam;
    public List<GameObject> agents;
    private List<GameObject> selectedAgents;
    
	// Use this for initialization
	void Start () {
        selectedAgents = new List<GameObject>();
        //selectedAgents = agents;
    }
	
	// Update is called once per frame
	void Update () {

        //When the player click with left button
        if(Input.GetMouseButtonDown(0))
        {
            //Send a ray
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //If the ray hist with some object
            if(Physics.Raycast(ray,out hit))
            {
                //If hit on a agent
                if(hit.collider.gameObject.tag=="Player")
                {
                    //Change the color of the selected agents
                    hit.collider.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

                    //Add a agent selected list
                    selectedAgents.Add(hit.collider.gameObject);
                }
                else
                {
                    //Change the color of the selected agents
                    foreach(GameObject agent in selectedAgents)
                    {
                        agent.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                    }
                    
                    //Clean the list
                    selectedAgents.Clear();
                }
            }
        }

        //When the player click with the right button
        else if(Input.GetMouseButtonDown(1))
        {
            //Send a ray
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //If the ray hist with some object
            if (Physics.Raycast(ray, out hit))
            {
                //Update the position of the selected agents
                UpdateAgents(hit.point);
            }
            
        }
		
	}

    /// <summary>
    /// Update the position of the selected agents list
    /// </summary>
    /// <param name="position">The position of agents goes</param>
    void UpdateAgents(Vector3 position)
    {
        foreach (GameObject agent in selectedAgents)
        {
            agent.GetComponent<NavMeshAgent>().SetDestination(position);
        }
    }
}
