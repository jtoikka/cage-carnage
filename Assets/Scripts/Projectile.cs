using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {

  public float speed = 1.0f;

  public GameObject explosion;

  public GameObject hitbox;

  [SyncVar]
  public NetworkInstanceId shooterId;

  public bool isLocalCreated;

  public GameObject shootingPlayer;

  void Awake() {
    if (shootingPlayer) {
      Physics.IgnoreCollision(shootingPlayer.GetComponent<CharacterController>(), GetComponent<CapsuleCollider>());
    } else {
      print("No shooting player");
    }
  }

  public override void OnStartClient() {
    base.OnStartClient();

    if (!shootingPlayer) {
      shootingPlayer = ClientScene.FindLocalObject(shooterId);
    }
    Physics.IgnoreCollision(shootingPlayer.GetComponent<CharacterController>(), GetComponent<CapsuleCollider>());

    if (shootingPlayer.GetComponent<Combat>().isLocalPlayer && !isLocalCreated && !isServer) {
      transform.gameObject.SetActive(false);
    }
  }


  private void OnCollisionEnter(Collision collision) {
    var contact = collision.contacts[0];

    var exp = Instantiate(explosion, contact.point, Quaternion.identity);
    Destroy(exp, 3.0f);

    if (isLocalCreated || isServer && shootingPlayer.GetComponent<Combat>().isLocalPlayer) {
      var hbox = Instantiate(hitbox, contact.point, Quaternion.identity);
      var box = hbox.GetComponent<MissileHitbox>();
      box.shootingPlayer = shootingPlayer.GetComponent<PlayerMovement>();
    }

    if (isServer) {
      Destroy(transform.gameObject, 1.0f);
    }
    transform.gameObject.SetActive(false);
  }
}
