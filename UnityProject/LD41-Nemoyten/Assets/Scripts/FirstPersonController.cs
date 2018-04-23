using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour {

	public AudioSource ShootSound;
	public AudioSource ExplodeSound;

	public AudioSource JumpSound;
	public AudioSource HitSound;
	public AudioSource DeathSound;

	public AudioSource PickUpSound;
	public AudioSource PutDownSound;

	public GameObject feets;
	public LayerMask groundCollision;
	public LayerMask EnemiesLayerMask;

	Rigidbody rb;

	float speed = 5f;
	float jumpVelocity = 5f;

	public Text deathText;

	public bool alive = true;

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
	public int currentHealth = 10;
	public Text HealthText;

	public GameObject PlayerBloodParticles;
	public GameObject PlayerBloodParticlesLooping;
	public GameObject BloodParticlesSpawnPoint;

	float invulnerableTime = 1.5f;
	bool invulnerable = false;

	int EnemiesLayer;
	int EnemyProjectileLayer;

	void Start () {
		rb = GetComponent<Rigidbody>();
		fpsCamera = GetComponentInChildren<Camera>().gameObject;

		GameStateManager.instance.Player = this.gameObject;

		currentHealth = maxHealth;
		UpdateHealthText();

		EnemiesLayer = LayerMask.NameToLayer("Enemies");
		EnemyProjectileLayer = LayerMask.NameToLayer("Enemy Projectiles");
	}

	public void RestoreHealth(){
		currentHealth = 10;
		UpdateHealthText();	
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
			JumpSound.Play();
		}

		if (Input.GetButtonDown("Fire1")){
			if (Time.time - rateOfFire > lastFired){
				//Debug.Log("Shoot " + Time.time.ToString());
				lastFired = Time.time;
				ShootSound.Play();
				GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
				bullet.GetComponent<Bullet>().explodeSound = ExplodeSound;
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

				PutDownSound.Play();
				return;
			}

			RaycastHit hitInfo;
			if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hitInfo, 1.5f, EnemiesLayerMask)){
				//Debug.Log("Picking up enemy");
				PickUpObject(hitInfo.collider.gameObject);
				PickUpSound.Play();
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
			PlayerDeath("GAME OVER: YOU WERE NOT ONE OF THE INGREDIENTS\nPRESS R TO RESTART");
		}
		else if (collider.gameObject == invisibleFloorOfDoom){
			PlayerDeath("GAME OVER: YOU FELL OFF OF THE WORLD. YOU WEREN'T EVEN AT 100% YET\nPRESS R TO RESTART");
		} else if (collider.gameObject.layer == EnemiesLayer) {
			GotHit();
		} else if (collider.gameObject.layer == EnemyProjectileLayer) {
			Destroy(collider.gameObject);
			GotHit();
		}
	}

	public void WrongRecipeDeath(){
		PlayerDeath("GAME OVER: THAT WASN'T ONE OF THE REQUIRED INGREDIENTS!\nNO IMPROVISING ALLOWED, THIS ISN'T JAZZ\nPRESS R TO RESTART");
	}

	public void GameCompleteDeath(){
		PlayerDeath("YOU WIN! CONGRATULATIONS ON SATISFYING MY HUNGER.\nOH, DID I NOT MENTION? YOU ARE DESSERT.", true);
	}

	void PlayerDeath(string text, bool win=false){
		alive = false;
		deathText.enabled = true;
		deathText.text = text;
		if (win){
			Instantiate(PlayerBloodParticlesLooping, BloodParticlesSpawnPoint.transform.position, PlayerBloodParticles.transform.rotation);
		}
		else {
			Instantiate(PlayerBloodParticles, BloodParticlesSpawnPoint.transform.position, PlayerBloodParticles.transform.rotation);
		}

		GetComponentInChildren<MouseLook>().enabled = false;

		DeathSound.Play();
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
			PlayerDeath("GAME OVER: THE FOOD ATE YOU!\nPRESS R TO RESTART");
			return;
		}

		HitSound.Play();
		StartCoroutine(RemoveInvulnerability());
	}

	IEnumerator RemoveInvulnerability(){
		yield return new WaitForSeconds(invulnerableTime);
		invulnerable = false;
	}
}
