using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Dropdown mGameModeSelector;

    // Use this for initialization
    void Start () {
        mGameModeSelector.value = PlayerPrefs.GetInt("GameMode");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
