using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script : MonoBehaviour {
    public bool collision;
    public bool playerShoot;
    private float timer;

	// Use this for initialization
	void Start () {
        collision = false;
        timer = Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.position += this.transform.forward.normalized * Time.deltaTime;
        timer += Time.deltaTime;
        if(timer>=7)
        {
            Destroy(this.gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        collision = true;
        Destroy(this.gameObject);
    }
}
