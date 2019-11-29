using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

  public float velocity = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    transform.localRotation *= Quaternion.AngleAxis(velocity * Time.deltaTime, Vector3.up);
	}
}
