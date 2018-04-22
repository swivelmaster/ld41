using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	public float recoilAmount = 5f;
	public int startHealth = 5;
	int currentHealth = 5;
	Rigidbody rb;
	bool alive = true;

	public float awakeDistance = 5f;

	public float recoilTime = .5f;

	int ProjectileLayer;
	int StewLayer;

	NavMeshAgent agent;

	bool isRecoiling = false;

	void Start () {
		rb = GetComponent<Rigidbody>();
		currentHealth = startHealth;

		ProjectileLayer = LayerMask.NameToLayer("Projectiles");
		StewLayer = LayerMask.NameToLayer("Stew");

		agent = GetComponent<NavMeshAgent>();
		StartCoroutine(NavigateTic());
	}
	
	void Update () {

	}

	IEnumerator NavigateTic(){
		while (true){
			yield return new WaitForSeconds(1f);
			if (isRecoiling){
				continue;
			}
			if (Vector3.Distance(GameStateManager.instance.Player.transform.position, transform.position) < awakeDistance){
				// Failsafe?
				agent.isStopped = false;
				agent.SetDestination(GameStateManager.instance.Player.transform.position);
			}
		}
	}

	void OnTriggerEnter(Collider collider){
		if (!alive) return;

		if (collider.gameObject.layer == ProjectileLayer){
			GotHit(collider.gameObject);
		} else if (collider.gameObject.layer == StewLayer){
			
		}
	}

	public void GotHit(GameObject hitBy){
		
		isRecoiling = true;
		agent.isStopped = true;
		
		Rigidbody bulletRB = hitBy.GetComponentInChildren<Rigidbody>();
		if (isRecoiling){
			rb.AddForce(bulletRB.velocity.normalized * recoilAmount, ForceMode.Impulse);
		}
		
		hitBy.GetComponent<Bullet>().Hit();
		currentHealth--;

		if (currentHealth <= 0){
			Die();
			return;
		}

		if (isRecoiling){
			StartCoroutine(StopRecoil());
		}
	}

	IEnumerator StopRecoil(){
		yield return new WaitForSeconds(recoilTime);
		rb.velocity = new Vector3(0,0,0);
		isRecoiling = false;
		agent.isStopped = false;
	}

	public void Die(){

	}
}
