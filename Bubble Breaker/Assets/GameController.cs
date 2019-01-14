using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GAME_MODE{
    Standard=0,
	Continuous=1,
    Shifter=2,
    MegaShift = 3,
};

public class GameController : MonoBehaviour {
    
    public GameObject [][] mBallsList;

    public List<GameObject> mNewLine;

    public int mScore = 0;
    public GAME_MODE mGameMode = GAME_MODE.Standard;

    public UIController mUIController;

    private List<GameObject> mSelectedGroup;

    private Color white = new Color(1, 1, 1);

    private int rows = 8;
    private int columns = 15;

    private bool clickDown = false;
    private bool clickDownPrev = false;

    private bool isOnClickRunning = false;
    

    void Start() {
        mSelectedGroup = new List<GameObject>();
        mNewLine = new List<GameObject>();
        
        CreateNewGame();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit();
        }

        if (!isOnClickRunning) {
            StartCoroutine(OnClick());
        }
    }

    public string GetGameModeStr() {
        switch (mGameMode) {
            case GAME_MODE.Standard:    return "Standard";
            case GAME_MODE.Continuous:  return "Continuous";
            case GAME_MODE.Shifter:     return "Shifter";
            case GAME_MODE.MegaShift:   return "MegaShift";
            default:                    return "";
        }    
    }

    public void CreateNewGame() {
        mUIController.HideGameOver();
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
            for (int i = 0; i < rows; i++){
                mBallsList[i] = new GameObject[columns];
            }
        }

        for (int i = 0; i < mNewLine.Count; i++) {
            Destroy(mNewLine[i]);
        }
        mNewLine.Clear();

        mGameMode = (GAME_MODE) PlayerPrefs.GetInt("GameMode");
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

        if (mGameMode == GAME_MODE.Continuous || mGameMode == GAME_MODE.MegaShift) {
            GenerateNewLine();
        }
    }

    public void Exit(){
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif
    }


    public int GetScore() {
        return mScore;
    }

    public int GetGroupSelectedSize() {
        return mSelectedGroup.Count;
    }



    bool ApplyGravity() {
        bool gravityApplyied = false;
        for (int i = 0; i < columns; i++){
            while (SearchForHoleInColumn(i)) {
                gravityApplyied = true;
                MoveColumnDown(i);
            }
        }
        return gravityApplyied;
    }

    bool CompactColumns(){
        bool columnsCompacted = false;
        bool newLineMode = (mGameMode == GAME_MODE.Continuous || mGameMode == GAME_MODE.MegaShift);

        while (SearchForHoleInFirstRow() || (SearchForSpaceForNewLine() && newLineMode)) {
            columnsCompacted = true;
            MoveColumnsToRight();
            if (newLineMode) {
                ApplyNewLine();
            }
        }
        return columnsCompacted;
    }

    void ApplyNewLine() {
        for(int i = 0; i < mNewLine.Count; i++) {
            GameObject ball = Instantiate(Resources.Load("Ball")) as GameObject;

            GameObject newLineBall = mNewLine[i];
            
            ball.GetComponent<SpriteRenderer>().color = newLineBall.GetComponent<SpriteRenderer>().color;
            ball.GetComponent<Ball>().mBallType = newLineBall.GetComponent<Ball>().mBallType;

            ball.GetComponent<Ball>().mRowPos = i;
            ball.GetComponent<Ball>().mColPos = 0;
            ball.transform.localPosition = new Vector2(0,i);
            mBallsList[i][0] = ball;
            Destroy(mNewLine[i]);  
        }

        mNewLine.Clear();

        GenerateNewLine();
    }

    void GenerateNewLine() {
        int randomRows = Random.Range(1, 8);
        for (int i = 0; i < randomRows; i++){
            int random = Random.Range(0, 4);
            GameObject ball = Instantiate(Resources.Load("NewLineBall")) as GameObject;
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
            ball.transform.localPosition = new Vector2(5.25f+0.5f*i, -1);
            mNewLine.Add(ball);
        }
    }

    bool ShiftBallsRight() {
        bool ballsShifted = false;
        for (int i = 0; i < rows; i++){
            while (SearchForHoleInRow(i)){
                ballsShifted = true;
                ShiftRowRight(i);
            }
        }
        return ballsShifted;
    }

    bool SearchForHoleInRow(int row) {
        int top = -1;
        for (int i = 0; i < columns; i++) {
            if (top == -1 && mBallsList[row][i]) {
                top = i;
            } else if (top != -1 && !mBallsList[row][i]) {
                return true;
            }
        }
        return false;
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

     void ShiftRowRight(int row) {
        for (int i = columns-1; i > 0; i--) {
            if (!mBallsList[row][i]){
                for (int j = i; j>0; j--) {
                    GameObject ball = null;
                    if (mBallsList[row][j - 1]) {
                        ball = mBallsList[row][j - 1];
                        mBallsList[row][j - 1] = null;
                        mBallsList[row][j] = ball;
                        ball.GetComponent<Ball>().mColPos++;
                        ball.transform.localPosition = new Vector2(j, row);
                    }
                }
            }
        }        
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

    private bool IsUIClicked(List<RaycastResult> raycastResults) {
        for (int i = 0; i < raycastResults.Count; i++) {
            if (raycastResults[i].gameObject.layer == LayerMask.NameToLayer("UI")) {
                return true;
            }
        }
        return false;
    }

    public IEnumerator OnClick() {
        isOnClickRunning = true;
        #if UNITY_ANDROID
            if (Input.touchCount >= 1 != clickDownPrev) {
                clickDown = Input.touchCount >= 1;
            }else{
                isOnClickRunning = false;
                yield break;
            }
            
        #else
            clickDown = Input.GetMouseButtonDown(0);
        #endif

        if (clickDown) {
            Vector3 input = new Vector3(0, 0, 0);

            #if UNITY_ANDROID
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began){
                    if (touch.tapCount == 1) { 
                        input = touch.position;
                    }
                }
            #else
                input = Input.mousePosition;
            #endif

            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = input;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            bool uiHit = false;
            if (raycastResults.Count > 0) {

                uiHit = IsUIClicked(raycastResults);

                isOnClickRunning = false;
                yield break;
            }

            RaycastHit2D hit;

            Ray aux = Camera.main.ScreenPointToRay(input);
            hit = Physics2D.Raycast(aux.origin, aux.direction,
                                        Mathf.Infinity, 1 << LayerMask.NameToLayer("Default"));

            if (!uiHit) { 
                if (mSelectedGroup.Count > 0) {
                    if (hit) {
                        GameObject ball = hit.collider.gameObject;
                        if (IsBallInSelectedGroup(ball)) {
                            ApplyScore();

                            float deleteAnimationTime = DeleteSelectedGroup();
                            yield return new WaitForSeconds(deleteAnimationTime);

                            if (ApplyGravity()) {
                                yield return new WaitForSeconds(0.25f);
                            }

                            if (CompactColumns()) {
                                yield return new WaitForSeconds(0.25f);
                            }


                            if (mGameMode == GAME_MODE.Shifter || mGameMode == GAME_MODE.MegaShift){
                                 if (ShiftBallsRight()) {
                                    yield return new WaitForSeconds(0.25f);
                                }
                            }

                            if (!MovesLeft()) {
                                EndGame();
                            }
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
        }
        clickDownPrev = clickDown;
        isOnClickRunning = false;
    }

    public void SelectGroup(GameObject ball) {
        mSelectedGroup.Add(ball);

        AddAdjacentBallsToGroup(ball);

        ball.GetComponent<SpriteRenderer>().color = (ball.GetComponent<SpriteRenderer>().color + white);

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

    public float DeleteSelectedGroup() {
        float animationTime = 0f;
        while (mSelectedGroup.Count > 0){
            GameObject ball = mSelectedGroup[0];

            for (int i = 0; i < rows; i++){
                for (int j = 0; j < columns; j++){
                    if (mBallsList[i][j]) { 
                        if (mBallsList[i][j].gameObject == ball) {
                            mSelectedGroup.Remove(ball);
                            Animation animation = ball.GetComponent<Animation>();
                            animation.Play();
                            animationTime = animation.clip.length;
                            DestroyObject(mBallsList[i][j],1);
                            mBallsList[i][j] = null;
                            break;
                        }
                    }
                }
            }
        }
        return animationTime;
    }

    private void ClearSelectedGroup() {
        for (int i = 0; i < mSelectedGroup.Count; i++) {
            mSelectedGroup[i].GetComponent<SpriteRenderer>().color = (mSelectedGroup[i].GetComponent<SpriteRenderer>().color - white);
        }
        mSelectedGroup.Clear();
    }

    private void AddAdjacentBallsToGroup(GameObject ball) {
        
        Ball ballComponent = ball.GetComponent<Ball>();

        if (ballComponent.mRowPos - 1 >= 0 && mBallsList[ballComponent.mRowPos-1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos - 1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBallsToGroup(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = (ballAux.GetComponent<SpriteRenderer>().color + white);
            }
        } 
        if (ballComponent.mColPos - 1 >= 0 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos-1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBallsToGroup(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = (ballAux.GetComponent<SpriteRenderer>().color + white);
            }
        } 
        if (ballComponent.mRowPos + 1 <= rows - 1 && mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos+1][ballComponent.mColPos];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBallsToGroup(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = (ballAux.GetComponent<SpriteRenderer>().color + white);
            }
        }
        if (ballComponent.mColPos + 1 <= columns - 1 && mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1]) {
            GameObject ballAux = mBallsList[ballComponent.mRowPos][ballComponent.mColPos+1];
            if (ballAux.GetComponent<Ball>().mBallType == ballComponent.mBallType && !IsBallInSelectedGroup(ballAux)) {
                mSelectedGroup.Add(ballAux);
                AddAdjacentBallsToGroup(ballAux);
                ballAux.GetComponent<SpriteRenderer>().color = (ballAux.GetComponent<SpriteRenderer>().color + white);
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

    private bool SearchForSpaceForNewLine() {
        for (int i = 0; i < columns; i++) {
            if (!mBallsList[0][i]){
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
        mUIController.SetEndGameScore("Game Score = " + mScore);

        int ballsLeft = BallsLeft();
        int bonusScore = (ballsLeft * -20) + 100;
        if (ballsLeft < 5) {
            mUIController.SetEndBonusScore("Bonus Score = " + bonusScore);
            mScore += bonusScore;
            mUIController.SetEndTotalScore("Total Score  = " + mScore);
        }

        string endText;
        string gameModeStr = GetGameModeStr();
        

        int historicGames = PlayerPrefs.GetInt("Games" + gameModeStr, 0);
        PlayerPrefs.SetInt("Games" + gameModeStr, historicGames+1);
        int historicAverage = PlayerPrefs.GetInt("Average" + gameModeStr, 0);

        int newAverage = (historicAverage * (historicGames) + mScore) / (historicGames + 1);
        PlayerPrefs.SetInt("Average" + gameModeStr, newAverage);

        int historicHigh = PlayerPrefs.GetInt("High" + gameModeStr, 0);
        if (mScore > historicHigh) {
            PlayerPrefs.SetInt("High" + gameModeStr, mScore);
            endText = "New High Score!";
        }else {
            endText = "Game Over!";
        }
        mUIController.SetEndText(endText);

        mUIController.ShowGameOver();

    }

    public void CreateNewGameButtonClicked() {
        CreateNewGame();
    }

    public void ChangeGameMode(int value) {
        PlayerPrefs.SetInt("GameMode", value);
    }

    public void ResetStatistics() {
        PlayerPrefs.SetInt("GamesStandard", 0);
        PlayerPrefs.SetInt("AverageStandard", 0);
        PlayerPrefs.SetInt("HighStandard", 0);
                    
        PlayerPrefs.SetInt("GamesContinuous", 0);
        PlayerPrefs.SetInt("AverageContinuous", 0);
        PlayerPrefs.SetInt("HighContinuous", 0);
                    
        PlayerPrefs.SetInt("GamesShifter", 0);
        PlayerPrefs.SetInt("AverageShifter", 0);
        PlayerPrefs.SetInt("HighShifter", 0);
                    
        PlayerPrefs.SetInt("GamesMegaShift", 0);
        PlayerPrefs.SetInt("AverageMegaShift", 0);
        PlayerPrefs.SetInt("HighMegaShift", 0);
    }

    private int BallsLeft() {
        int ballsLeft = 0;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (mBallsList[i][j] != null) {
                    ballsLeft++;
                }
            }
        }
        return ballsLeft;
    }
};

