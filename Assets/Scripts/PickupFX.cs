using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFX : MonoBehaviour {

  public float spinRate = 60.0f;
  public float floatRate = 1.0f;
  public float amplitude = 0.2f;

  public AudioSource pickupSound;

  private Vector3 startPos;

  private float waveTimer = 0.0f;

  // Use this for initialization
  void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
    transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * spinRate, Vector3.up);
    transform.position = startPos + new Vector3(0, Mathf.Sin(waveTimer) * amplitude, 0);
    waveTimer += Time.deltaTime * floatRate;
	}

  public void OnTriggerEnter(Collider other) {
    var sound = GameObject.Instantiate(pickupSound, transform.position, Quaternion.identity);

    Destroy(sound, sound.clip.length);
  }
}
