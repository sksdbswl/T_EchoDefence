using System;
using UnityEngine;

public class Player : MonoBehaviour,IMuzzleProvider, IProvider
{
    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public PlayerOverlapSphere PlayerOverlapSphere { get; set; }
    public Weapon Weapon { get; private set; }
    public Transform Muzzle => Weapon.MuzzlePoint;    
    public Player PlayerProvider => this;
    
    public Animator animator;
    
    [Header("InGameSettings")]
    [SerializeField] public PlayerStat playerStat;
        
    private void Awake()
    {
        // 초기 플레이어 설정
        playerStat = new PlayerStat();
        animator = GetComponent<Animator>();
        PlayerOverlapSphere = GetComponent<PlayerOverlapSphere>();
        
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
    
    public void ResetToStart(Vector3 startPos)
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
    }
    
    //[SerializeField] private GameObject normalModel;
    [SerializeField] private GameObject mergedModel;

    // 유닛 증감 처리
    public void MergeToUpgradedPlayer()
    {
        Debug.Log("Merge player:: 외형 변화");
        
        mergedModel.SetActive(true);
    }
    
    public void DivideToUpgradedPlayer()
    {
        mergedModel.SetActive(false);
        // playerStat.Damage *= 3; // 강화 예시
        // playerStat.Speed  *= 2;
    }
}
