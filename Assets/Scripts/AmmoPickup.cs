using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AmmoPickup : NetworkBehaviour {

  public int amount = 10;

  public Weapon weapon;

  private void OnTriggerEnter(Collider other) {
    if (isServer) {
      var combat = other.GetComponent<Combat>();

      combat.CmdAddAmmo(weapon, amount);
      Destroy(this.gameObject);
    }
  }
}
