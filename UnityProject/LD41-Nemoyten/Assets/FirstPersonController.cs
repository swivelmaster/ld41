using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour {

	public GameObject feets;
	public LayerMask groundCollision;

	Rigidbody rb;

	float speed = 5f;
	float jumpVelocity = 5f;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		float move = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
		float strafe = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

		rb.transform.Translate(new Vector3(strafe, 0, move));

		if (Input.GetButtonDown("Jump") && Physics.Raycast(feets.transform.position, Vector3.down, .2f, groundCollision)){
			Debug.Log("Jumping");
			rb.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
		}
	}
}
