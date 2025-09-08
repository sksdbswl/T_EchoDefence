using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public VirtualJoystick VirtualJoystick;
    public UnitManager Units;
    public BulletController BulletController;
    
    public bool IsBattleClear = false;
    public bool IsPlayerDead = false;
    public bool IsBossZoneTrigger = false;
    
    private void Awake()
    {
        Instance = this;
        Units = GetComponent<UnitManager>();
        BulletController = GetComponent<BulletController>();
    }
    
    public void ExitGame()
    {
        Debug.Log("ExitGame");
        //Application.Quit();
    }
}
