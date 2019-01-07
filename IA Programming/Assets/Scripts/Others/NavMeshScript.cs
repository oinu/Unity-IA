using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NavMeshScript : MonoBehaviour {

    public Camera cam;
    public GameObject zone;
    public List<GameObject> agents;
    private List<GameObject> selectedAgents;
    private Vector3 mouseClickDownScreenPosition, mouseClickUpScreenPosition;
    private bool rightDirection;
    
	// Use this for initialization
	void Start () {
        selectedAgents = new List<GameObject>();
        rightDirection = false;
    }

	// Update is called once per frame
	void Update () {

        
        //When the player click with left button
        if (Input.GetMouseButtonDown(0))
        {
            mouseClickDownScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            zone.SetActive(true);
            
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

        //When the mouse button held pressed
        if(Input.GetMouseButton(0))
        {
            //Update the second position
            mouseClickUpScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);

            //Re-size the area of zone
            zone.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(mouseClickDownScreenPosition.x - mouseClickUpScreenPosition.x)
                , Mathf.Abs(mouseClickDownScreenPosition.y - mouseClickUpScreenPosition.y));

            //Moving the image respecting the mouse position
            Vector3 aux, pos;

            float x = mouseClickDownScreenPosition.x - mouseClickUpScreenPosition.x;
            float y = mouseClickDownScreenPosition.y - mouseClickUpScreenPosition.y;

            aux = cam.ScreenToViewportPoint(mouseClickDownScreenPosition);

            if (x > 0 && y > 0)
            {
                pos = new Vector3(-zone.GetComponent<RectTransform>().rect.width / 2 + cam.pixelWidth * aux.x,
                 cam.pixelHeight * aux.y - zone.GetComponent<RectTransform>().rect.height / 2, 0);
                rightDirection = false;
            }
            else if (x < 0 && y > 0)
            {
                pos = new Vector3(zone.GetComponent<RectTransform>().rect.width / 2 + cam.pixelWidth * aux.x,
                cam.pixelHeight * aux.y - zone.GetComponent<RectTransform>().rect.height / 2, 0);
                rightDirection = true;
            }
            else if (x < 0 && y < 0)
            {
                pos = new Vector3(zone.GetComponent<RectTransform>().rect.width / 2 + cam.pixelWidth * aux.x,
                cam.pixelHeight * aux.y + zone.GetComponent<RectTransform>().rect.height / 2, 0);
                rightDirection = true;
            }
            else
            {
                pos = new Vector3(-zone.GetComponent<RectTransform>().rect.width / 2 + cam.pixelWidth * aux.x,
                cam.pixelHeight * aux.y + zone.GetComponent<RectTransform>().rect.height / 2, 0);
                rightDirection = false;
            }

            zone.GetComponent<RectTransform>().position = pos;

            SelectAgentsWithRange();
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

        //When the player release click the left button
        if(Input.GetMouseButtonUp(0))
        {
            zone.SetActive(false);
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

    //Select the agents inside the zone
    void SelectAgentsWithRange()
    {
        //Send a ray starting in the click down position
        Ray ray = cam.ScreenPointToRay(mouseClickDownScreenPosition);
        RaycastHit hit;
        Vector3 firstPosition,secondPosition;

        //If the ray hist with some object
        if (Physics.Raycast(ray, out hit))
        {
            firstPosition = hit.point;
        }
        else firstPosition = Vector3.zero;

        //Calculet the world position in the actual mouse position (if the player don't click up the button
        ray = cam.ScreenPointToRay(mouseClickUpScreenPosition);

        if (Physics.Raycast(ray, out hit))
        {
            secondPosition = hit.point;
        }
        else secondPosition = Vector3.zero;

        //If the first click is in to the left
        if(rightDirection)
        {
            foreach(GameObject agent in agents)
            {
                //If is between the first and second position
                if (agent.transform.position.x>firstPosition.x && agent.transform.position.x<secondPosition.x)
                {
                    if (agent.transform.position.z < firstPosition.z && agent.transform.position.z > secondPosition.z)
                    {
                        //Compare the color of the agent for not add the same agent in the list
                        if(agent.GetComponent<Renderer>().material.color!=Color.green)
                        {
                            agent.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            selectedAgents.Add(agent);
                        }
                    }
                    else if (agent.transform.position.z < secondPosition.z && agent.transform.position.z > firstPosition.z)
                    {
                        //Compare the color of the agent for not add the same agent in the list
                        if (agent.GetComponent<Renderer>().material.color != Color.green)
                        {
                            agent.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            selectedAgents.Add(agent);
                        }
                    }
                }
            }
        }

        //If the first click is in to the right
        else
        {
            foreach (GameObject agent in agents)
            {
                //If is between the first and second position
                if (agent.transform.position.x < firstPosition.x && agent.transform.position.x > secondPosition.x)
                {
                    if (agent.transform.position.z < firstPosition.z && agent.transform.position.z > secondPosition.z)
                    {
                        //Compare the color of the agent for not add the same agent in the list
                        if (agent.GetComponent<Renderer>().material.color != Color.green)
                        {
                            agent.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            selectedAgents.Add(agent);
                        }
                    }
                    else if (agent.transform.position.z < secondPosition.z && agent.transform.position.z > firstPosition.z)
                    {
                        //Compare the color of the agent for not add the same agent in the list
                        if (agent.GetComponent<Renderer>().material.color != Color.green)
                        {
                            agent.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            selectedAgents.Add(agent);
                        }
                    }
                }
            }
        }

        
    }
}
