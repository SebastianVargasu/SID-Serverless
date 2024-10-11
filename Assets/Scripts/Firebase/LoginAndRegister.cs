
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginAndRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private Button logOutButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button LoginButton;
    [SerializeField] private Button RegisterButton;
    [SerializeField] private Button ResetpasswordButton;
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        InitializeFirebase();
    }

    public void GoToPlay()
    {
        SceneManager.LoadScene("PlayScene");
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            Debug.Log(auth.CurrentUser);
            Debug.Log("arriba");
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                logOutButton.interactable = false;
                playButton.interactable = false;

                LoginButton.interactable = true;
                RegisterButton.interactable = true;
                ResetpasswordButton.interactable = true;
                if (GameUserInfo.Instance != null)
                {
                    GameUserInfo.Instance.SetUser(null);
                }
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);

                logOutButton.interactable = true;
                playButton.interactable = true;
                LoginButton.interactable = false;
                RegisterButton.interactable = false;
                ResetpasswordButton.interactable = false;

                if (GameUserInfo.Instance != null)
                {
                    GameUserInfo.Instance.SetUser(user);
                }
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
            }
        }
        else if (auth.CurrentUser == null && user == null)
        {
            logOutButton.interactable = false;
            playButton.interactable = false;

            LoginButton.interactable = true;
            RegisterButton.interactable = true;
            ResetpasswordButton.interactable = true;
        }
    }

    public void RegisterUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);

                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            UserProfile userProfile = new UserProfile { DisplayName = userNameInput.text };
            result.User.UpdateUserProfileAsync(userProfile).ContinueWith(task2 =>
            {
                Debug.Log("Firebase user created successfully:" + result.User.DisplayName + " " + result.User.UserId);
                FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(result.User.UserId).Child("username").SetValueAsync(result.User.DisplayName);
            });

        });
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);

                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

        });
    }

    public void ResetPassword()
    {
        auth.SendPasswordResetEmailAsync(emailInput.text).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                logText.text = "SendPasswordResetEmailAsync was canceled.";

                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception.Message);
                return;
            }

            Debug.Log("Successfully sent password reset email.");
        });
    }

    public void LogOut()
    {
        auth.SignOut();
    }
    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
