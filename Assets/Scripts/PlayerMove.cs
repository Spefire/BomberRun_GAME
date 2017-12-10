﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMove : NetworkBehaviour {

	public float speed = 2.0f;

	void Start() {
		if (!isLocalPlayer)	{
			return;
		}
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update () {
		if (!isLocalPlayer)	{
			return;
		}
		float translation = Input.GetAxis ("Vertical") * speed;
		float straffe = Input.GetAxis ("Horizontal") * speed;
		translation *= Time.deltaTime;
		straffe *= Time.deltaTime;

		transform.Translate (straffe, 0, translation);
	}
} 