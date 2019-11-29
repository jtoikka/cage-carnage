using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletAnim : MonoBehaviour {

  public float targetSpeed = 2.0f;
  public float accelerationRate = 1.0f;

  private Transform wheel;

  private Light light;

  private AudioSource sound;

	// Use this for initialization
	void Start () {
    wheel = transform.GetChild(1);
    light = wheel.GetChild(0).GetComponent<Light>();
    sound = wheel.GetChild(1).GetComponent<AudioSource>();
	}

  public bool isTurnedOn = false;
	
	// Update is called once per frame
	void Update () {
    if (isTurnedOn) {
      speed += accelerationRate * Time.deltaTime;
      if (speed > targetSpeed) {
        speed = targetSpeed;
      }
    } else {
      speed -= accelerationRate * Time.deltaTime;
      if (speed < 0) {
        speed = 0;
      }
    }

    light.intensity = speed / targetSpeed;

    sound.volume = speed / targetSpeed;

    wheel.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime * 10.0f, Vector3.up);
	}

  private float speed = 0.0f;

  public void TurnOn() {
    isTurnedOn = true;
  }

  public void TurnOff() {
    isTurnedOn = false;
  }
}
