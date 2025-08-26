using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BulletController BulletController;
    public VirtualJoystick VirtualJoystick;
    public UnitManager Units;
    public GrenadeManager Grenade;
    
    private void Awake()
    {
        Instance = this;
        BulletController = GetComponent<BulletController>();
        Units = GetComponent<UnitManager>();
        Grenade = GetComponent<GrenadeManager>();
    }
    
    public void ExitGame()
    {
        Debug.Log("ExitGame");
        //Application.Quit();
    }
}
