using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {

  public Vector3 force;

  public AudioSource trampolineSound; 

  private void OnTriggerEnter(Collider other) {
    var playerMovement = other.GetComponent<PlayerMovement>();
    if (!playerMovement) {
      return;
    }

    playerMovement.Launch(force);
  }
}
