using UnityEngine;
using System.Collections;

public enum BALL_TYPE{
    Type0 = 0,
    Type1 = 1,
    Type2 = 2,
    Type3 = 3,
    Type4 = 4,
};

public class Ball : MonoBehaviour {



    public BALL_TYPE mBallType;
    public int mRowPos = -1;
    public int mColPos = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
