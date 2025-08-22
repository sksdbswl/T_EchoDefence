using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine PlayerStateMachine { get; private set; }

    private void Awake()
    {
        // 초기 플레이어 생성 및 FSM 시작 선언
        PlayerStateMachine = new PlayerStateMachine(this); 
        PlayerStateMachine.ChangeState(PlayerStateMachine.PrevState);
    }
}
