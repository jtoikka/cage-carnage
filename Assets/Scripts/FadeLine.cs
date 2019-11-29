using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLine : MonoBehaviour {

  public float fadeSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		var line = GetComponent<LineRenderer>();

    line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, 0);

    if (transform.childCount > 0) {
      var light = transform.GetChild(0).GetComponent<Light>();

      var flash = light.transform.GetChild(0);

      flash.gameObject.SetActive(true);

      Destroy(flash.gameObject, 0.1f);

      light.intensity = 5.0f;
    }
  }

  private bool hitMaxFade = false;

  private float deathCounter = 0.0f;

	// Update is called once per frame
	void Update () {
    var line = GetComponent<LineRenderer>();

    line.startColor = new Color(line.startColor.r, line.startColor.g, line.startColor.b, line.startColor.a - Time.deltaTime * fadeSpeed);

    var newAlpha = line.endColor.a;

    if (hitMaxFade) {
      newAlpha -= Time.deltaTime * fadeSpeed * 0.3f;
    } else {
      newAlpha += Time.deltaTime * fadeSpeed;
    }
    line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, newAlpha);


    if (line.endColor.a >= 1.0f) {
      hitMaxFade = true;
    }

    if (transform.childCount > 0) {
      var light = transform.GetChild(0).GetComponent<Light>();
      light.intensity -= Time.deltaTime * 50.0f;
    }

    if (newAlpha < 0) {
      deathCounter += Time.deltaTime;
      //if (deathCounter > 2.0f) {
        Destroy(gameObject, 2.0f);
      //}
    }
	}
}
