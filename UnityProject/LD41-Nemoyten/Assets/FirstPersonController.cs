using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour {

	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		float move = Input.GetAxisRaw("Vertical");
		float strafe = Input.GetAxisRaw("Horizontal");

		rb.AddForce(move, 0, strafe);
	}
}
