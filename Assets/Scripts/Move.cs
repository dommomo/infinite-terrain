using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public float speed = 1;
    public float fastSpeed = 3;
    public KeyCode EnableFastSpeed = KeyCode.LeftShift;
    public Transform turnWithMovement;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var currentSpeed = speed;
        if (Input.GetKey(EnableFastSpeed)) currentSpeed = fastSpeed;
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.Translate(movement * currentSpeed * Time.deltaTime);

        if (movement.x > 0.1f || movement.x < -0.1f || movement.y > 0.1f || movement.y < -0.1f)
        { 
            turnWithMovement.rotation = Quaternion.LookRotation(Vector3.back, movement.normalized);
        }
    }
}
