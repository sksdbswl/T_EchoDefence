using System;
using UnityEngine;

public class Player : MonoBehaviour,IMuzzleProvider
{
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public Weapon Weapon { get; private set; }
    public Transform Muzzle => Weapon.MuzzlePoint;       
    public bool IsBattleClear = false;
    public bool IsPlayerDead = false;
    public Animator animator;
    
    [Header("InGameSettings")]
    [SerializeField] public PlayerStat playerStat;
    private void Awake()
    {
        // 초기 플레이어 설정
        playerStat = new PlayerStat();
        animator = GetComponent<Animator>();
        
        // 기본 무기 설정
        Weapon = GetComponentInChildren<Weapon>();
       
        // 초기 플레이어 생성 및 FSM 시작 선언
        PlayerStateMachine = new PlayerStateMachine(this); 
        PlayerStateMachine.ChangeState(PlayerStateMachine.PrevState);
    }

    private void Start()
    {
        GameManager.Instance.Units.Init(this);
        GameManager.Instance.Units.RegisterPlayer(this);
    }

    private void Update()
    {
        PlayerStateMachine.Update(); 
    }

    public void OnStateChanged()
    {
        
    }
}
