using System;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager mInstance { get; private set; }

    private DatabaseReference mDatabaseReference;
    private FirebaseAuth mAuth = null;
    private FirebaseUser mCurrentUser = null;
    
    [SerializeField]
    private string mDBURL= string.Empty;

    private bool mIsInit = false;

    private void Awake()
    {
        if (null == mInstance)
        {
            InitFireBase();
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //InitFireBase();

    }

    // FireBase 초기화
    private void InitFireBase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {

            if (task.Result == DependencyStatus.Available)
            {
                mAuth = FirebaseAuth.DefaultInstance;
                mDatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("FireBase 준비완료");

                SignIn();
            }

            else
            {
                Debug.LogError($"Firebase 초기화 실패: {task.Result}");

            }
        });
    }

    // 익명 로그인
    private void SignIn()
    {
        mAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                mCurrentUser = task.Result.User;
                mIsInit = true;

                Debug.Log($"익명 로그인 성공 ! UID : {mCurrentUser.UserId}");
            }
            else
            {
                Debug.LogError("익명 로그인 실패!");
            }
        });
    }

    // 점수 저장 ( 높은 점수일 때만 )
    public void SaveScore(int newScore, Action<bool> callback = null)
    {
        if (!mIsInit)
        {
            Debug.LogError("SaveScroe : Firebase가 초기화 되지 않았습니다!");
            callback?.Invoke(false);
            return;
        }

        string userId = mCurrentUser.UserId;

        // 먼저 현재 저장된 점수를 확인한다.
        LoadScore(currentScore =>
        {
            if(newScore > currentScore)
            {
                mDatabaseReference.Child("users").Child(userId).Child("score").SetValueAsync(newScore)
                .ContinueWithOnMainThread(task =>
                {
                    if(task.IsCompleted)
                    {
                        Debug.Log($"점수 저장 성공! {currentScore} -> {newScore}");
                        callback.Invoke(true);
                    }

                    else
                    {
                        Debug.LogError("점수 저장 실패!");
                        callback.Invoke(false);
                    }
                });
            }

            else
            {
                Debug.Log($"기존 점수({currentScore})가 더 높거나 같습니다. 저장 불가");
                callback?.Invoke(false);
            }
        });
    }

    public void LoadScore(Action<int> callback)
    {
        if (!mIsInit)
        {
            Debug.LogError("SaveScroe : Firebase가 초기화 되지 않았습니다!");
            callback?.Invoke(0);
            return;
        }

        string userId = mCurrentUser.UserId;
        mDatabaseReference.Child("users").Child(userId).Child("score").GetValueAsync()
           .ContinueWithOnMainThread(task =>
           {
               if(task.IsCompleted)
               {
                   DataSnapshot snap = task.Result;
                   
                   if(snap.Exists)
                   {
                       int score = int.Parse(snap.Value.ToString());
                       Debug.Log($"점수 불러오기 성공 : {score}점");
                       callback?.Invoke(score);
                   }
                   else
                   {
                       Debug.Log("저장된 점수가 없습니다. 0점으로 시작합니다.");
                       callback.Invoke(0);
                   }
               }
               else
               {
                   Debug.LogError("점수 불러오기 실패");
                   callback?.Invoke(0);
               }
           });
    }
}
