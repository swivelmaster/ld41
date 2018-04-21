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

	void Start () {
		rb = GetComponent<Rigidbody>();
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
