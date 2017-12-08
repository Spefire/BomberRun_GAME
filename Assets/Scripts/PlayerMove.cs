﻿using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	public float speed = 2.0f;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update () {
		float translation = Input.GetAxis ("Vertical") * speed;
		float straffe = Input.GetAxis ("Horizontal") * speed;
		translation *= Time.deltaTime;
		straffe *= Time.deltaTime;

		transform.Translate (straffe, 0, translation);
	}
} 