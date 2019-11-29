using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArmorPickup : NetworkBehaviour {

  public float amount = 50.0f;

  private void OnTriggerEnter(Collider other) {
    if (isServer) {
      var health = other.GetComponent<Health>();

      health.CmdAddArmor(amount);
      Destroy(this.gameObject);
    }
  }
}
