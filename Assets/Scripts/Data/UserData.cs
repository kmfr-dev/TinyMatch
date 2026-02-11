using Firebase.Firestore;
using UnityEngine;

[FirestoreData]
public class UserData
{
    [FirestoreProperty] public int HighScore { get; set; }

    public UserData()
    {
        HighScore = 0;
    }
}
