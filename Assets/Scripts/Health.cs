using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class Health : NetworkBehaviour {

  [SyncVar]
  public float health = 100.0f;

  [SyncVar]
  public float armor = 0.0f;

  public Text healthText;
  public Text armorText;

  public GameObject damageSound1;
  public GameObject damageSound2;

  public PostProcessingProfile profile;

  public Color healthVignetteColor;
  public Color armorVignetteColor;
  public Color damageVignetteColor;

  private float vignetteSpeed = 1.0f;

  public float shrinkSpeed = 4.0f;

  public bool IsAlive() {
    return health > 0;
  }

  [Command]
  public void CmdAddHealth(float amount) {
    health += amount;

    if (health > 200) {
      health = 200;
    }

    RpcAddHealth(amount);
  }

  [ClientRpc]
  public void RpcAddHealth(float amount) {
    if (isLocalPlayer) {
      var settings = profile.vignette.settings;
      settings.intensity = 0.7f;
      settings.color = healthVignetteColor;

      vignetteSpeed = 0.5f;
      profile.vignette.settings = settings;

      healthText.transform.localScale = new Vector3(2.5f, 2.5f, 1);
    }
  }

  [Command]
  public void CmdResetHealth() {
    health = 100;
    armor = 0;
  }

  [Command]
  public void CmdDecreaseHealth(float amount, int sound) {
    float armorDamage = 0;
    float healthDamage = 0;

    if (armor > 0) {
      var aDamage = amount / 3.0f;
      armorDamage = Mathf.Floor(aDamage);
      healthDamage = Mathf.Ceil(amount - aDamage);

      armor -= armorDamage;

      if (armor < 0) {
        healthDamage -= Mathf.Floor(armor);
        armor = 0;
      }
      health -= healthDamage;
    } else {
      health -= Mathf.Floor(amount);
      healthDamage = Mathf.Floor(amount);
    }
    var died = false;
    if (health <= 0) {
      health = 0;
      died = true;
    }

    var s = sound;

    if (sound == 0) {
      s = (int) Mathf.Round(Random.value + 1);
    }
    if (!died) {
      RpcPlayDamageSound(s, transform.position, healthDamage, armorDamage);
    }

    //if (isLocalPlayer) {
    //  var a = 1.0f + amount / 100.0f * 2.0f;
    //healthText.transform.localScale = new Vector3(a, a, 1);
    //}
    //var a = 1.0f + amount / 100.0f * 2.0f;
    //healthText.transform.localScale = new Vector3(a, a, 1);
  }

  [ClientRpc]
  public void RpcPlayDamageSound(int sound, Vector3 pos, float damageHealth, float damageArmour) {
    if (sound == 1) {
      var s = Instantiate(damageSound1, pos, Quaternion.identity);
      s.transform.parent = transform.GetChild(0).GetChild(0);
      s.transform.localPosition = Vector3.zero;
      var source = s.GetComponent<AudioSource>();

      source.pitch += (Random.value * 0.6f - 0.3f);
    } else {
      var s = Instantiate(damageSound2, pos, Quaternion.identity);
      s.transform.parent = transform.GetChild(0).GetChild(0);
      s.transform.localPosition = Vector3.zero;
      var source = s.GetComponent<AudioSource>();

      source.pitch += (Random.value * 0.6f - 0.3f);
    }
    if (isLocalPlayer) {
      var settings = profile.vignette.settings;
      settings.intensity += damageHealth / 100.0f;
      settings.color = damageVignetteColor;

      profile.vignette.settings = settings;
      vignetteSpeed = 1.0f;

      if (isLocalPlayer) {
        var a = 1.0f + damageHealth / 100.0f * 2.0f;
        healthText.transform.localScale = new Vector3(a, a, 1);

        var b = 1.0f + damageArmour / 100.0f * 2.0f;
        armorText.transform.localScale = new Vector3(b, b, 1);
      }
    }
  }

  [Command]
  public void CmdAddArmor(float amount) {
    armor += amount;

    armorText.transform.localScale = new Vector3(2.5f, 2.5f, 1);

    if (armor > 200) {
      armor = 200;
    }
  }

  [ClientRpc]
  public void RpcAddArmor(float amount) {
    if (isLocalPlayer) {
      var settings = profile.vignette.settings;
      settings.intensity += 0.7f;
      settings.color = armorVignetteColor;

      profile.vignette.settings = settings;
      vignetteSpeed = 0.5f;
    }
  }

  [Command]
  public void CmdResetArmor() {
    armor = 0;
  }

  private float healthTickTimer = 0.0f;

  void Update() {
    if (isServer) {
      if (health > 100.0f) {
        healthTickTimer += Time.deltaTime;
        if (healthTickTimer >= 1) {
          healthTickTimer -= 1;
          health -= 1;
        }
      } else {
        healthTickTimer = 0.0f;
      }
    }
    if (isLocalPlayer) {
      healthText.text = health.ToString();
      armorText.text = armor.ToString();

      var settings = profile.vignette.settings;
      if (settings.intensity > 0) {
        settings.intensity -= Time.deltaTime;
        if (settings.intensity < 0) {
          settings.intensity = 0;
        }
      }

      profile.vignette.settings = settings;

      if (healthText.transform.localScale.y > 1) {
        healthText.transform.localScale -= new Vector3(Time.deltaTime * shrinkSpeed, Time.deltaTime * shrinkSpeed, 0);
        if (healthText.transform.localScale.y < 1) {
          healthText.transform.localScale = new Vector3(1, 1, 1);
        }
      }

      if (armorText.transform.localScale.y > 1) {
        armorText.transform.localScale -= new Vector3(Time.deltaTime * shrinkSpeed, Time.deltaTime * shrinkSpeed, 0);
        if (armorText.transform.localScale.y < 1) {
          armorText.transform.localScale = new Vector3(1, 1, 1);
        }
      }
    }
  }
}
