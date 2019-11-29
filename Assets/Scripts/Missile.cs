using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Missile : NetworkBehaviour {

  public GameObject explosion;
  public GameObject hitbox;

  public bool isLocalCreated = false;

  public PlayerMovement shootingPlayer;

  [SyncVar]
  public NetworkInstanceId shooterId;

  void Awake() {
    if (shootingPlayer) {
      Physics.IgnoreCollision(shootingPlayer.GetComponent<CharacterController>(), GetComponent<CapsuleCollider>());
    }
  }


  public override void OnStartClient() {
    base.OnStartClient();

    if (!shootingPlayer) {
      var dude = ClientScene.FindLocalObject(shooterId);

      shootingPlayer = dude.GetComponent<PlayerMovement>();
    }
  }

  private void OnCollisionEnter(Collision collision) {
    var contact = collision.contacts[0];

    var exp = Instantiate(explosion, contact.point, Quaternion.identity);
    Destroy(exp, 3.0f);

    if (isLocalCreated) {
      var hbox = Instantiate(hitbox, contact.point, Quaternion.identity);
      var box = hbox.GetComponent<MissileHitbox>();
      box.shootingPlayer = shootingPlayer;
    }

    if (isServer) {
      Destroy(transform.gameObject, 1.0f);
    }
    transform.gameObject.SetActive(false);
  }
}
