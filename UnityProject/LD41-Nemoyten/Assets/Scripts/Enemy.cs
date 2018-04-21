using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float recoilAmount = 5f;
	public int startHealth = 5;
	int currentHealth = 5;
	Rigidbody rb;
	bool alive = true;

	int ProjectileLayer;
	int StewLayer;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		currentHealth = startHealth;

		ProjectileLayer = LayerMask.NameToLayer("Projectiles");
		StewLayer = LayerMask.NameToLayer("Stew");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		if (!alive) return;

		if (collider.gameObject.layer == ProjectileLayer){
			GotHit(collider.gameObject);
		} else if (collider.gameObject.layer == StewLayer){
			
		}
	}

	public void GotHit(GameObject hitBy){
		Rigidbody bulletRB = hitBy.GetComponentInChildren<Rigidbody>();
		rb.AddForce(bulletRB.velocity.normalized * recoilAmount, ForceMode.Impulse);
		hitBy.GetComponent<Bullet>().Hit();
		currentHealth--;

		if (currentHealth <= 0){
			Die();
		}
	}

	public void Die(){

	}
}
