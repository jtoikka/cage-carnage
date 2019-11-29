using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

  public float afterDuration = 0.0f;

	// Use this for initialization
	void Start () {
		
	}

  private float time = 0.0f;

	// Update is called once per frame
	void Update () {
    time += Time.deltaTime;
    if (time > afterDuration) {
      Destroy(gameObject);
    }
	}
}
