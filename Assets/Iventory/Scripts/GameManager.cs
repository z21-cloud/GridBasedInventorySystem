using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Input;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        instance = this;
    }
    #endregion

    public PlayerContorller playerContorller;
}
