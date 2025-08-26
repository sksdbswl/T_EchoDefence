using UnityEngine;

public class UnitAgent : MonoBehaviour, IMuzzleProvider, IProvider
{
    private Player _owner;
    private PlayerStat _stats;
    private Animator _anim;
    public Player PlayerProvider => _owner; 
    [SerializeField] private Transform muzzle;  // 유닛 모델의 총구 위치
    public Transform Muzzle => muzzle;       

    public void Bind(Player owner)
    {
        _owner = owner;
        _stats = owner.playerStat;
        _anim  = GetComponent<Animator>();
        _anim.SetTrigger(PlayerAnimationController.Run);
        gameObject.SetActive(true);
    }
    
    public void OnDespawn()
    {
        _owner = null;
        _stats = null;
        gameObject.SetActive(false);
    }

    // private void Update()
    // {
    //     if (_owner == null) return;
    //
    //     float moveSpeed = _owner.playerStat.Speed;
    //     Vector3 target = _owner.transform.position;
    //     transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    // }
}