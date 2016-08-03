using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Dropdown mGameModeSelector;
    public Text UIScore;
    public GameObject GameOverPanel;
    public Text EndText;
    public Text EndGameScore;
    public Text EndBonusScore;
    public Text EndTotalScore;

    public Text GamesStandard;
    public Text AverageStandard;
    public Text HighStandard;

    public Text GamesContinuous  ;
    public Text AverageContinuous;
    public Text HighContinuous   ;

    public Text GamesShifter  ;
    public Text AverageShifter;
    public Text HighShifter   ;

    public Text GamesMegaShift  ;
    public Text AverageMegaShift;
    public Text HighMegaShift   ;

    public Text UIGroupScore;

    public Text UIGameMode;

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

        GamesStandard.text   = PlayerPrefs.GetInt("GamesStandard",0).ToString();
        AverageStandard.text = PlayerPrefs.GetInt("AverageStandard", 0).ToString();
        HighStandard.text    = PlayerPrefs.GetInt("HighStandard", 0).ToString();

        GamesContinuous.text   = PlayerPrefs.GetInt("GamesContinuous", 0).ToString();
        AverageContinuous.text = PlayerPrefs.GetInt("AverageContinuous", 0).ToString();
        HighContinuous.text    = PlayerPrefs.GetInt("HighContinuous", 0).ToString();

        GamesShifter.text   = PlayerPrefs.GetInt("GamesShifter", 0).ToString();
        AverageShifter.text = PlayerPrefs.GetInt("AverageShifter", 0).ToString();
        HighShifter.text    = PlayerPrefs.GetInt("HighShifter", 0).ToString();

        GamesMegaShift.text   = PlayerPrefs.GetInt("GamesMegaShift", 0).ToString();
        AverageMegaShift.text = PlayerPrefs.GetInt("AverageMegaShift", 0).ToString();
        HighMegaShift.text    = PlayerPrefs.GetInt("HighMegaShift", 0).ToString();

        int groupSelectedSize = mGameController.GetGroupSelectedSize();
        if (groupSelectedSize > 0) {
            UIGroupScore.text = "Group Score = " + groupSelectedSize * (groupSelectedSize - 1);
        }else {
            UIGroupScore.text = "";
        }

        UIGameMode.text = mGameController.GetGameModeStr();
	}
    

    public void HideGameOver() {
        GameOverPanel.SetActive(false);
        EndBonusScore.text = "";
        EndTotalScore.text = "";
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
