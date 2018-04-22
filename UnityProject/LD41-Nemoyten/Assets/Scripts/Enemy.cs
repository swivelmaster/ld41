using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	public float recoilAmount = 5f;
	public int startHealth = 5;
	int currentHealth = 5;
	Rigidbody rb;
	public bool alive = true;

	public GameStateManager.EnemyType enemyType = GameStateManager.EnemyType.DogO;

	public GameObject StewEnterParticle;

	public float awakeDistance = 5f;
	// Wake up agent when shot even if player is beyond awake distance
	bool wasHitOnce = false;

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
		while (alive){
			yield return new WaitForSeconds(1f);
			// (can swap from alive to dead in this one second!)
			if (isRecoiling || !alive || !agent.enabled){
				continue;
			}
			if (wasHitOnce || Vector3.Distance(GameStateManager.instance.Player.transform.position, transform.position) < awakeDistance){
				// Failsafe?
				agent.isStopped = false;
				agent.SetDestination(GameStateManager.instance.Player.transform.position);
			}
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.layer == ProjectileLayer){
			GotHit(collider.gameObject);
		} else if (collider.gameObject.layer == StewLayer){
			GameStateManager.instance.UpdateRecipe(enemyType);
			Instantiate(StewEnterParticle, transform.position, StewEnterParticle.transform.rotation);
			Destroy(this.gameObject);
		}
	}

	// This is weird and I'm tired but the point is to recoil only if we're not
	// already recoiling so we don't stack up the coroutines that stop it
	// and make weird things happen.
	public void GotHit(GameObject hitBy){
		hitBy.GetComponent<Bullet>().Hit();
		currentHealth--;
		
		wasHitOnce = true;
		if (agent.enabled){
			agent.isStopped = true;
		}
		
		Rigidbody bulletRB = hitBy.GetComponentInChildren<Rigidbody>();
		if (!isRecoiling && !alive){
			// Had to move this around because nav mesh agent conflicts with rigidbody
			// soooo... weird behavior when trying to do it this way
			// New result: Only add force when already dead
			// Player can use gun to push enemy around I guess
			rb.AddForce(bulletRB.velocity.normalized * recoilAmount * 5f, ForceMode.Impulse);
		}

		if (alive && currentHealth <= 0){
			Die();
		}

		if (!isRecoiling){
			StartCoroutine(StopRecoil());
			isRecoiling = true;
		}
	}

	IEnumerator StopRecoil(){
		yield return new WaitForSeconds(recoilTime);
		// Debug.Log("Setting velocity to zero");
		rb.velocity = new Vector3(0,0,0);
		isRecoiling = false;
		if (agent.enabled){
			agent.isStopped = false;
		}
	}

	public void PickedUp(bool pickedUp){
		rb.isKinematic = pickedUp;
		rb.useGravity = !pickedUp;
	}

	public void Die(){
		if (agent.enabled){
			agent.isStopped = true;
		}
		
		// Just kill it completely
		agent.enabled = false;
		
		alive = false;

		// This is convoluted but we need the game object that contains the mesh renderer
		// so we can rotate it to show that the character is dead
		GetComponentInChildren<MeshRenderer>().transform.Rotate(Vector3.forward, 90f);

		rb.isKinematic = false;

		// remove triggers so they don't hurt the player
		Collider[] colliders = GetComponentsInChildren<Collider>();
		foreach (Collider c in colliders){
			if (c.isTrigger){
				Destroy(c);
			}
		}

	}
}
