using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public AudioSource explodeSound;

	public GameObject ExplodeParticle;

	float startTime = 0;
	float ttl = 5f;
	void Start () {
		startTime = Time.time;
	}
	
	void Update () {
		if (Time.time > startTime + ttl){
			Destroy(this.gameObject);
			return;
		}
	}

	public void Hit(){
		Instantiate(ExplodeParticle, transform.position, Quaternion.identity);
		explodeSound.Play();
		Destroy(gameObject);
	}
}
