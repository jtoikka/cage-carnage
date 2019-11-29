using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPickup : NetworkBehaviour {

  public float amount = 30.0f;

  private void OnTriggerEnter(Collider other) {
    if (isServer) {
      var health = other.GetComponent<Health>();

      health.CmdAddHealth(amount);
      Destroy(this.gameObject);
    }
  }
}
