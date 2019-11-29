using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour {

  public PlayerMovement player;

  private int numColliding = 0;

  private void OnTriggerEnter(Collider other) {
    player.isGrounded = true;
    numColliding += 1;
  }

  private void OnTriggerExit(Collider other) {
    numColliding -= 1;
    if (numColliding == 0) {
      player.isGrounded = false;
    }
  }
}
