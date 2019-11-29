using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileHitbox : MonoBehaviour {

  public PlayerMovement shootingPlayer;

  public float damage = 120.0f;

  private ScoreBoard scoreBoard;

	void Start () {
    scoreBoard = GameObject.Find("Score Board").GetComponent<ScoreBoard>();
	}
	
	// Update is called once per frame
	void Update () {
    Destroy(transform.gameObject, 0.1f);
	}

  private void OnTriggerEnter(Collider other) {
    var closestPoint = other.ClosestPoint(transform.position);
    var dist = (closestPoint - transform.position).magnitude;
    var dir = (closestPoint - transform.position).normalized;

    var collider = transform.GetComponent<SphereCollider>();

    var radius = collider.radius;

    var force = (radius - dist) / radius;

    if (force < 0) {
      return;
    }
    var shotForce = force * dir * 15.0f;

    var otherPlayer = other.GetComponent<PlayerMovement>();

    if (otherPlayer == shootingPlayer) {
      shootingPlayer.recoil(force * dir * 20.0f);

      var health = shootingPlayer.GetComponent<Health>();

      if (health.IsAlive()) {
        var combat = other.GetComponent<Combat>();
        combat.CmdSetKillMessage(combat.netId, "self", "poop");
      }
      health.CmdDecreaseHealth(Mathf.Min(50.0f, 100 * force * 0.4f), 0);
    } else {
      if (other.GetComponent<Health>().IsAlive()) {
        var combat = other.GetComponent<Combat>();
        shootingPlayer.GetComponent<Combat>().CmdSetKillMessage(combat.netId, shootingPlayer.name, "poop");
      }
      shootingPlayer.CmdShootPlayer(other.gameObject, shotForce, damage * force);
    }
  }

}
