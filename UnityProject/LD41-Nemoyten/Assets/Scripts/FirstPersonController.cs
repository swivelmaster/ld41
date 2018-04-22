using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour {

	public GameObject feets;
	public LayerMask groundCollision;
	public LayerMask EnemiesLayerMask;

	Rigidbody rb;

	float speed = 5f;
	float jumpVelocity = 5f;

	public Text deathText;

	bool alive = true;

	public GameObject stew;
	public GameObject invisibleFloorOfDoom;

	float rateOfFire = .5f;
	float lastFired = 0;

	public GameObject bulletPrefab;
	public GameObject bulletSpawnPoint;
	float bulletSpeed = 2.0f;

	public GameObject gun;

	GameObject fpsCamera;

	int maxHealth = 10;
	int currentHealth = 10;
	public Text HealthText;

	public GameObject PlayerBloodParticles;
	public GameObject BloodParticlesSpawnPoint;

	float invulnerableTime = 1.5f;
	bool invulnerable = false;

	int EnemiesLayer;

	void Start () {
		rb = GetComponent<Rigidbody>();
		fpsCamera = GetComponentInChildren<Camera>().gameObject;

		GameStateManager.instance.Player = this.gameObject;

		currentHealth = maxHealth;
		UpdateHealthText();

		EnemiesLayer = LayerMask.NameToLayer("Enemies");
	}

	void UpdateHealthText(){
		HealthText.text = "HEALTH: " + currentHealth.ToString();
	}

	void Update () {
		if (!alive){
			if (Input.GetKeyDown("r")){
				SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
			}
			return;
		}

		float move = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
		float strafe = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

		rb.transform.Translate(new Vector3(strafe, 0, move));

		if (Input.GetButtonDown("Jump") && Physics.Raycast(feets.transform.position, Vector3.down, .2f, groundCollision)){
			rb.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
		}

		if (Input.GetButtonDown("Fire1")){
			if (Time.time - rateOfFire > lastFired){
				//Debug.Log("Shoot " + Time.time.ToString());
				lastFired = Time.time;

				GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
				bullet.transform.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * bulletSpeed, ForceMode.Impulse);
			}
		}

		if (Input.GetKeyUp("f")){
			if (currentPickedUpEnemy){
				currentPickedUpEnemy.PickedUp(false);
				currentPickedUpEnemy.transform.parent = null;				
				currentPickedUpEnemy.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * 8f, ForceMode.Impulse);
				currentPickedUpEnemy = null;
				//Debug.Log("Dropping enemy");
				return;
			}

			RaycastHit hitInfo;
			if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hitInfo, 1.0f, EnemiesLayerMask)){
				//Debug.Log("Picking up enemy");
				PickUpObject(hitInfo.collider.gameObject);
			}
			else {
				//Debug.Log("Nothing to pick up.");
			}
		}
	}

	Enemy currentPickedUpEnemy = null;

	void PickUpObject(GameObject enemyGo){
		Enemy enemy = enemyGo.GetComponent<Enemy>();
		if (enemy.alive){
			//Debug.Log("Can't pick up an enemy that isn't dead!");
			return;
		}

		enemy.PickedUp(true);
		enemyGo.transform.parent = transform;
		currentPickedUpEnemy = enemy;
	}

	void OnTriggerEnter(Collider collider){
		if (!alive){
			return;
		}
		
		if (collider.gameObject == stew){
			alive = false;
			deathText.enabled = true;
			deathText.text = "GAME OVER: YOU WERE NOT ONE OF THE INGREDIENTS\nPRESS R TO RESTART";
		}
		else if (collider.gameObject == invisibleFloorOfDoom){
			alive = false;
			deathText.enabled = true;
			deathText.text = "GAME OVER: YOU FELL OFF OF THE WORLD. YOU WEREN'T EVEN AT 100% YET\nPRESS R TO RESTART";
			Instantiate(PlayerBloodParticles, BloodParticlesSpawnPoint.transform.position, PlayerBloodParticles.transform.rotation);
		} else if (collider.gameObject.layer == EnemiesLayer) {
			GotHit();
		}
	}

	void GotHit(){
		if (invulnerable){
			return;
		}
		
		invulnerable = true;
		Instantiate(PlayerBloodParticles, BloodParticlesSpawnPoint.transform.position, PlayerBloodParticles.transform.rotation);
		currentHealth--;

		UpdateHealthText();

		if (currentHealth <= 0){
			alive = false;
			deathText.enabled = true;
			deathText.text = "GAME OVER: THE FOOD ATE YOU!\nPRESS R TO RESTART";
			return;
		}

		StartCoroutine(RemoveInvulnerability());
	}

	IEnumerator RemoveInvulnerability(){
		yield return new WaitForSeconds(invulnerableTime);
		invulnerable = false;
	}
}
