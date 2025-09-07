// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// #region Managers
// namespace Managers
// {
//     public class ObjectPoolManager : MonoBehaviour
//     {
//         public static ObjectPoolManager instance;
//         public static ObjectPoolManager Instance { get { return instance; } }
//         private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
//
//         private void Awake()
//         {
//             if (instance != null && instance != this)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//             instance = this;
//         }
//
//         public void CreatePool(GameObject prefab, int count, Transform parent)
//         {
//             if (pools.ContainsKey(prefab)) return;
//
//             Queue<GameObject> newPool = new Queue<GameObject>();
//             for (int i = 0; i < count; i++)
//             {
//                 GameObject obj = Instantiate(prefab, parent);
//                 obj.SetActive(false);
//                 newPool.Enqueue(obj);
//             }
//             pools.Add(prefab, newPool);
//         }
//
//         public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
//         {
//             if (!pools.ContainsKey(prefab))
//             {
//                 CreatePool(prefab, 1, parent);
//             }
//
//             Queue<GameObject> pool = pools[prefab];
//             GameObject obj = (pool.Count > 0) ? pool.Dequeue() : Instantiate(prefab, parent);
//
//             obj.SetActive(true);
//             obj.transform.SetPositionAndRotation(position, rotation);
//             return obj;
//         }
//
//         public void ReturnToPool(GameObject prefab, GameObject obj, Transform parent)
//         {
//             obj.SetActive(false);
//             obj.transform.SetParent(parent);
//
//             if (!pools.ContainsKey(prefab))
//             {
//                 pools.Add(prefab, new Queue<GameObject>());
//             }
//
//             pools[prefab].Enqueue(obj);
//         }
//     }
// }
// #endregion
//
// #region FSM
// namespace FSM
// {
//     public interface IState
//     {
//         void Enter();
//         void Exit();
//         void HandleInput();
//         void Update();
//     }
//
//     public abstract class PlayerBaseState : IState
//     {
//         protected PlayerStateMachine stateMachine;
//         public PlayerBaseState(PlayerStateMachine stateMachine) { this.stateMachine = stateMachine; }
//         public virtual void Enter() { }
//         public virtual void Exit() { }
//         public virtual void HandleInput() { }
//         public virtual void Update() { }
//     }
//
//     public abstract class StateMachine
//     {
//         protected IState currentState;
//
//         public void ChangeState(IState newState)
//         {
//             currentState?.Exit();
//             currentState = newState;
//             currentState?.Enter();
//         }
//
//         public void HandleInput()
//         {
//             currentState?.HandleInput();
//         }
//
//         public void Update()
//         {
//             currentState?.Update();
//         }
//     }
//
//     public class PlayerStateMachine : StateMachine
//     {
//         public Game.Player Player { get; }
//
//         public PrevState PrevState { get; }
//         public FightState FightState { get; }
//         public ClearState ClearState { get; }
//         public SpawnState SpawnState { get; }
//         public NextState NextState { get; }
//
//         public PlayerStateMachine(Game.Player player)
//         {
//             Player = player;
//             PrevState = new PrevState(this);
//             FightState = new FightState(this);
//             ClearState = new ClearState(this);
//             SpawnState = new SpawnState(this);
//             NextState = new NextState(this);
//         }
//     }
//
//     #region States
//     public class ClearState : PlayerBaseState
//     {
//         public ClearState(PlayerStateMachine stateMachine) : base(stateMachine) { }
//
//         public override void Enter()
//         {
//             Debug.Log("Clear State Enter");
//             stateMachine.Player.animator.SetTrigger(Game.PlayerAnimationController.Idle);
//             StageManager.Instance.Stage++;
//             StageManager.Instance.CreateStageMonster(StageManager.Instance.Stage);
//
//             StageManager.Instance.StageSettings((startPos) =>
//             {
//                 stateMachine.Player.animator.SetTrigger(Game.PlayerAnimationController.Run);
//                 var scroll = StageManager.Instance.GetComponent<MapScrollController>();
//                 scroll.ScrollUntilStartAtZero(startPos, () =>
//                 {
//                     GameManager.Instance.IsBattleClear = false;
//                     stateMachine.Player.animator.SetTrigger(Game.PlayerAnimationController.Idle);
//                     stateMachine.ChangeState(stateMachine.PrevState);
//                 });
//             });
//         }
//
//         public override void Exit() { Debug.Log("Clear State Exit"); }
//     }
//
//     public class FightState : PlayerBaseState
//     {
//         public FightState(PlayerStateMachine sm) : base(sm) { }
//
//         public override void Enter()
//         {
//             stateMachine.Player.PlayerOverlapSphere.enabled = true;
//             stateMachine.Player.animator.SetTrigger(Game.PlayerAnimationController.Run);
//             GameManager.Instance.Units.StartFireLoop();
//         }
//
//         public override void Update()
//         {
//             if (GameManager.Instance.IsBattleClear)
//                 stateMachine.ChangeState(stateMachine.ClearState);
//             else if (GameManager.Instance.IsPlayerDead)
//             {
//                 // TODO: GameOver
//             }
//         }
//
//         public override void Exit()
//         {
//             GameManager.Instance.Units.StopFireLoop();
//             stateMachine.Player.PlayerOverlapSphere.enabled = false;
//         }
//     }
//
//     public class PrevState : PlayerBaseState
//     {
//         public PrevState(PlayerStateMachine stateMachine) : base(stateMachine) { }
//
//         public override void Enter()
//         {
//             stateMachine.Player.StartCoroutine(DelayToSpawn());
//         }
//
//         private IEnumerator DelayToSpawn()
//         {
//             // 다음 스테이지 타이머 시작
//             CameraManager.Instance.StageSettingTimer();
//             stateMachine.ChangeState(stateMachine.FightState);
//
//             var scroll = StageManager.Instance.GetComponent<MapScrollController>();
//             scroll.ScrollUntilBossZone();
//         }
//
//         public override void Exit() { Debug.Log("Prev State Exit"); }
//     }
//
//     public class SpawnState : PlayerBaseState { public SpawnState(PlayerStateMachine sm) : base(sm) { } }
//     public class NextState : PlayerBaseState { public NextState(PlayerStateMachine sm) : base(sm) { } }
//     #endregion
// }
// #endregion
//
// #region Player
// namespace Game
// {
//     public static class PlayerAnimationController
//     {
//         public static readonly int Idle = Animator.StringToHash("IDLE");
//         public static readonly int Run  = Animator.StringToHash("RUN");
//     }
//
//     /// <summary>
//     /// 기준이 되는 플레이어 캐릭터
//     /// </summary>
//     public class Player : MonoBehaviour
//     {
//         public FSM.PlayerStateMachine PlayerStateMachine { get; private set; }
//         public PlayerOverlapSphere PlayerOverlapSphere { get; set; }
//         public Weapon Weapon { get; private set; }
//         public Transform Muzzle => Weapon.MuzzlePoint;
//         public Animator animator;
//         [Header("InGameSettings")]
//         public PlayerStat playerStat;
//         [SerializeField] private GameObject mergedModel;
//
//         private void Awake()
//         {
//             playerStat = new PlayerStat();
//             animator = GetComponent<Animator>();
//             PlayerOverlapSphere = GetComponent<PlayerOverlapSphere>();
//             Weapon = GetComponentInChildren<Weapon>();
//
//             PlayerStateMachine = new FSM.PlayerStateMachine(this);
//             PlayerStateMachine.ChangeState(PlayerStateMachine.PrevState);
//         }
//
//         private void Start()
//         {
//             GameManager.Instance.Units.Init(this);
//             GameManager.Instance.Units.RegisterPlayer(this);
//         }
//
//         private void Update()
//         {
//             PlayerStateMachine.Update();
//         }
//
//         public void MergeToUpgradedPlayer() => mergedModel.SetActive(true);
//         public void DivideToUpgradedPlayer() => mergedModel.SetActive(false);
//     }
//     
//     /// <summary>
//     /// 플레이어 기준으로 따라가는 유닛 생성
//     /// </summary>
//     public class UnitAgent : MonoBehaviour, IMuzzleProvider, IProvider
//     {
//         private Player _owner;
//         private PlayerStat _stats;
//         private Animator _anim;
//         public Player PlayerProvider => _owner; 
//         [SerializeField] private Transform muzzle;  // 유닛 모델의 총구 위치
//         public Transform Muzzle => muzzle;       
//
//         public void Bind(Player owner)
//         {
//             _owner = owner;
//             _stats = owner.playerStat;
//             _anim  = GetComponent<Animator>();
//             _anim.SetTrigger(PlayerAnimationController.Run);
//             gameObject.SetActive(true);
//         }
//     
//         public void OnDespawn()
//         {
//             _owner = null;
//             _stats = null;
//             gameObject.SetActive(false);
//         }
//     }
// }
// #endregion
