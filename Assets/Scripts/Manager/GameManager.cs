using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BulletController BulletController;
    public VirtualJoystick VirtualJoystick;
    public UnitManager Units;
    public SkillManager Skills;
    
    private void Awake()
    {
        Instance = this;
        BulletController = GetComponent<BulletController>();
        Units = GetComponent<UnitManager>();
        Skills = GetComponent<SkillManager>();
    }
    
    public void ExitGame()
    {
        Debug.Log("ExitGame");
        //Application.Quit();
    }
}
