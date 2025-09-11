using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Unit Pool")]
    [SerializeField] private GameObject NormalUnitPrefab;
    [SerializeField] private GameObject UpgradeUnitPrefab;
    [SerializeField] private int prewarmCount = 20;
    [SerializeField] private Transform parents;
    
    [Header("Spawn Ring Settings")]
    [SerializeField] private float baseRadius = 1.0f;     
    [SerializeField] private float ringGap    = 1.0f;     
    [SerializeField] private int   firstRingCapacity = 6; 
    [SerializeField] private int   ringCapacityStep  = 6; 
    [SerializeField] private float jitter = 0.15f;        

    private Player _owner;
    private IMuzzleProvider _playerProvider; 
    private readonly List<UnitAgent> _activeUnits = new();
    public IReadOnlyList<UnitAgent> ActiveUnits => _activeUnits;

    private Coroutine _fireLoop;
    private float fireInterval = 1f;

    private bool isPlayerUpgraded = false; 
    private int _totalUnitCount = 0; // 총 유닛 수 추적

    void Awake()
    {
        ObjectPoolManager.Instance.CreatePool(NormalUnitPrefab, prewarmCount, parents);
        ObjectPoolManager.Instance.CreatePool(UpgradeUnitPrefab, prewarmCount, parents);
    }

    public void Init(Player owner)
    {
        _owner = owner;
        _totalUnitCount = _owner.playerStat.UnitCnt;
    }

    public void RegisterPlayer(IMuzzleProvider playerProvider)
    {
        _playerProvider = playerProvider;        
    }

    /// <summary>
    /// 기존 ApplyDelta 호출용 래퍼
    /// </summary>
    public void ApplyDelta(int delta)
    {
        Debug.Log($"ApplyDelta:: {delta}");
        Debug.Log($"_totalUnitCount(before):: {_totalUnitCount}");

        _totalUnitCount += delta; 
        _owner.playerStat.UnitCnt = _totalUnitCount; 

        Debug.Log($"_totalUnitCount(after):: {_totalUnitCount}");
        Debug.Log($"_owner.playerStat.UnitCnt:: {_owner.playerStat.UnitCnt}");

        ApplyTotalCount(_totalUnitCount);
    }

    /// <summary>
    /// 총 유닛 수 기준으로 재배치
    /// </summary>
    private void ApplyTotalCount(int totalCount)
    {
        if (_owner == null) return;

        // 1. 기존 유닛 제거
        foreach (var unit in _activeUnits)
        {
            unit.OnDespawn();
            ObjectPoolManager.Instance.ReturnToPool(
                unit.IsUpgraded ? UpgradeUnitPrefab : NormalUnitPrefab, 
                unit.OriginPrefab, 
                parents
            );
        }
        _activeUnits.Clear();

        // 2. 본체 업그레이드 여부 먼저 확정
        if (totalCount >= 10 && !isPlayerUpgraded)
            MergePlayer();
        else if (totalCount < 10 && isPlayerUpgraded)
            DividePlayer();

        // 3. 플레이어 본체를 제외한 나머지 유닛 수
        int remain = totalCount;
        if (isPlayerUpgraded)
            remain -= 10; // 본체 1명은 이미 업그레이드 처리됐으니 제외

        // 4. 업그레이드 유닛 / 일반 유닛 계산
        int upgradedCount = remain / 10; // 10 단위마다 업그레이드 유닛
        int normalCount   = remain % 10; // 나머지는 일반 유닛
        
        Debug.Log($"upgradedCount:: {upgradedCount}");
        Debug.Log($"normalCount:: {normalCount}");
        
        int globalIndex = 0;

        // (A) 업그레이드 유닛 생성
        for (int i = 0; i < upgradedCount; i++)
        {
            Vector3 pos = GetSpawnPosAroundPlayer(globalIndex++, _owner.transform.position);
            var go = ObjectPoolManager.Instance.GetFromPool(UpgradeUnitPrefab, pos, Quaternion.identity, parents);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner,go.gameObject);
            _activeUnits.Add(agent);
        }

        // (B) 일반 유닛 생성
        for (int i = 0; i < normalCount; i++)
        {
            Vector3 pos = GetSpawnPosAroundPlayer(globalIndex++, _owner.transform.position);
            var go = ObjectPoolManager.Instance.GetFromPool(NormalUnitPrefab, pos, Quaternion.identity, parents);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner,go.gameObject);
            _activeUnits.Add(agent);
        }
    }
    
    private void MergePlayer()
    {
        isPlayerUpgraded = true;
        _owner.MergeToUpgradedPlayer();
    }

    private void DividePlayer()
    {
        isPlayerUpgraded = false;
        _owner.DivideToUpgradedPlayer();
    }

    /// <summary>
    /// globalIndex 기준으로 어느 링/몇 번째 슬롯인지 계산
    /// </summary>
    private Vector3 GetSpawnPosAroundPlayer(int globalIndex, Vector3 center)
    {
        int ring = 0;
        int capacityThisRing = firstRingCapacity;
        int idxInRing = globalIndex;

        while (idxInRing >= capacityThisRing)
        {
            idxInRing -= capacityThisRing;
            ring++;
            capacityThisRing += ringCapacityStep;
        }

        float radius = baseRadius + ring * ringGap;
        float t = (float)idxInRing / capacityThisRing;
        float angle = t * Mathf.PI * 2f;

        float x = center.x + Mathf.Cos(angle) * radius;
        float z = center.z + Mathf.Sin(angle) * radius;
        return new Vector3(x, center.y, z);
    }

    /// <summary>
    /// 유닛 애니메이션 오버라이드
    /// </summary>
    public void SetUnitAnimation(int animationHash)
    {
        foreach (var unit in GameManager.Instance.Units.ActiveUnits)
        {
            unit.SetAnimation(animationHash);
        }
    }
    
    /// <summary>
    /// 유닛 비활성화
    /// </summary>
    public void UnitSetFalse()
    {
        foreach (var unit in GameManager.Instance.Units.ActiveUnits)
        {
            unit.gameObject.SetActive(false);
        }
    }
    
    // ===== 발사 루프 =====
    public void StartFireLoop()
    {
        fireInterval = Mathf.Clamp(1f / _owner.playerStat.Speed, 0.05f, 1f);
        if (_fireLoop != null) return;
        _fireLoop = StartCoroutine(FireTick());
    }

    public void StopFireLoop()
    {
        if (_fireLoop == null) return;
        StopCoroutine(_fireLoop);
        _fireLoop = null;
    }

    private IEnumerator FireTick()
    {
        while (true)
        {
            if (_playerProvider != null)
            {
                var pos = _playerProvider.Muzzle.transform.position;
                var rot = _playerProvider.Muzzle.rotation;
                GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
            }

            var list = _activeUnits.ToArray();
            foreach (var agent in list)
            {
                if (agent == null || agent.Muzzle == null || _owner == null) continue;
                GameManager.Instance.BulletController.Shoot(agent.Muzzle.position, agent.Muzzle.rotation, _owner);
            }

            yield return new WaitForSeconds(fireInterval);
        }
    }
}
