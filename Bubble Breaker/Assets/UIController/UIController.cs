using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Dropdown mGameModeSelector;
    public Text UIScore;
    public GameObject GameOverPanel;
    public Button NewGame;
    public Text EndText;
    public Text EndGameScore;
    public Text EndBonusScore;
    public Text EndTotalScore;
    public Text HighScore;
    public Text GroupScore;

    public GameController mGameController;

    // Use this for initialization
    void Start () {
        //GameOverPanel = Instantiate(Resources.Load("GameOverPanel")) as GameObject;
        GameOverPanel.SetActive(false);
        //GameOverPanel.transform.SetParent(FindObjectOfType<Canvas>().transform);
        GameOverPanel.transform.localPosition = new Vector3(0, 0, 0);

        mGameModeSelector.value = PlayerPrefs.GetInt("GameMode");

        EndBonusScore.text = "";
        EndTotalScore.text = "";
    }
	
	// Update is called once per frame
	void Update () {
        UIScore.text = "Score = " + mGameController.GetScore();
        HighScore.text = "High Score = " + PlayerPrefs.GetInt("HighScore");

        int groupSelectedSize = mGameController.GetGroupSelectedSize();
        if (groupSelectedSize > 0) {
            GroupScore.text = "Group Score = " + groupSelectedSize * (groupSelectedSize - 1);
        }else {
            GroupScore.text = "";
        }
	}
    

    public void HideGameOver() {
        GameOverPanel.SetActive(false);
    }

    public void ShowGameOver() {
        GameOverPanel.SetActive(true);
    }

    public void SetEndText(string endText) {
        EndText.text = endText;
    }

    public void SetEndGameScore(string endGameScore) {
        EndGameScore.text = endGameScore;
    }

    public void SetEndBonusScore(string endBonusScore) {
        EndBonusScore.text = endBonusScore;
    }

    public void SetEndTotalScore(string endTotalScore) {
        EndTotalScore.text = endTotalScore;
    }
}
