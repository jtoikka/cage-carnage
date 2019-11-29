using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorState : MonoBehaviour {

  public bool isVisible = true;
  public CursorLockMode lockMode = CursorLockMode.None;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    Cursor.visible = isVisible;
    Cursor.lockState = lockMode;
	}
}
