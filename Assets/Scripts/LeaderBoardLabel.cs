using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI positionLabel;
    [SerializeField] private TextMeshProUGUI usernameLabel;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    public void SetLabel(string position, string username, string score)
    {
        positionLabel.text = position;
        usernameLabel.text = username;
        scoreLabel.text = score;
    }
}
