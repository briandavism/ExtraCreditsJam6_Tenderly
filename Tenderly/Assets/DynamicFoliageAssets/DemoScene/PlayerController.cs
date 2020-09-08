using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        transform.position += (Vector3.right * Input.GetAxis("Horizontal") * 0.05f) + (Vector3.up * Input.GetAxis("Vertical") * 0.05f);

	}
}
