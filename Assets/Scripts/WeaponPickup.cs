using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponPickup : NetworkBehaviour {

  public int ammoAmount = 10;

  public Weapon weapon;

  private void OnTriggerEnter(Collider other) {
    if (isServer) {
      var health = other.GetComponent<Health>();

      if (!health || !health.IsAlive()) {
        return;
      }
      var combat = other.GetComponent<Combat>();

      combat.CmdAddAmmo(weapon, ammoAmount);
      combat.CmdAddWeapon(weapon);
      Destroy(this.gameObject);
    }
  }
}
