using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class GameController : MonoBehaviour {
    
    public GameObject [][] mBallsList;

    public Text UIScore;
    public GameObject GameOverPanel;
    public Button NewGame;
    public Text EndText;
    public Text EndPanelScore;
    public Text HighScore;
    public Text GroupScore;

    public int mScore = 0;

    private List<GameObject> mSelectedGroup;

    private Color test = new Color(50, 50, 50);

    private int rows = 8;
    private int columns = 15;

    public bool startNewGame;

    private bool clickDown = false;
    private bool clickDownPrev = false;

    // Use this for initialization
    void Start() { 

        //GameOverPanel = Instantiate(Resources.Load("GameOverPanel")) as GameObject;
        GameOverPanel.SetActive(false);
        //GameOverPanel.transform.SetParent(FindObjectOfType<Canvas>().transform);
        GameOverPanel.transform.localPosition = new Vector3(0, 0, 0);

        mSelectedGroup = new List<GameObject>();

        startNewGame = true;
    }
	
	// Update is called once per frame
	void Update () {

        if (startNewGame){
            CreateNewGame();
        }

        UIScore.text = "Score = " + mScore;
        HighScore.text = "High Score = " + PlayerPrefs.GetInt("HighScore");
        if (mSelectedGroup.Count > 0) {
            GroupScore.text = "Group Score = " + mSelectedGroup.Count * (mSelectedGroup.Count - 1);
        }
        else {
            GroupScore.text = "";
        }
        

        OnClick();
    }

    public void CreateNewGame(){
        startNewGame = false;
        GameOverPanel.SetActive(false);
        mSelectedGroup.Clear();
        if (mBallsList != null) {
            for (int i = 0; i < rows; i++){
                for (int j = 0; j < columns; j++){
                    if (mBallsList[i][j]) {
                        Destroy(mBallsList[i][j]);
                        mBallsList[i][j] = null;
                    }
                }
            }
        }else {
            mBallsList = new GameObject[rows][];
            for (int i = 0; i < rows; i++)
            {
                mBallsList[i] = new GameObject[columns];
            }
        }
        
        mScore = 0;

        for (int i = 0; i < rows; i++){
            for (int j = 0; j < columns; j++){
                int random = Random.Range(0,4);
                
                GameObject ball = Instantiate(Resources.Load("Ball")) as GameObject;
                switch(random){
                    case 0:
                        ball.GetComponent<SpriteRenderer>().color = Color.blue;
                        ball.GetComponent<Ball>().mBallType = BALL_TYPE.Type0;
                        break;
                    case 1:
                        ball.GetComponent<SpriteRenderer>().color = Color.green;
                        ball.GetComponent<Ball>().mBallType = BALL_TYPE.Type1;
                        break;
                    case 2:
                        ball.GetComponent<SpriteRenderer>().color = Color.red;
                        ball.GetComponent<Ball>().mBallType = BALL_TYPE.Type2;
                        break;
                    case 3:
                        ball.GetComponent<SpriteRenderer>().color = Color.yellow;
                        ball.GetComponent<Ball>().mBallType = BALL_TYPE.Type3;
                        break;
                    case 4:
                        ball.GetComponent<SpriteRenderer>().color = Color.blue;
                        ball.GetComponent<Ball>().mBallType = BALL_TYPE.Type4;
                        break;
                    default: break;
                }
                ball.GetComponent<Ball>().mRowPos = i;
                ball.GetComponent<Ball>().mColPos = j;
                ball.transform.localPosition = new Vector2(j,i);
                mBallsList[i][j] = ball;
            }
        }
    }

    public void Exit(){
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif
    }

    void ApplyGravity() {
        for (int i = 0; i < columns; i++){
            while (SearchForHoleInColumn(i)) {
                MoveColumnDown(i);
            }
        }
        while (SearchForHoleInFirstRow()){
            MoveColumnsToRight();
        }

        if (!MovesLeft()) {
            EndGame();
        }
    }

    bool SearchForHoleInColumn(int column) {
        int top = -1;
        for (int i = rows-1; i >= 0; i--) {
            if (top == -1 && mBallsList[i][column]) {
                top = i;
            } else if (top != -1 && !mBallsList[i][column]) {
                return true;
            }
        }
        return false;
    }

    void MoveColumnDown(int column) {
        for (int i = 0; i < rows; i++) {
            if (!mBallsList[i][column]){
                for (int j = i; j < rows-1; j++) {
                    GameObject ball = null;
                    if (mBallsList[j + 1][column]) {
                        ball = mBallsList[j + 1][column];
                        mBallsList[j + 1][column] = null;
                        mBallsList[j][column] = ball;
                        ball.GetComponent<Ball>().mRowPos--;
                        ball.transform.localPosition = new Vector2(column, j);

                    }
                    

                }
            }
        }        
    }

    public void OnClick() {

        #if UNITY_ANDROID
            if (Input.touchCount >= 1 != clickDownPrev) {
                clickDown = Input.touchCount >= 1;
            }else{
                return;
            }
            
        #else
            clickDown = Input.GetMouseButtonDown(0);
        #endif

        if (clickDown) {

            Vector3 input = new Vector3(0, 0, 0);

            #if UNITY_ANDROID
                input = Input.GetTouch(0).position;
            #else
                input = Input.mousePosition;
            #endif

            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = input;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if (raycastResults.Count > 0) {
                return;
            }

            RaycastHit2D hit;

            Ray aux = Camera.main.ScreenPointToRay(input);
            hit = Physics2D.Raycast(aux.origin, aux.direction,
                                        Mathf.Infinity, 1 << LayerMask.NameToLayer("Default"));

            if (mSelectedGroup.Count > 0) {
                if (hit) {
                    GameObject ball = hit.collider.gameObject;
                    if (IsBallInSelectedGroup(ball)) {
                        ApplyScore();
                        DeleteSelectedGroup();
                        ApplyGravity();
                    }else {
                        ClearSelectedGroup();
                        
                    }
                }else{
                    ClearSelectedGroup();
                }
            } else {
                if (hit) {
                    GameObject ball = hit.collider.gameObject;
                    if (ball.GetComponent<Ball>()) {
                        SelectGroup(ball);
                    }
                }
            }
            
        }
        clickDownPrev = clickDown;
    }

    public void SelectGroup(GameObject ball) {
        mSelectedGroup.Add(ball);

        AddAdjacentBalls(ball);

        ball.GetComponent<SpriteRenderer>().color = ball.GetComponent<SpriteRenderer>().color + test;

        if (mSelectedGroup.Count <= 1) {
            ClearSelectedGroup();
        }
        
    }

    private bool IsBallInSelectedGroup(GameObject ball) {
        for (int i = 0; i < mSelectedGroup.Count; i++) {
            if (mSelectedGroup[i].GetInstanceID() == ball.GetInstanceID()) {
                return true;
            }
        }
        return false;
    }

    public void DeleteSelectedGroup() {
        while (mSelectedGroup.Count > 0)
        {
            GameObject ball = mSelectedGroup[0];

            for (int i = 0; i < rows; i++){
                for (int j = 0; j < columns; j++){
                    if (mBallsList[i][j]) { 
                        if (mBallsList[i][j].gameObject == ball) {
                            mSelectedGroup.Remove(ball);
                            DestroyObject(mBallsList[i][j]);
                            mBallsList[i][j] = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ClearSelectedGroup() {
        for (int i = 0; i < mSelectedGroup.Count; i++) {
            mSelectedGroup[i].GetComponent<SpriteRenderer>().color = mSelectedGroup[i].GetComponent<SpriteRenderer>().color - test;
        }
        mSelectedGroup.Clear();
    }

    private void AddAdjacentBalls(GameObject ball) {
        
        Ball ballComponent = ball.GetComponent<Ball>();

        if (ballComponent.mRowPos - 1 >= 0 && mBallsList[ballComponent.mRowPos-1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos - 1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBalls(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = ballAux.GetComponent<SpriteRenderer>().color + test;
            }
        } 
        if (ballComponent.mColPos - 1 >= 0 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBalls(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = ballAux.GetComponent<SpriteRenderer>().color + test;
            }
        } 
        if (ballComponent.mRowPos + 1 <= rows - 1 && mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBalls(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = ballAux.GetComponent<SpriteRenderer>().color + test;
            }
        }
        if (ballComponent.mColPos + 1 <= columns - 1 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBalls(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = ballAux.GetComponent<SpriteRenderer>().color + test;
            }
        }    
    }

    private bool SearchForHoleInFirstRow() {
        int first = -1;
        for (int i = 0; i < columns; i++) {
            if (first == -1 && mBallsList[0][i]){
                first = i;
            }else if (first != -1 && !mBallsList[0][i]){
                return true;
            }
        }
        return false;
    }

    private void MoveColumnsToRight() {
        for (int i = columns - 1; i >= 0; i--){
            if (!mBallsList[0][i]){
                for (int j = i; j > 0; j--) {
                    for (int k = 0; k < rows; k++) {
                        GameObject ball = null;
                        if (mBallsList[k][j-1]) {
                            ball = mBallsList[k][j - 1];
                            mBallsList[k][j - 1] = null;
                            mBallsList[k][j] = ball;
                            ball.GetComponent<Ball>().mColPos++;
                            ball.transform.localPosition = new Vector2(j, k);
                        }
                    }
                        
                   
                }
            }
        }      
    }

    private void ApplyScore() {
        mScore += mSelectedGroup.Count*(mSelectedGroup.Count-1);
    }

    private bool MovesLeft() {
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (mBallsList[i][j]) {
                    if (HasAdjacentBall(mBallsList[i][j])) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool HasAdjacentBall(GameObject ball) {

        Ball ballComponent = ball.GetComponent<Ball>();

        if (ballComponent.mRowPos - 1 >= 0 && mBallsList[ballComponent.mRowPos-1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos - 1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                return true;
            }
        } 
        if (ballComponent.mColPos - 1 >= 0 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                return true;
            }
        } 
        if (ballComponent.mRowPos + 1 <= rows - 1 && mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                return true;
            }
        }
        if (ballComponent.mColPos + 1 <= columns - 1 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                return true;
            }
        }  

        return false;
    }

    private void EndGame() {

        int historicHighScore = PlayerPrefs.GetInt("HighScore");

        if (mScore > historicHighScore) {
            PlayerPrefs.SetInt("HighScore", mScore);
            EndText.text = "New High Score!";
        }
        else {
            EndText.text = "Game Over!";
        }

        EndPanelScore.text = "Game Score = " + mScore;

        GameOverPanel.SetActive(true);

        //notify no moves left

        //Check if high score and show it

        //Ask if want to play again

    }

    public void CreateNewGameButtonClicked() {
        startNewGame = true;
    }
};
