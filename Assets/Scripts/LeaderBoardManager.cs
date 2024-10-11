using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderBoardManager : MonoBehaviour
{
    [SerializeField] private Transform leaderBoardLabelsRoot;
    [SerializeField] private GameObject leaderBoardLabelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(5).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.ChildrenCount);
                if (snapshot.ChildrenCount > 0)
                {
                    List<DataSnapshot> orderedChildren = snapshot.Children.ToList();
                    orderedChildren.Reverse();
                    int position = 1; // Contador manual para la posición

                    foreach (DataSnapshot child in orderedChildren)
                    {
                        var r = (Dictionary<string, object>)child.Value;
                        string username = child.Child("username").Value.ToString();
                        string score = child.Child("score").Value.ToString();

                        // Instanciar el prefab y actualizar el label
                        GameObject newLabel = Instantiate(leaderBoardLabelPrefab, leaderBoardLabelsRoot);
                        newLabel.GetComponent<LeaderBoardLabel>().SetLabel(position.ToString(), username, score);

                        position++; // Aumentar la posición después de cada iteración
                    }
                }
            }
        });
    }

    public void BackToMenu(){
        SceneManager.LoadScene("LoginAndRegister");
    }
}
