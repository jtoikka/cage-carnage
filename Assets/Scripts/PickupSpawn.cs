using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupSpawn : NetworkBehaviour {

  public GameObject spawnItem;

  public float cooldownTime = 2.0f;

  private float cooldownTimer = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (transform.childCount == 0) {
      if (cooldownTimer <= 0) {
        SpawnItem();
        cooldownTimer = cooldownTime;
      } else {
        cooldownTimer -= Time.deltaTime;
      }
    }
	}

  //[Command]
  private void SpawnItem() {
    var item = Instantiate(spawnItem, transform.position + spawnItem.transform.position, Quaternion.identity);
    item.transform.SetParent(this.transform);

    NetworkServer.Spawn(item);
  }
}
