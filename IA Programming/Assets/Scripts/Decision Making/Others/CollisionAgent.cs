using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAgent : MonoBehaviour {

    public int life;
    public GameObject bullet;
    public bool shooting;
    public int munition;
    private float timer;
	// Use this for initialization
	void Start () {
        timer = 1.0f;
        shooting = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(life==0)
        {
            shooting = false;
            Destroy(this.gameObject);
        }
        if(shooting)
        {
            timer += Time.deltaTime;
            if (timer - Time.deltaTime >= 1)
            {
                GameObject b = GameObject.Instantiate<GameObject>(bullet);
                b.transform.position = this.transform.position + this.transform.forward.normalized;
                b.transform.forward = this.transform.forward;
                b.GetComponent<Bullet_Script>().playerShoot = true;
                timer = Time.deltaTime;
                munition--;
            }
        }     

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="Bullet(Clone)")
        {
            life--;
            if (this.name == "PJ" && life <= 1) life = 1;
        }
    }
}
