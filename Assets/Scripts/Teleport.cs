using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Teleport : NetworkBehaviour {

  public Teleport link;

  public float exitOffsetDistance = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  private void OnTriggerEnter(Collider other) {
    if (!isClient) {
      return;
    }
    var mov = other.GetComponent<PlayerMovement>();

    if (mov) {
      print("Move!");
      other.transform.position = link.transform.position + link.transform.forward * exitOffsetDistance;
      other.transform.rotation = Quaternion.LookRotation(link.transform.forward, Vector3.up);
      mov.RotateVelocity(link.transform.forward);
    }

  }
}
