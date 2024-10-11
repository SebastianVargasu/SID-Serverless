using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class GameUserInfo : MonoBehaviour
{
    public static GameUserInfo Instance { get; private set; }
    FirebaseUser user;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SetUser(FirebaseUser user)
    {
        this.user = user;
    }
    public FirebaseUser GetFirebaseUser(){
        return user;
    }

    public string GetuserName()
    {
        return user.DisplayName;
    }

    public void GetuserHighScore(Action<string> onGetScore)
    {
        string score = "";
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(user.UserId).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);
                onGetScore("0");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Value == null)
                {
                    onGetScore("0");
                }
                else
                {
                    score = snapshot.Value.ToString();
                    onGetScore(score);
                }

            }
        });


    }


}
