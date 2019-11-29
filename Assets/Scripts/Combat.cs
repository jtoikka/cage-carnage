using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Combat : NetworkBehaviour {
  public int rocketAmmo = 0;

  public int machineGunAmmo = 100;

  public int shotgunAmmo = 0;

  public int railgunAmmo = 0;

  public Transform gunHolder;
  public Transform gunHolderOther;

  public GameObject missile;

  public GameObject machineGunShot;

  public GameObject shotgunShot;

  public GameObject railgunShot;

  public GameObject shotgunPickup;
  public GameObject rocketPickup;
  public GameObject railgunPickup;

  public Weapon weapon = Weapon.Rocket;

  [SyncVar]
  public bool hasMachineGun = true;

  [SyncVar]
  public bool hasRocketLauncher = false;

  [SyncVar]
  public bool hasShotgun = false;

  [SyncVar]
  public bool hasRailgun = false;

  public Text ammoText;

  public Text deathText;

  [SyncVar]
  public string hitByPlayer;

  [SyncVar]
  public string hitByWeapon;

  [SyncVar]
  public NetworkInstanceId killerId;

  public GameObject waterHit;
  public GameObject bloodHit;
  public GameObject miss;

  public Transform gunBone;

  private GameObject machineGun;

  private GameObject rocketLauncher;

  private GameObject shotgun;

  private GameObject gauntlet;

  private GameObject railgun;

  private List<GameObject> weapons = new List<GameObject>();

  public GameObject machineGunAmmoBox;
  public GameObject shotgunAmmoBox;
  public GameObject rocketAmmoBox;
  public GameObject railgunAmmoBox;

  [Command]
  public void CmdAddAmmo(Weapon weapon, int amount) {
    RpcAddAmmo(weapon, amount);
  }

  [ClientRpc]
  public void RpcAddAmmo(Weapon weapon, int amount) {
    if (isLocalPlayer) {
      switch (weapon) {
        case Weapon.Rocket:
          rocketAmmo += amount;
          if (rocketAmmo > 50) {
            rocketAmmo = 50;
          }
          break;
        case Weapon.MachineGun:
          machineGunAmmo += amount;
          if (machineGunAmmo > 200) {
            machineGunAmmo = 200;
          }
          break;
        case Weapon.Railgun:
          railgunAmmo += amount;
          if (railgunAmmo > 25) {
            railgunAmmo = 25;
          }
          break;
        case Weapon.Shotgun:
          shotgunAmmo += amount;
          if (shotgunAmmo > 25) {
            shotgunAmmo = 25;
          }
          break;
      }
    }
  }

  [Command]
  public void CmdAddWeapon(Weapon w) {
    RpcSetWeapon(w);
  }

  [ClientRpc]
  public void RpcSetWeapon(Weapon w) {
    if (w != weapon) {
      GetComponent<PlayerMovement>().SetSwappingWeapon();
    }
    weapon = w;
    switch (w) {
      case Weapon.Rocket:
        hasRocketLauncher = true;
        break;
      case Weapon.MachineGun:
        hasMachineGun = true;
        break;
      case Weapon.Railgun:
        hasRailgun = true;
        break;
      case Weapon.Shotgun:
        hasShotgun = true;
        break;
    }
  }

  public void Reset() {
    CmdSpawnDrop(weapon, transform.position + transform.forward);

    rocketAmmo = 0;
    machineGunAmmo = 100;
    shotgunAmmo = 0;
    railgunAmmo = 0;

    hasRocketLauncher = false;
    hasRailgun = false;
    hasShotgun = false;
    hasMachineGun = true;

    weapon = Weapon.MachineGun;
    CmdSetWeapon(weapon);
  }

  [Command]
  public void CmdSpawnDrop(Weapon weapon, Vector3 position) {
    GameObject item;
    switch(weapon) {
      case Weapon.Rocket:
        item = Instantiate(rocketPickup, position, Quaternion.identity);
        NetworkServer.Spawn(item);
        break;
      case Weapon.Shotgun:
        item = Instantiate(shotgunPickup, position, Quaternion.identity);
        NetworkServer.Spawn(item);
        break;
      case Weapon.Railgun:
        item = Instantiate(railgunPickup, position, Quaternion.identity);
        NetworkServer.Spawn(item);
        break;
      default:
        break;
    }
  }

	// Use this for initialization?
	public override void OnStartClient() {
    base.OnStartClient();
	}

  public void Start()
  {
    print("Start!");
    //if (isLocalPlayer) {
    //  print("Is local player");
    //  machineGun = gunHolder.GetChild(1).gameObject;
    //  rocketLauncher = gunHolder.GetChild(2).gameObject;
    //  shotgun = gunHolder.GetChild(3).gameObject;
    //  railgun = gunHolder.GetChild(4).gameObject;
    //  gauntlet = gunHolder.GetChild(5).gameObject;
    //} else {
    //  print("Is not?");
    //  machineGun = gunHolderOther.GetChild(0).gameObject;
    //  rocketLauncher = gunHolderOther.GetChild(1).gameObject;
    //  shotgun = gunHolderOther.GetChild(2).gameObject;
    //  railgun = gunHolderOther.GetChild(3).gameObject;
    //  gauntlet = gunHolderOther.GetChild(4).gameObject;
    //  gunHolderOther.gameObject.SetActive(false);
    //}

    //weapons.Add(machineGun);
    //weapons.Add(shotgun);
    //weapons.Add(rocketLauncher);
    //weapons.Add(railgun);
    //weapons.Add(gauntlet);
  }

  private bool shootingReset = true;

  private bool lockChangeweapon = false;

  private float cooldown = 0.0f;

  private bool resetDeath = false;

  private bool isInitialized;

  bool forceChange = false;

	// Update is called once per frame
	void FixedUpdate () {
    if (!isInitialized) {
      if (isLocalPlayer) {
        print("Is local player");
        machineGun = gunHolder.GetChild(1).gameObject;
        rocketLauncher = gunHolder.GetChild(2).gameObject;
        shotgun = gunHolder.GetChild(3).gameObject;
        railgun = gunHolder.GetChild(4).gameObject;
        gauntlet = gunHolder.GetChild(5).gameObject;
        gunHolderOther.gameObject.SetActive(false);

        transform.position += Vector3.up;
      } else {
        print("Is not?");
        machineGun = gunHolderOther.GetChild(0).gameObject;
        rocketLauncher = gunHolderOther.GetChild(1).gameObject;
        shotgun = gunHolderOther.GetChild(2).gameObject;
        railgun = gunHolderOther.GetChild(3).gameObject;
        gauntlet = gunHolderOther.GetChild(4).gameObject;
        gunHolderOther.gameObject.SetActive(true);
        gunHolder.gameObject.SetActive(false);
      }

      weapons.Add(machineGun);
      weapons.Add(shotgun);
      weapons.Add(rocketLauncher);
      weapons.Add(railgun);
      weapons.Add(gauntlet);
      isInitialized = true;
    }
    if (!isLocalPlayer) {
      gunBone = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
      foreach (var w in weapons) {
        switch (weapon) {
          case Weapon.MachineGun:
            if (w == machineGun) {
              w.SetActive(true);
            } else {
              w.SetActive(false);
            }
            break;
          case Weapon.Rocket:
            if (w == rocketLauncher) {
              w.SetActive(true);
            } else {
              w.SetActive(false);
            }
            break;
          case Weapon.Shotgun:
            if (w == shotgun) {
              w.SetActive(true);
            } else {
              w.SetActive(false);
            }
            break;
          case Weapon.Railgun:
            if (w == railgun) {
              w.SetActive(true);
            } else {
              w.SetActive(false);
            }
            break;
          case Weapon.Gauntlet:
            if (w == gauntlet) {
              w.SetActive(true);
            } else {
              w.SetActive(false);
            }
            break;
          default:
            break;
        }
      }

      return;
    }
    cooldown -= Time.fixedDeltaTime;

    ammoText.text = Ammo().ToString();

    var activateMachineGunAmmo = false;
    var activateShotgunAmmo = false;
    var activateRocketAmmo = false;
    var activateRailgunAmmo = false;

    switch (weapon) {
      case Weapon.MachineGun:
        activateMachineGunAmmo = true;
        break;
      case Weapon.Rocket:
        activateRocketAmmo = true;
        break;
      case Weapon.Shotgun:
        activateShotgunAmmo = true;
        break;
      case Weapon.Railgun:
        activateRailgunAmmo = true;
        break;
      case Weapon.Gauntlet:
        break;
      default:
        break;
    }

    if (activateMachineGunAmmo) {
      if (!machineGunAmmoBox.activeInHierarchy) {
        machineGunAmmoBox.SetActive(true);
      }
    } else {
      machineGunAmmoBox.SetActive(false);
    }
    if (activateRocketAmmo) {
      if (!rocketAmmoBox.activeInHierarchy) {
        rocketAmmoBox.SetActive(true);
      }
    } else {
      rocketAmmoBox.SetActive(false);
    }
    if (activateShotgunAmmo) {
      if (!shotgunAmmoBox.activeInHierarchy) {
        shotgunAmmoBox.SetActive(true);
      }
    } else {
      shotgunAmmoBox.SetActive(false);
    }
    if (activateRailgunAmmo) {
      if (!railgunAmmoBox.activeInHierarchy) {
        railgunAmmoBox.SetActive(true);
      }
    } else {
      railgunAmmoBox.SetActive(false);
    }


    foreach (var w in weapons) {
      switch (weapon) {
        case Weapon.MachineGun:
          if (w == machineGun) {
            w.SetActive(true);
          } else {
            w.SetActive(false);
          }
          break;
        case Weapon.Rocket:
          if (w == rocketLauncher) {
            w.SetActive(true);
          } else {
            w.SetActive(false);
          }
          break;
        case Weapon.Shotgun:
          if (w == shotgun) {
            w.SetActive(true);
          } else {
            w.SetActive(false);
          }
          break;
        case Weapon.Railgun:
          if (w == railgun) {
            w.SetActive(true);
          } else {
            w.SetActive(false);
          }
          break;
        case Weapon.Gauntlet:
          if (w == gauntlet) {
            w.SetActive(true);
          } else {
            w.SetActive(false);
          }
          break;
        default:
          break;
      }
    }

    var health = GetComponent<Health>();

    var scroll = Input.GetAxis("Change Weapon");

    if (scroll != 0 && health.IsAlive()) {
      if (!lockChangeweapon) {
        if (scroll > 0) {
          setNextWeapon();
        } else {
          setPreviousWeapon();
        }
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
      }
      lockChangeweapon = true;
    } else {
      lockChangeweapon = false;
    }

    var meleeButton = Input.GetAxis("Melee");

    if (meleeButton > 0.0f) {
      setMelee();
      GetComponent<PlayerMovement>().SetSwappingWeapon();
      cooldown = 0.3f;
      CmdSetWeapon(weapon);
    }
    if (Input.GetAxis("Machine Gun") > 0) {
      if (hasMachineGun) {
        weapon = Weapon.MachineGun;
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
        CmdSetWeapon(weapon);
        if (Ammo() == 0) {
          forceChange = true;
        }
      }
    }
    if (Input.GetAxis("Shotgun") > 0) {
      if (hasShotgun) {
        weapon = Weapon.Shotgun;
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
        CmdSetWeapon(weapon);
        if (Ammo() == 0) {
          forceChange = true;
        }
      }
    }
    if (Input.GetAxis("Rocket Launcher") > 0) {
      if (hasRocketLauncher) {
        weapon = Weapon.Rocket;
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
        CmdSetWeapon(weapon);
        if (Ammo() == 0) {
          forceChange = true;
        }
      }
    }
    if (Input.GetAxis("Railgun") > 0) {
      if (hasRailgun) {
        weapon = Weapon.Railgun;
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
        CmdSetWeapon(weapon);
        if (Ammo() == 0) {
          forceChange = true;
        }
      }
    }
    if (Ammo() > 0) {
      forceChange = false;
    }

		var shootButton = Input.GetAxis("Fire1");

    if (!health.IsAlive() && !resetDeath) {
      string t = "";
      switch (hitByWeapon) {
        case "poop":
          if (hitByPlayer == "self") {
            t = "You pooped yourself";
          } else {
            t = hitByPlayer + " buried you in poop";
          }
          break;
        case "water gun":
          t = hitByPlayer + " made you wet";
          break;
        case "tommy gun":
          t = hitByPlayer + " lit you up";
          break;
        case "shotgun":
          t = "Blasted by " + hitByPlayer;
          break;
        case "hamster wheel":
          t = hitByPlayer + " tore you to pieces";
          break;
        case "floor":
          t = "Splat goes the hamster";
          break;
      }
      deathText.text = t;
      shootButton = 0;
      if (killerId != netId) {
        GetComponent<PlayerMovement>().CmdDied(killerId);
      } else {
        GetComponent<PlayerMovement>().CmdDiedSelf();
      }
      resetDeath = true;
    } else if (health.IsAlive()) {
      resetDeath = false;
      deathText.text = "";
    }

    if (shootButton > 0 && shootingReset && health.IsAlive()) {
      Fire();

      switch(weapon) {
        case Weapon.Rocket:
          cooldown = 0.8f;
          break;
        case Weapon.MachineGun:
          cooldown = 0.1f;
          break;
        case Weapon.Railgun:
          cooldown = 1.5f;
          break;
        case Weapon.Shotgun:
          cooldown = 1.0f;
          break;
        case Weapon.Gauntlet:
          cooldown = 0.4f;
          break;
      }
      if (Ammo() == 0 && weapon != Weapon.Gauntlet) {
        cooldown = 0.1f;
      }
    } else if (shootButton == 0) {
      gauntletAnim.TurnOff();
      CmdDisableGauntlet();
      if (Ammo() <= 0 && weapon != Weapon.Gauntlet && !forceChange) {
        setPreviousWeapon();
        GetComponent<PlayerMovement>().SetSwappingWeapon();
        cooldown = 0.3f;
      } 
    }

    if ((shootButton == 0 && cooldown <= 0) || (weapon == Weapon.MachineGun && cooldown <= 0) || (weapon == Weapon.Gauntlet)) {
      shootingReset = true;
    } else {
      shootingReset = false;
    }


	}

  private int weaponCycleCount = 0;

  void setNextWeapon() {
    weaponCycleCount += 1;

    var hasWeapon = false;
    if (weapon == Weapon.Rocket) {
      weapon = Weapon.Railgun;
      hasWeapon = hasRailgun;
    } else if (weapon == Weapon.MachineGun) {
      weapon = Weapon.Shotgun;
      hasWeapon = hasShotgun;
    } else if (weapon == Weapon.Shotgun) {
      weapon = Weapon.Rocket;
      hasWeapon = hasRocketLauncher;
    } else {
      weapon = Weapon.MachineGun;
      hasWeapon = hasMachineGun;
    }

    if (Ammo() == 0 || !hasWeapon) {
      if (weaponCycleCount >= 5) {
        weapon = Weapon.Gauntlet;
        weaponCycleCount = 0;
      } else {
        setNextWeapon();
      }
    } else {
      weaponCycleCount = 0;
    }
    CmdSetWeapon(weapon);
  }

  int Ammo() {
    if (weapon == Weapon.Rocket) {
      return rocketAmmo;
    }
    if (weapon == Weapon.MachineGun) {
      return machineGunAmmo;
    }
    if (weapon == Weapon.Railgun) {
      return railgunAmmo;
    }
    if (weapon == Weapon.Shotgun) {
      return shotgunAmmo;
    }
    return 0;
  }

  void setPreviousWeapon() {
    weaponCycleCount += 1;
    if (weapon == Weapon.Rocket) {
      weapon = Weapon.Shotgun;
    } else if (weapon == Weapon.Railgun) {
      weapon = Weapon.Rocket;
    } else if (weapon == Weapon.Shotgun){
      weapon = Weapon.MachineGun;
    } else {
      weapon = Weapon.Railgun;
    }

    if (Ammo() == 0) {
      if (weaponCycleCount >= 5) {
        weapon = Weapon.Gauntlet;
        weaponCycleCount = 0;
      } else {
        setPreviousWeapon();
      }
    } else {
      weaponCycleCount = 0;
    }
    CmdSetWeapon(weapon);
  }

  void setMelee() {
    weapon = Weapon.Gauntlet;
  }

  void Fire() {
    var pos = gunHolder.position + gunHolder.rotation * Vector3.forward * 1.0f;
    var rot = gunHolder.rotation * missile.transform.rotation;

    GetComponent<PlayerMovement>().SetShooting();

    switch (weapon) {
      case Weapon.Gauntlet:
        LocalFireGauntlet();
        break;
      case Weapon.Rocket:
        if (rocketAmmo > 0) {
          CmdFireProjectile(pos, rot, gunHolder.rotation, Weapon.Rocket, netId);
          LocalFireProjectile(pos, rot, gunHolder.rotation, missile);
          rocketAmmo -= 1;
        } else {
          CmdPlayClickSound(gunHolder.position);
          playClickSound(gunHolder.position);
        }
        break;
      case Weapon.MachineGun:
        if (machineGunAmmo > 0) {
          var dirOffset = new Vector3(Random.value * 2.0f - 1.0f, Random.value * 2.0f - 1.0f, 50.0f).normalized * Random.value;

          var rngDir = Quaternion.LookRotation(dirOffset);

          CmdFireInstant(pos, gunHolder.rotation * rngDir * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), weapon);
          LocalFireInstant(pos, gunHolder.rotation * rngDir * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), 7.0f);

          machineGunAmmo -= 1;
        } else {
          CmdPlayClickSound(gunHolder.position);
          playClickSound(gunHolder.position);
        }
        break;
      case Weapon.Shotgun:
        if (shotgunAmmo > 0) {
          for (int i = 0; i < 11; i++) {
            var dirOffset = new Vector3(Random.value * 2.0f - 1.0f, Random.value * 2.0f - 1.0f, 10.0f).normalized * Random.value;

            var rngDir = Quaternion.LookRotation(dirOffset);

            CmdFireInstant(pos, gunHolder.rotation * rngDir * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), weapon);
            LocalFireInstant(pos, gunHolder.rotation * rngDir * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), 10.0f);
          }
          shotgunAmmo -= 1;
        } else {
          CmdPlayClickSound(gunHolder.position);
          playClickSound(gunHolder.position);
        }
        break;
      case Weapon.Railgun:
        if (railgunAmmo > 0) {
          CmdFireInstant(pos, gunHolder.rotation * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), weapon);
          LocalFireInstant(pos, gunHolder.rotation * Vector3.forward, gunHolder.rotation * new Vector3(0.29f, -0.27f, 0), 100.0f);
          railgunAmmo -= 1;
        } else {
          CmdPlayClickSound(gunHolder.position);
          playClickSound(gunHolder.position);
        }
        break;
    }
  }

  [Command]
  public void CmdPlayClickSound(Vector3 pos) {
    RpcPlayClickSound(pos);
  }

  [ClientRpc]
  public void RpcPlayClickSound(Vector3 pos) {
    if (!isLocalPlayer) playClickSound(pos);
  }

  public GameObject clickSound;

  public void playClickSound(Vector3 pos) {
    Instantiate(clickSound, pos, Quaternion.identity);
  }

  [Command]
  public void CmdFireProjectile(Vector3 position, Quaternion projectileRotation, Quaternion projectileDirection, Weapon weaponType, NetworkInstanceId id) {
    RpcFireProjectile(position, projectileRotation, projectileDirection, weaponType, id);
  }

  [ClientRpc]
  public void RpcFireProjectile(Vector3 position, Quaternion projectileRotation, Quaternion projectileDirection, Weapon weaponType, NetworkInstanceId id) {
    if (isLocalPlayer) {
      return;
    }

    var projectilePrefab = missile;

    projectilePrefab.GetComponent<Projectile>().shootingPlayer = null;
    GameObject projectileInstance = Instantiate(projectilePrefab, position, projectileRotation);

    var rb = projectileInstance.GetComponent<Rigidbody>();
    var properties = projectileInstance.GetComponent<Projectile>();
    rb.velocity = projectileDirection * Vector3.forward * properties.speed;

    properties.shooterId = id;
  }

  [ClientRpc]
  public void RpcInstant(Vector3 position, Vector3 direction, Vector3 gunOffset, Weapon w) {
    if (isLocalPlayer) {
      return;
    }

    var ray = new Ray(position + direction, direction);

    RaycastHit hit;

    Physics.Raycast(ray, out hit);

    var gunToPoint = hit.point - (position + gunOffset);

    var quat = Quaternion.LookRotation(gunToPoint.normalized);

    GameObject ob;
    if (w == Weapon.MachineGun) {
      ob = machineGunShot;
      if (hit.collider.GetComponent<Health>()) {
        Instantiate(bloodHit, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      } else {
        Instantiate(miss, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      }
    } else if (weapon == Weapon.Shotgun) {
      ob = shotgunShot;
      if (hit.collider.GetComponent<Health>()) {
        Instantiate(bloodHit, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      } else {
        Instantiate(miss, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      }
    } else {
      ob = railgunShot;
      Instantiate(waterHit, hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));

    }

    var line = Instantiate(ob, position + gunOffset, quat);

    line.transform.localScale = new Vector3(1, 1, gunToPoint.magnitude);
  }


  public void LocalFireInstant(Vector3 position, Vector3 direction, Vector3 gunOffset, float damage) {
    var ray = new Ray(position, direction);

    RaycastHit hit;

    Physics.Raycast(ray, out hit);

    var gunToPoint = hit.point - (position + gunOffset);

    var quat = Quaternion.LookRotation(gunToPoint.normalized);

    GameObject ob;

    string name;
    if (weapon == Weapon.MachineGun) {
      ob = machineGunShot;
      name = "tommy gun";
      if (hit.collider.GetComponent<Health>()) {
        Instantiate(bloodHit, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      } else {
        Instantiate(miss, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      }
    } else if (weapon == Weapon.Shotgun) {
      ob = shotgunShot;
      name = "shotgun";
      if (hit.collider.GetComponent<Health>()) {
        Instantiate(bloodHit, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      } else {
        Instantiate(miss, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
      }
    } else {
      ob = railgunShot;
      name = "water gun";
      Instantiate(waterHit, hit.point, Quaternion.LookRotation(-gunToPoint.normalized, Vector3.up) * Quaternion.AngleAxis(Random.value * 360, Vector3.forward));
    }

    var line = Instantiate(ob, position + gunOffset, quat);

    line.transform.localScale = new Vector3(1, 1, gunToPoint.magnitude);

    var health = hit.collider.GetComponent<Health>();

    if (health) {
      if (health.IsAlive()) {
        CmdSetKillMessage(health.netId, transform.name, name);
      }
      CmdDecreaseHealth(hit.collider.gameObject, damage);
    }
  }

  public GauntletAnim gauntletAnim;

  public GauntletAnim gauntletAnimOther;

  public void LocalFireGauntlet() {
    var hit = Physics.OverlapSphere(transform.position + transform.forward * 1.2f, 0.5f, 1 << 10);

    if (hit.Length > 0) {
      foreach (var h in hit) {
        var health = h.GetComponent<Health>();
        if (health.IsAlive()) {
          CmdSetKillMessage(health.netId, transform.name, "hamster wheel");
        }
        CmdDecreaseHealth(h.gameObject, 50.0f);
      }
    }
    gauntletAnim.TurnOn();
    CmdActivateGauntlet();
  }

  [Command]
  public void CmdActivateGauntlet() {
    RpcActivateGauntlet();
  }

  [Command]
  public void CmdDisableGauntlet() {
    RpcDisableGauntlet();
  }

  [ClientRpc]
  public void RpcActivateGauntlet() {
    if (!isLocalPlayer) {
      gauntletAnimOther.TurnOn();
    }
  }

  [ClientRpc]
  public void RpcDisableGauntlet() {
    if (!isLocalPlayer) {
      gauntletAnimOther.TurnOff();
    }
  }

  [Command]
  public void CmdSetKillMessage(NetworkInstanceId id, string name, string weapon) {
    var player = NetworkServer.FindLocalObject(id);
    var combat = player.GetComponent<Combat>();

    combat.hitByPlayer = name;
    combat.hitByWeapon = weapon;
    combat.killerId = netId;
  }

  [Command]
  public void CmdSetWeapon(Weapon w) {
    RpcSetWeapon(w);
  }

  [Command]
  public void CmdDecreaseHealth(GameObject player, float amount) {
    player.GetComponent<Health>().CmdDecreaseHealth(amount, 0);
  }


  [Command]
  public void CmdFireInstant(Vector3 position, Vector3 direction, Vector3 gunOffset, Weapon w) {
    RpcInstant(position, direction, gunOffset, w);
  }


  public void LocalFireProjectile(Vector3 position, Quaternion projectileRotation, Quaternion projectileDirection, GameObject projectilePrefab) {
    projectilePrefab.GetComponent<Projectile>().shootingPlayer = gameObject;
    GameObject projectileInstance = Instantiate(projectilePrefab, position, projectileRotation);

    var rb = projectileInstance.GetComponent<Rigidbody>();
    var properties = projectileInstance.GetComponent<Projectile>();
    rb.velocity = projectileDirection * Vector3.forward * properties.speed;

    properties.shooterId = netId;
    properties.isLocalCreated = true;
    properties.shootingPlayer = gameObject;
  }
}

public enum Weapon { Rocket, MachineGun, Shotgun, Gauntlet, Railgun }