using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public VirtualJoystick VirtualJoystick;
    public CameraController CameraController;
    public UnitManager Units;
    public BulletController BulletController;
    
    public bool IsStageClear = false; 
    public bool IsGameExit = false;
    
    private void Awake()
    {
        Instance = this;
        Units = GetComponent<UnitManager>();
        BulletController = GetComponent<BulletController>();
    }
    
    public void GameClear()
    {
        StageManager.Instance.ClearPanel.SetActive(true);
        Units.UnitSetFalse();
        IsGameExit = true;
    }
    
    public void GameOver()
    {
        StageManager.Instance.GameOverPanel.SetActive(true);
        Units.SetUnitAnimation(PlayerAnimationController.Die);
        Units.StopFireLoop();
        IsGameExit = true;
    }
}
