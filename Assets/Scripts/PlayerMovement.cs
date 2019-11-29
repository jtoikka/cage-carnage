using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour {

  private CharacterController characterController;

  public float speedLimit = 1.0f;

  public float xSensitivity = 1.0f;
  public float ySensitivity = 1.0f;

  private float gunRotation;

  public Transform gunHolder;

  public bool isGrounded = true;

  private bool jumpReset = true;

  public float jumpForce = 1.0f;

  public float gravity = -10.0f;

  private Animator animator;

  private bool shootReset;

  private bool isGettingKnockedBack;

  public Vector3 velocity = new Vector3(0, 0, 0);

  private float pm_stopspeed = 127.0f / 100.0f * 8.0f;
  private float pm_duckScale = 0.25f;
  private float pm_accelerate = 10.0f;
  private float pm_airaccelerate = 1.0f;
  private float pm_friction = 4.0f;

  private float footstepTimer = 0.5f; 

  private NetworkStartPosition[] spawnPoints;

  public GameObject RagdollPrefab;
  public Transform middleBackBone;

  private Quaternion baseMiddleBackRotation;

  public GameObject jumpSound;
  Transform camera;

  public Transform ragdollMiddleBack;

  public Material[] playerMats;

	// Use this for initialization
	void Start () {
    characterController = GetComponent<CharacterController>();

    animator = transform.GetComponent<Animator>();
    var networkAnimator = transform.GetComponent<NetworkAnimator>();
    networkAnimator.SetParameterAutoSend(0, true);
    networkAnimator.SetParameterAutoSend(1, true);
    networkAnimator.SetParameterAutoSend(2, true);

    baseMiddleBackRotation = middleBackBone.localRotation;

    if (!isLocalPlayer) {
      var cam = transform.GetChild(0).GetChild(0);
      //cam.GetChild(0).parent = null;
      Destroy(cam.GetChild(0).gameObject);
    } else {
      camera = gunHolder.GetChild(0).GetChild(0);
      spawnPoints = FindObjectsOfType<NetworkStartPosition>();

      var bod = transform.GetChild(1);
      bod.gameObject.SetActive(false);
    }
	}

  private ScoreBoard scoreBoard;

  private GameObject ui;

  public override void OnStartClient() {
    base.OnStartClient();

    scoreBoard = GameObject.Find("Score Board").GetComponent<ScoreBoard>();

    if (isServer) {
      scoreBoard.CmdRegisterPlayer(netId);
    }

    ui = transform.GetChild(2).gameObject;
    ui.SetActive(false);

    ui.transform.parent = null;
  }

  private float respawnTimer = 3.0f;

  public float recoilTimer = 0.0f;

  public void SetShooting() {
    footstepTimer = 0.5f;
  }

  //private bool isSwappingWeapon;
  private float swapWeaponTimer = 0.0f;

  public void SetSwappingWeapon() {
    //isSwappingWeapon = true;
    swapWeaponTimer = 1.0f;
    animator.SetTrigger("Change Weapons");
    //GetComponent<Combat>().CmdSetWeapon();
  }

  public void SetPosition(Vector3 pos) {
    transform.position = pos;
  }

  bool hasResetCombat = false;

  //Transform camera; 

  float groundedCounter = 0.0f;

  private bool hasResetRagdoll = false;

	// Update is called once per frame
	void FixedUpdate () {
    
    var health = GetComponent<Health>();
    if (health.IsAlive()) {
      GetComponent<CharacterController>().enabled = true;
      if (!isLocalPlayer) {
        transform.GetChild(1).gameObject.SetActive(true);
      }
      if (!hasResetRagdoll) {
        hasResetRagdoll = true;
        if (transform.childCount == 3) {
          Destroy(transform.GetChild(2).gameObject);
        }
        var ragdoll = Instantiate(RagdollPrefab);
        ragdoll.transform.parent = transform;
        ragdollMiddleBack = ragdoll.transform.GetChild(1).GetChild(0).GetChild(0);
        ragdoll.SetActive(false);
        ragdoll.transform.localPosition = Vector3.zero;

        var skin = ragdoll.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

        if (scoreBoard) {
          
          var num = scoreBoard.getPlayerNum(netId);

          if (isLocalPlayer) {
            ui.transform.Find("Head").GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = playerMats[num];
          }

          skin.material = playerMats[num];

          switch(num) {
            case 0:
              name = "Boyo";
              break;
            case 1:
              name = "Mama";
              break;
            case 2:
              name = "Papa";
              break;
            case 3:
              name = "Loco";
              break;
            default:
              break;
          }
        }
      }
    } else {
      GetComponent<CharacterController>().enabled = false;
      hasResetRagdoll = false;
      if (transform.childCount > 2) {
        if (!transform.GetChild(2).gameObject.activeInHierarchy) {
          transform.GetChild(2).gameObject.SetActive(true);
          ragdollMiddleBack.GetComponent<Rigidbody>().velocity = velocity * 10.0f;
        }
      }
      transform.GetChild(1).gameObject.SetActive(false);
    }
    if (!isLocalPlayer) {
      var ham = transform.GetChild(1).GetChild(0);
      var skin = ham.GetComponent<SkinnedMeshRenderer>();

      if (scoreBoard) {
        var num = scoreBoard.getPlayerNum(netId);

        skin.material = playerMats[num];
      }

      return;
    }
    if (!ui.activeInHierarchy) {
      var num = scoreBoard.getPlayerNum(netId);
      ui.SetActive(true);
      ui.transform.Find("Head").GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = playerMats[num];
    }

    if (recoilTimer > 0) {
      recoilTimer -= Time.fixedDeltaTime;
    }

    if (!health.IsAlive()) {
      var combat = GetComponent<Combat>();

      var killer = ClientScene.FindLocalObject(combat.killerId);

      camera.parent = null;

      camera.position = killer.GetComponent<PlayerMovement>().ragdollMiddleBack.transform.position + new Vector3(2, 2, 2);

      camera.LookAt(killer.GetComponent<PlayerMovement>().ragdollMiddleBack);

      if (!hasResetCombat) {
        hasResetCombat = true;
        combat.Reset();

        var soundVersion = (int)Mathf.Floor(Random.value * 3);

        playDeathSound(transform.position, soundVersion);
        CmdPlayDeathSound(soundVersion);

        //ragdollMiddleBack.GetComponent<Rigidbody>().velocity = velocity;
      }
      respawnTimer -= Time.fixedDeltaTime;
      if (respawnTimer <= 0) {
        respawnTimer = 3.0f;
        health.CmdResetHealth();

        var spawn = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        var spawnPoint = spawn.position;

        velocity = Vector3.zero;
        transform.position = spawnPoint + new Vector3(0, 1, 0);
        transform.rotation = spawn.rotation;
        gunRotation = 0.0f;

      }
    } else {
      hasResetCombat = false;
      if (camera) {
        camera.parent = gunHolder.GetChild(0);

        camera.localPosition = new Vector3(0, 0, 0);

        camera.localRotation = Quaternion.identity;
      }
    }

    var mouseX = Input.GetAxisRaw("Mouse X");
    var mouseY = Input.GetAxisRaw("Mouse Y");

    var mouseSensitivityAdjust = Input.GetAxis("Mouse Sensitivity");

    xSensitivity += mouseSensitivityAdjust;
    ySensitivity += mouseSensitivityAdjust;

    if (xSensitivity < 1) {
      xSensitivity = 1;
    }
    if (ySensitivity < 1) {
      ySensitivity = 1;
    }

    if (!health.IsAlive()) {
      mouseX = 0;
      mouseY = 0;
    }

    var sphereCheck = Physics.CheckSphere(transform.position - new Vector3(0f, 0.6f, 0f), 0.5f, 1 << 8);
    sphereCheck = sphereCheck || Physics.CheckSphere(transform.position - new Vector3(0f, 0.7f, 0f), 0.5f, 1 << 8);

    var newGrounded = sphereCheck && velocity.y <= 0;

    if (!isGrounded && newGrounded) {
      if (velocity.y < -16f) {
        if (health.IsAlive()) {
          GetComponent<Combat>().CmdSetKillMessage(health.netId, "self", "floor");
        }
        health.CmdDecreaseHealth((-velocity.y - 16f) * 2f, 2);
      }
    }

    isGrounded = newGrounded;

    gunRotation += mouseY * Time.deltaTime * ySensitivity;
    gunRotation = ClampAngle(gunRotation, -90, 90);
    gunHolder.localRotation = Quaternion.AngleAxis(gunRotation, new Vector3(-1, 0, 0));

    transform.localRotation = transform.localRotation * Quaternion.AngleAxis(mouseX * Time.fixedDeltaTime * xSensitivity, new Vector3(0, 1, 0));

    var rotation = transform.rotation;

    if (isGrounded) {
      groundedCounter = 0.0f;
      walkMove();
      var ray = new Ray(transform.position + new Vector3(0, -1, 0), Vector3.down);

      RaycastHit hit;

      Physics.Raycast(ray, out hit);

      var normal = hit.normal;

      var rotationToNormal = Quaternion.FromToRotation(Vector3.up, normal);

      footstepTimer += velocity.magnitude * Time.fixedDeltaTime * 0.4f;

      characterController.Move(rotationToNormal * velocity * Time.fixedDeltaTime);

      var s = Vector3.Dot(velocity, transform.forward);

      var f = velocity;
      f.y = 0;

      //print("Speed: " + f);

      if (s < 0) {
        animator.SetFloat("Forward", -f.magnitude / 5.0f);
      } else {
        animator.SetFloat("Forward", f.magnitude / 5.0f);
        animator.SetBool("Jump", false);
      }
    } else {
      //animator.SetBool("Backflip", false);
      if (groundedCounter < 0.1f) {
        CheckJump();
      } else {
        //animator.SetBool("Backflip", false);
      }
      groundedCounter += Time.fixedDeltaTime;
      airMove();
      footstepTimer = 0.5f;
      characterController.Move(velocity * Time.fixedDeltaTime);

      //CmdSetAnimatorForward(0.0f);

      //animator.SetFloat("Forward", 0.0f);
    }

    var sign = Mathf.Sign(Mathf.Sin(footstepTimer * Mathf.PI));
    var remainder = footstepTimer - Mathf.Floor(footstepTimer);
    var cameraRoll = (-1.0f + remainder * 2.0f) * sign;


    gunHolder.GetChild(0).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 0.5f, Vector3.forward);
    gunHolder.GetChild(1).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 10.0f, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
    gunHolder.GetChild(2).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 10.0f, Vector3.up);
    gunHolder.GetChild(3).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 10.0f, Vector3.up);
    gunHolder.GetChild(4).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 10.0f, Vector3.up);
    gunHolder.GetChild(5).transform.localRotation = Quaternion.AngleAxis(cameraRoll * 10.0f, Vector3.up);

    var t = (footstepTimer + 0.5f) * 2.0f;

    sign = Mathf.Sign(Mathf.Sin(t * Mathf.PI));
    remainder = t - Mathf.Floor(t);
    var bobble = (-1.0f + remainder * 2.0f) * sign;

    gunHolder.GetChild(0).transform.localPosition = new Vector3(0, 0.025f + bobble * 0.025f, 0);

    if (swapWeaponTimer > 0.0f) {
      swapWeaponTimer -= Time.fixedDeltaTime * 4;

      for (var i = 1; i < 6; i++) {
        gunHolder.GetChild(i).localRotation = gunHolder.GetChild(i).localRotation * Quaternion.AngleAxis(swapWeaponTimer * 90.0f, Vector3.right);;
      }
      //gunHolder.localRotation = gunHolder.localRotation * Quaternion.AngleAxis(swapWeaponTimer * 90.0f, new Vector3(0, -1, 0));
    }
	}

  private void LateUpdate() {
    middleBackBone.localRotation = gunHolder.transform.localRotation * baseMiddleBackRotation;
  }

  [Command]
  public void CmdSetVelocity(Vector3 v) {
    RpcSetVelocity(v);
  }

  [ClientRpc]
  public void RpcSetVelocity(Vector3 v) {
    if (!isLocalPlayer) {
      velocity = v;
    }
  }

  [Command]
  public void CmdDiedSelf() {
    print("Died");
    scoreBoard.CmdDropPoint(netId);
  }

  [Command]
  public void CmdDied(NetworkInstanceId killerId) {
    print("Died and killed");
    scoreBoard.CmdDropPointWithKiller(netId, killerId);
  }

  void OnControllerColliderHit(ControllerColliderHit hit) {
    var normal = hit.normal;

    if (normal.y > Mathf.Sin(Mathf.PI / 4)) {
      return;
    }

    var other = normal;

    other.x = 0;
    other.z = 0;

    if (normal.y > -Mathf.Sin(Mathf.PI / 4) || velocity.y < 0) {
      other.y = 0;
    }

    normal.y = 0;

    var neg = -normal;

    var l = Vector3.Dot(neg, velocity);

    velocity += normal * l + other * velocity.y;
  }

  // TODO: Jump should maybe be delayed by a frame
  private bool CheckJump() {

    var isHoldingJump = Input.GetAxis("Jump") > 0;

    var health = GetComponent<Health>();

    if (!health.IsAlive()) {
      isHoldingJump = false;
    }
    if (!isHoldingJump) {
      jumpReset = true;
      return false;
    }

    if (!jumpReset) {
      return false;
    }

    if (isGrounded || (groundedCounter < 0.1f && velocity.y < 0)) {
      if (velocity.y < 0) {
        velocity.y = 0;
      }
      velocity.y += jumpForce;
      if (Input.GetAxis("Vertical") < 0) {
        animator.SetBool("Jump", true);
        //print("Jump");
      } else {
        //animator.SetTrigger("Jump");
        //print("Jump");
      }
    }

    isGrounded = false;
    jumpReset = false;

    var sound = Instantiate(jumpSound, transform.position, Quaternion.identity);
    sound.transform.parent = transform.GetChild(0).GetChild(0);
    sound.transform.localPosition = Vector3.zero;

    var source = sound.GetComponent<AudioSource>();

    source.pitch += (Random.value * 0.6f - 0.3f);

    CmdJumpSound(transform.position);
    return true;
  }

  [Command]
  public void CmdJumpSound(Vector3 pos) {
    RpcJumpSound(pos);
  }

  [ClientRpc]
  public void RpcJumpSound(Vector3 pos) {
    if (isLocalPlayer) {
      return;
    }
    var sound = Instantiate(jumpSound, pos, Quaternion.identity);
    var source = sound.GetComponent<AudioSource>();

    //sound.transform.parent = transform.GetChild(0).GetChild(0);
    //sound.transform.localPosition = Vector3.zero;

    source.pitch += (Random.value * 0.6f - 0.3f);
  }


  public void recoil(Vector3 force) {
    recoilTimer += 0.4f;
    velocity += force;
  }

  public void Launch(Vector3 force) {
    velocity = force;
    CmdPlayLaunchSound();
    playLaunchSound(transform.position + Vector3.down);
  }

  [Command]
  public void CmdPlayLaunchSound() {
    RpcPlayLaunchSound(transform.position + Vector3.down);
  }

  [ClientRpc]
  public void RpcPlayLaunchSound(Vector3 pos) {
    if (!isLocalPlayer) playLaunchSound(pos);
  }

  public AudioSource trampolineSound;

  public void playLaunchSound(Vector3 pos) {
    Instantiate(trampolineSound, pos, Quaternion.identity);
  }

  [Command]
  public void CmdPlayDeathSound(int version) {
    RpcPlayDeathSound(transform.position, version);
  }

  [ClientRpc]
  public void RpcPlayDeathSound(Vector3 pos, int version) {
    if (!isLocalPlayer) playDeathSound(pos, version);
  }

  public AudioSource deathSound1;
  public AudioSource deathSound2;
  public AudioSource deathSound3;

  public void playDeathSound(Vector3 pos, int version) {
    AudioSource s;
    if (version == 0) {
      s = Instantiate(deathSound1, pos, Quaternion.identity); 
    } else if (version == 1) {
      s = Instantiate(deathSound2, pos, Quaternion.identity); 
    } else {
      s = Instantiate(deathSound3, pos, Quaternion.identity);
    }
    s.transform.parent = transform;
    s.transform.localPosition = Vector3.zero;
  }

  public void RotateVelocity(Vector3 dir) {
    velocity = velocity.magnitude * dir.normalized;
  }

  // TODO: This should be in "Combat"
  [Command]
  public void CmdShootPlayer(GameObject player, Vector3 force, float damage) {
    var movementComponent = player.GetComponent<PlayerMovement>();
    movementComponent.RpcRecoil(force);

    var health = player.GetComponent<Health>();

    health.CmdDecreaseHealth(damage, 0);
  }

  [ClientRpc]
  private void RpcRecoil(Vector3 force) {
    velocity += force;
  }

  private void walkMove() {
    if (CheckJump()) {
      airMove();
      return;
    }

    applyFriction();

    var horizontal = Input.GetAxis("Horizontal");
    var vertical = Input.GetAxis("Vertical");

    var health = GetComponent<Health>();

    if (!health.IsAlive()) {
      horizontal = 0;
      vertical = 0;
    }

    var forward = (transform.forward * vertical).normalized;
    var right = (transform.right * horizontal).normalized;

    var wishVel = (forward + right).normalized;


    var accel = pm_accelerate;
    if (recoilTimer > 0) {
      accel = pm_airaccelerate;
    }

    var wishSpeed = 8.6f;


    accelerate(wishVel, wishSpeed, accel);
  }

  private void airMove() {
    var isHoldingJump = Input.GetAxis("Jump") > 0;
    if (!isHoldingJump) {
      jumpReset = true;
    }

    velocity.y += gravity * Time.fixedDeltaTime;

    var horizontal = Input.GetAxis("Horizontal");
    var vertical = Input.GetAxis("Vertical");

    var health = GetComponent<Health>();

    if (!health.IsAlive()) {
      horizontal = 0;
      vertical = 0;
    }

    var forward = (transform.forward * vertical).normalized;
    var right = (transform.right * horizontal).normalized;

    var wishVel = (forward + right).normalized;

    var wishSpeed = 8.6f;

    if (wishVel.magnitude == 0.0f) {
      wishSpeed = 0;
    }

    if (recoilTimer > 0) {
      wishVel = Vector3.zero;
    }

    var d = Vector3.Dot(wishVel.normalized, velocity.normalized);
    float bonusAcc = 0.0f;
    if (d < 0) {
      bonusAcc = d * -1 * pm_airaccelerate;
    }

    accelerate(wishVel, wishSpeed, pm_airaccelerate + bonusAcc);

  }

  private void applyFriction() {

    var vel = velocity;

    if (isGrounded) {
      vel.y = 0;
      velocity.y = 0;
    }

    var speed = vel.magnitude;

    if (speed < 1) {
      velocity.x = 0;
      velocity.z = 0;

      return;
    }

    float drop = 0.0f;

    if (!isGettingKnockedBack) {
      var control = speed < pm_stopspeed ? pm_stopspeed : speed;
      drop += control * pm_friction * Time.fixedDeltaTime;
    }

    var newSpeed = speed - drop;

    if (newSpeed < 0) {
      newSpeed = 0;
    }
    newSpeed /= speed;

    velocity *= newSpeed;
  }

  private void accelerate(Vector3 wishDirection, float wishSpeed, float accel) {
    var currentSpeed = Vector3.Dot(velocity, wishDirection);
    var addSpeed = wishSpeed - currentSpeed;

    if (addSpeed <= 0) {
      return;
    }
    var accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;

    if (accelSpeed > addSpeed) {
      accelSpeed = addSpeed;
    }

    velocity += accelSpeed * wishDirection;

  }

  public static float ClampAngle(float angle, float min, float max) {
    angle = angle % 360;
    if ((angle >= -360F) && (angle <= 360F)) {
      if (angle < -360F) {
        angle += 360F;
      }
      if (angle > 360F) {
        angle -= 360F;
      }
    }
    return Mathf.Clamp(angle, min, max);
  }
}
