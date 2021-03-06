﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveInside : MonoBehaviour {

    public float speed = 1;
    public float fastSpeed = 3;
    public KeyCode EnableFastSpeed = KeyCode.LeftShift;
    public Transform turnWithMovement;
    public BuildingInterior Building;
    
    private Vector3 previousPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //walk or run
        var currentSpeed = speed;
        if (Input.GetKey(EnableFastSpeed)) currentSpeed = fastSpeed;
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        //turn player icon
        if (movement.x > 0.1f || movement.x < -0.1f || movement.y > 0.1f || movement.y < -0.1f)
        { 
            turnWithMovement.rotation = Quaternion.LookRotation(Vector3.back, movement.normalized);
        }

        //check if blocked tile
        Vector3 currentPos = transform.position;
        var player = new Rect(currentPos, Vector2.one);
        if (Building.IsBlocked(player))
        {
            transform.position = currentPos = previousPosition;
        }
        if (Building.IsExiting(player))
        {
            var go = new GameObject();
            GameObject.DontDestroyOnLoad(go);
            var starter = go.AddComponent<TerrainStarter>();
            starter.PreviousPosition = Building.PreviousPosition;
            SceneManager.LoadScene(Building.SceneIdForTerrainView);
        }
        previousPosition = currentPos;
    }
}
