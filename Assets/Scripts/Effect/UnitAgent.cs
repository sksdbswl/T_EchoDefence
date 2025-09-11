using UnityEngine;

public class UnitAgent : MonoBehaviour, IMuzzleProvider, IProvider
{
    private Player _owner;
    private PlayerStat _stats;
    private Animator _anim;
    public Player PlayerProvider => _owner; 
    [SerializeField] private Transform muzzle;  // 유닛 모델의 총구 위치
    public Transform Muzzle => muzzle;       
    
    public GameObject OriginPrefab { get; private set; }
    public bool IsUpgraded { get; private set; } = false;

    public void Bind(Player owner, GameObject originPrefab, bool isUpgraded = false)
    {
        _owner = owner;
        _stats = owner.playerStat;
        _anim  = GetComponent<Animator>();
        _anim.SetTrigger(PlayerAnimationController.Run);
        OriginPrefab = originPrefab;
        IsUpgraded = isUpgraded; 
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        _owner = null;
        _stats = null;
        IsUpgraded = false; 
        gameObject.SetActive(false);
    }

    public void SetAnimation(int animation)
    {
        _anim.SetTrigger(animation);
    }
}