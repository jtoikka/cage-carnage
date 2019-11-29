using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToCamera : MonoBehaviour {

  //public Camera camera;

  void Awake() {
    //camera = Camera.main;
  }


  void Update() {
    var camera = Camera.main;
    transform.LookAt(camera.transform);
  }
}
