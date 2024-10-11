using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button[] buttons; 
    private List<int> sequence = new List<int>(); 
    private List<int> playerInput = new List<int>(); 
    private int currentRound = 0;
    private int currentIndex = 0;
    bool isShowingSequence = false;

    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI lastHighScore;
    [SerializeField] private GameDataController gameDataController;

    void Start()
    {
        StartNewRound();
    }

    public void OnButtonClick(int index)
    {
        if (isShowingSequence)
            return;

        playerInput.Add(index);


        if (playerInput[currentIndex] == sequence[currentIndex])
        {
            currentIndex++;
            HighlightCorrectButton(buttons[index]);
            if (currentIndex == sequence.Count)
            {

                currentRound++;
                gameDataController.SetCurrentScore(currentRound);
                Debug.Log("Correcto! Puntos: " + currentRound);
                StartNewRound();
            }
        }
        else
        {

            Debug.Log("Fallaste! Juego reiniciado.");
            LoseGame();
        }
    }

    private void StartNewRound()
    {
        playerInput.Clear();
        currentIndex = 0;


        int newButtonIndex = Random.Range(0, buttons.Length);
        sequence.Add(newButtonIndex);


        StartCoroutine(ShowSequence());
    }

    private IEnumerator ShowSequence()
    {
        isShowingSequence = true;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < sequence.Count; i++)
        {
            int buttonIndex = sequence[i];
            buttons[buttonIndex].GetComponent<Image>().color = Color.blue; 
            yield return new WaitForSeconds(0.5f);
            buttons[buttonIndex].GetComponent<Image>().color = Color.white; 
            yield return new WaitForSeconds(0.5f);
        }
        isShowingSequence = false;
    }

    public void HighlightCorrectButton(Button button)
    {
        StartCoroutine(ChangeColorTemporary(button, Color.green, 0.5f));
    }


    public void HighlightAllWrong()
    {
        foreach (Button button in buttons)
        {
            StartCoroutine(ChangeColorTemporary(button, Color.red, 0.5f));
        }
    }


    private IEnumerator ChangeColorTemporary(Button button, Color targetColor, float duration)
    {
        Color originalColor = button.image.color;
        button.image.color = targetColor;
        yield return new WaitForSeconds(duration);
        button.image.color = originalColor;
    }

    private void LoseGame()
    {
        HighlightAllWrong();
        loseScreen.SetActive(true);
        finalScore.text = "Score: " + currentRound;
        lastHighScore.text = "Last " + gameDataController.GetLastHighScore();
        FirebaseUser user = GameUserInfo.Instance.GetFirebaseUser();

        if (currentRound > gameDataController.currentHighScore)
            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(user.UserId).Child("score").SetValueAsync(currentRound);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void GoBackToMenu(){
        SceneManager.LoadScene("LoginAndRegister");
    }

    public void ShowLeaderBoard(){
        SceneManager.LoadScene("LeaderBoard");
    }

}
