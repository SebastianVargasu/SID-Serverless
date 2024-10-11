using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDataController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userDisplayName;
    [SerializeField] private TextMeshProUGUI userCurrentScore;
    [SerializeField] private TextMeshProUGUI userMaxScore;
    public int currentHighScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        userDisplayName.text = GameUserInfo.Instance.GetuserName();
        GameUserInfo.Instance.GetuserHighScore(SetHighScore);
    }

    private void SetHighScore(string score)
    {
        currentHighScore = int.Parse(score);
        userMaxScore.text = "HighScore: " + score;
    }

    public void SetCurrentScore(int score){
        userCurrentScore.text = "Score: " + score;
    }

    public string GetLastHighScore(){
        return userMaxScore.text;
    }
}
