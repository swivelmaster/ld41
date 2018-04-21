using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is NOT something I know how to do on my own
// So credit where credit is due:
// https://www.youtube.com/watch?v=blO039OzUZc&list=PLYnfu9d0mzDuo5bX74Vleq8p9YxCz5ZmO&t=0s&index=6
public class MouseLook : MonoBehaviour {

	Vector2 mouseLook;
	Vector2 smoothV;
	float sensitivity = 3.0f;
	float smoothing = 2.0f;

	GameObject parentGo;

	bool cursorLocked = true;

	void Start () {
		parentGo = transform.parent.gameObject;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void Update () {

		if (!cursorLocked){
			if (Input.GetKeyDown("escape")){
				Cursor.lockState = CursorLockMode.None;
				cursorLocked = true;
			}
			return;
		}

		Vector2 look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		look = Vector2.Scale(look, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		smoothV.x = Mathf.Lerp(smoothV.x, look.x, 1f / smoothing);
		smoothV.y = Mathf.Lerp(smoothV.y, look.y, 1f / smoothing);

		mouseLook += smoothV;

		transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
		parentGo.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, parentGo.transform.up);

		if (Input.GetKeyDown("escape")){
			Cursor.lockState = CursorLockMode.Locked;
			cursorLocked = false;
		}
	}
}
