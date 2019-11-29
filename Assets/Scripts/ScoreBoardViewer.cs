using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardViewer : MonoBehaviour {

  bool isReleased = true;

  public ScoreBoard scoreBoard;

  
	// Update is called once per frame
	void Update () {
    if (!scoreBoard) {
      scoreBoard = GameObject.Find("Score Board").GetComponent<ScoreBoard>();
    }
    var showScoreBoard = Input.GetAxis("Show Score Board");

    if (showScoreBoard > 0 || scoreBoard.GameOver()) {
      if (!scoreBoardUI.activeInHierarchy) {
        scoreBoardUI.SetActive(true);
      }
    } else {
      scoreBoardUI.SetActive(false);
    }

    boyoKills.text = scoreBoard.player1Kills.ToString();
    mamaKills.text = scoreBoard.player2Kills.ToString();
    papaKills.text = scoreBoard.player3Kills.ToString();
    locoKills.text = scoreBoard.player4Kills.ToString();

    boyoDeaths.text = scoreBoard.player1Deaths.ToString();
    mamaDeaths.text = scoreBoard.player2Deaths.ToString();
    papaDeaths.text = scoreBoard.player3Deaths.ToString();
    locoDeaths.text = scoreBoard.player4Deaths.ToString();

    var t = scoreBoard.timer;
    var minutes = Mathf.Floor(t / 60.0f);
    var seconds = Mathf.Floor(t - minutes * 60);

    if (t < 0) {
      minutes = 0;
    }
    if (t < 0) {
      seconds = 0;
    }
    var divider = ":";
    if (seconds < 10) {
      divider = ":0";
    }

    timer.text = "0" + minutes.ToString() + divider + seconds.ToString();

    if (t < 0) {
      winner.text = "Winner: " + scoreBoard.Winner();
    }
	}

  public GameObject scoreBoardUI;

  public Text papaKills;
  public Text papaDeaths;

  public Text mamaKills;
  public Text mamaDeaths;

  public Text boyoKills;
  public Text boyoDeaths;

  public Text locoKills;
  public Text locoDeaths;

  public Text timer;

  public Text winner;



}
