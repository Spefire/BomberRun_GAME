﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGame : NetworkBehaviour {

	[Header("Stats variables")]
	public Canvas interf;
	public Text lifeText;
	public Text bombText;
	[SyncVar]
	public int id;
	[SyncVar(hook = "OnChangeLifes")] 
	public int nbLifes = 3;
	[SyncVar(hook = "OnChangeBombs")]
	public int nbBombs = 2;

	[Header("Power variables")]
	public GameObject bombPrefab;
	public float distanceAvaible = 5.0f;
	public GameObject zone;
	[SyncVar]
	private bool canPose;

	//---------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------

	void Start() {
		if (isServer) {
			id = Random.Range (1, 10000);
		}
		if (!isLocalPlayer)	{
			interf.gameObject.SetActive (false);
			return;
		}
		canPose = false;
		interf.worldCamera = Camera.main;
		OnChangeBombs (nbBombs);
		OnChangeLifes (nbLifes);
	}

	void Update () {
		if (!isLocalPlayer)	{
			return;
		}
		DisplayZone ();
		if (canPose && Input.GetMouseButtonDown (0)) {
			CmdBomb (zone.transform.position.x, zone.transform.position.z, id);
		}
	}

	//---------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------

	private void DisplayZone() {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider != null && nbBombs >= 1) {
				float x = Mathf.Round (hit.point.x);
				float y = Mathf.Floor (hit.point.y);
				float z = Mathf.Round (hit.point.z);
				Vector3 target = new Vector3 (x, y, z);
				float dist = Vector3.Distance (this.transform.position, target);
				if (-0.5f < y && y < 0.5f && dist <= distanceAvaible && MapGeneration.currentMap.IsEmpty((int) x, (int) z)) {
					canPose = true;
					zone.transform.position = target;
				} else {
					canPose = false;
					zone.transform.position = new Vector3 (0, -100.0f, 0);
				}
			} else {
				canPose = false;
				zone.transform.position = new Vector3 (0, -100.0f, 0);
			}
		}
	}

	public void GainBomb() {
		if (!isServer) {
			return;
		}
		//Debug.LogError ("GAIN BOMB : " + id);
		nbBombs++;
	}

	public void LooseBomb() {
		if (!isServer) {
			return;
		}
		//Debug.LogError ("LOOSE BOMB : " + id);
		nbBombs--;
	}

	public void LooseLife(){
		nbLifes--;
	}

	private void OnChangeBombs(int nbB){
		//Debug.LogError ("BOMB NB : " + nbB);
		if (nbB > 1) {
			bombText.text = nbB+" Bombes en stock";
		} else if (nbB == 1) {
			bombText.text = nbB+" Bombe en stock";
		} else {
			bombText.text = "Aucune Bombe";
		}
		//Debug.LogError ("BOMB TEXT : " + bombText.text);
	}

	private void OnChangeLifes(int nbL) {
		if (nbL > 1) {
			lifeText.text = nbL+" Vies restantes";
		} else if (nbL == 1) {
			lifeText.text = nbL+" Vie restante";
		} else {
			lifeText.text = "Vous etes mort...";
		}
	}

	[Command]
	void CmdBomb(float x, float z, int id) {
		var bomb = (GameObject) Instantiate (bombPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
		bomb.GetComponent<BombGame> ().SetIDPlayerGame (id);
		//Debug.LogError ("CMD ID : " + id);
		NetworkServer.Spawn (bomb);
	}
} 