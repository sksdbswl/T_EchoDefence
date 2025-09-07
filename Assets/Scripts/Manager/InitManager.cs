using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitManager : MonoBehaviour
{
    [SerializeField] private Button GameStart;

    private void Awake()
    {
        GameStart.onClick.AddListener(()=> SceneManager.LoadScene("InGameScene"));
    }
}
