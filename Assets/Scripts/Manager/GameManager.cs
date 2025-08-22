using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame");
        //Application.Quit();
    }
}
