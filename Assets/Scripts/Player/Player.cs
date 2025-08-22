using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public Weapon Weapon { get; private set; }
    public bool IsBattleClear = false;
    public bool IsPlayerDead = false;

    [Header("InGameSettings")]
    public int Level = 1;
    public float Speed = 3f;
    
    private void Awake()
    {
        // 기본 무기 설정
        Weapon = GetComponentInChildren<Weapon>(); 
        
        Debug.Log($"Weapon ==========================================================:: {Weapon}" );
        // 초기 플레이어 생성 및 FSM 시작 선언
        PlayerStateMachine = new PlayerStateMachine(this); 
        PlayerStateMachine.ChangeState(PlayerStateMachine.PrevState);
    }
    
    private void Update()
    {
        PlayerStateMachine.Update(); 
    }
}
