using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.position += this.transform.forward.normalized * Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        //GameObject.Destroy(this.gameObject);
    }
}
