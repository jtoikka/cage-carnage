using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkBasics : NetworkBehaviour {

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

  public string ipAddress;

  public GameObject controls;

  public void setIpAddress(string address) {
    NetworkManager.singleton.networkPort = 7777;
    ipAddress = address;
  }

  public Text clientIPText;

  public void startHost() {
    NetworkManager.singleton.StartHost();
    print("Start host!");
  }

  public void startClient() {
    NetworkManager.singleton.networkPort = 7777;
    NetworkManager.singleton.networkAddress = clientIPText.text;
    NetworkManager.singleton.StartClient();
  }

  public void Quit() {
    Application.Quit();
  }

  public void HideControls() {
    controls.SetActive(false);
  }

  public void ShowControls() {
    controls.SetActive(true);
  }
}
