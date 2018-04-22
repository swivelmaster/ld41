using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour {

	public GameObject feets;
	public LayerMask groundCollision;

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

	void Start () {
		rb = GetComponent<Rigidbody>();
		fpsCamera = GetComponentInChildren<Camera>().gameObject;

		GameStateManager.instance.Player = this.gameObject;
	}

	void Update () {
		if (!alive){
			return;
		}

		float move = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
		float strafe = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

		rb.transform.Translate(new Vector3(strafe, 0, move));

		if (Input.GetButtonDown("Jump") && Physics.Raycast(feets.transform.position, Vector3.down, .2f, groundCollision)){
			Debug.Log("Jumping");
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
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject == stew){
			alive = false;
			deathText.enabled = true;
			deathText.text = "GAME OVER: YOU WERE NOT ONE OF THE INGREDIENTS";
		}
		else if (collider.gameObject == invisibleFloorOfDoom){
			alive = false;
			deathText.enabled = true;
			deathText.text = "GAME OVER: YOU FELL OFF OF THE WORLD. YOU WEREN'T EVEN AT 100% YET";
		}
	}
}
