using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreBoard : NetworkBehaviour {

  [SyncVar]
  public int player1Kills = 0;

  [SyncVar]
  public int player1Deaths = 0;

  [SyncVar]
  public NetworkInstanceId player1Id;

  [SyncVar]
  public int player2Kills = 0;

  [SyncVar]
  public int player2Deaths = 0;

  [SyncVar]
  public NetworkInstanceId player2Id;

  [SyncVar]
  public int player3Kills = 0;

  [SyncVar]
  public int player3Deaths = 0;
  [SyncVar]
  public NetworkInstanceId player3Id;

  [SyncVar]
  public int player4Kills = 0;

  [SyncVar]
  public int player4Deaths = 0;

  [SyncVar]
  public NetworkInstanceId player4Id;

  private Stack<int> freePlayerSlots = new Stack<int>();

  [SyncVar]
  public float timer = 60.0f * 5.0f;

  public void Update() {
    if (!isServer) {
      return;
    }

    timer -= Time.deltaTime;

    if (timer < -20.0f) {
      NetworkManager.singleton.StopHost();
    }
  }

  public override void OnStartServer() {
    print("Start score board!");
    base.OnStartServer();

    freePlayerSlots.Push(3);
    freePlayerSlots.Push(2);
    freePlayerSlots.Push(1);
    freePlayerSlots.Push(0);
  }

  public bool GameOver() {
    return timer <= 0.0f;
  }

  public string Winner() {
    int mostKills = player1Kills;
    string winner = "Boyo";

    if (player2Kills > mostKills) {
      mostKills = player2Kills;
      winner = "Mama";
    }

    if (player3Kills > mostKills) {
      mostKills = player3Kills;
      winner = "Papa";
    }

    if (player4Kills > mostKills) {
      mostKills = player4Kills;
      winner = "Loco";
    }

    return winner;
  }

  [Command]
  public void CmdRegisterPlayer(NetworkInstanceId id) {
    if (GameOver()) return;
    print("Registered player");
    var next = freePlayerSlots.Pop();
    if (next == 0) {
      player1Id = id;
    } else if (next == 1) {
      player2Id = id;
    } else if (next == 2) {
      player3Id = id;
    } else if (next == 3) {
      player4Id = id;
    } else {
      print("Too many players");
    }

    //var player = NetworkServer.
  }

  [Command]
  public void CmdAddPoint(NetworkInstanceId id) {
    if (GameOver()) return;
    if (id == player1Id) {
      player1Kills += 1;
    } else if (id == player2Id) {
      player2Kills += 1;
    } else if (id == player3Id) {
      player3Kills += 1;
    } else if (id == player4Id) {
      player4Kills += 1;
    } else {
      print("Wrong id");
    }
  }

  [Command]
  public void CmdDropPointWithKiller(NetworkInstanceId id, NetworkInstanceId killerId) {
    if (GameOver()) return;
    if (id == player1Id) {
      player1Deaths += 1;
    } else if (id == player2Id) {
      player2Deaths += 1;
    } else if (id == player3Id) {
      player3Deaths += 1;
    } else if (id == player4Id) {
      player4Deaths += 1;
    } else {
      print("Wrong id");
    }

    CmdAddPoint(killerId);
  }

  [Command]
  public void CmdDropPoint(NetworkInstanceId id) {
    if (GameOver()) return;
    if (id == player1Id) {
      player1Deaths += 1;
      player1Kills -= 1;
    } else if (id == player2Id) {
      player2Deaths += 1;
      player2Kills -= 1;
    } else if (id == player3Id) {
      player3Deaths += 1;
      player3Kills -= 1;
    } else if (id == player4Id) {
      player4Deaths += 1;
      player4Kills -= 1;
    } else {
      print("Wrong id");
    }
  }

  //override public string ToString() {
  //  return
  //    "Player 1: " + (player1Score >= 0 ? " " + player1Score.ToString() : player1Score.ToString()) + "   " +
  //    "Player 2: " + (player2Score >= 0 ? " " + player2Score.ToString() : player2Score.ToString()) + "   " +
  //    "Player 3: " + (player3Score >= 0 ? " " + player3Score.ToString() : player3Score.ToString()) + "   " +
  //    "Player 4: " + (player4Score >= 0 ? " " + player4Score.ToString() : player4Score.ToString());
  //}

  public int getPlayerNum(NetworkInstanceId id) {
    if (id == player1Id) {
      return 0;
    } else if (id == player2Id) {
      return 1;
    } else if (id == player3Id) {
      return 2;
    } else if (id == player4Id) {
      return 3;
    } else {
      return 0;
    }
  }


  //[SyncVar]
  //public List<int> playerScores = new List<int>();





	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
