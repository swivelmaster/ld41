using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public AudioSource explodeSound;

	public GameObject ExplodeParticle;

	int GroundLayer;

	float startTime = 0;
	float ttl = 5f;
	void Start () {
		startTime = Time.time;
		GroundLayer = LayerMask.NameToLayer("Ground");
	}
	
	void Update () {
		if (Time.time > startTime + ttl){
			Destroy(this.gameObject);
			return;
		}
	}

	public void Hit(){
		Instantiate(ExplodeParticle, transform.position, Quaternion.identity);
		if (explodeSound)
			explodeSound.Play();
		Destroy(gameObject);
	}

	public void OnTriggerEnter(Collider collider){
		if (collider.gameObject.layer == GroundLayer){
			Hit();
		}
	}
}
