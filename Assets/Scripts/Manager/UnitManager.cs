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
        _totalUnitCount = Mathf.Max(0, _totalUnitCount + delta); // 음수 방지
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
            ObjectPoolManager.Instance.ReturnToPool(unit.IsUpgraded ? UpgradeUnitPrefab : NormalUnitPrefab, unit.gameObject, parents);
        }
        _activeUnits.Clear();

        // 2. 업그레이드 여부 결정
        if (totalCount >= 10)
        {
            if (!isPlayerUpgraded)
                MergePlayer();
        }
        else
        {
            if (isPlayerUpgraded)
                DividePlayer();
        }

        // 3. 업그레이드 유닛 수 / 일반 유닛 수 계산
        int upgradedCount = isPlayerUpgraded ? totalCount / 10 : 0;
        int normalCount = isPlayerUpgraded ? totalCount % 10 : totalCount;

        // 4. 유닛 생성
        int globalIndex = 0;

        // (A) 업그레이드 유닛
        for (int i = 0; i < upgradedCount; i++)
        {
            Vector3 pos = GetSpawnPosAroundPlayer(globalIndex++, _owner.transform.position);
            var go = ObjectPoolManager.Instance.GetFromPool(UpgradeUnitPrefab, pos, Quaternion.identity, parents);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner);
            _activeUnits.Add(agent);
        }

        // (B) 일반 유닛
        for (int i = 0; i < normalCount; i++)
        {
            Vector3 pos = GetSpawnPosAroundPlayer(globalIndex++, _owner.transform.position);
            var go = ObjectPoolManager.Instance.GetFromPool(NormalUnitPrefab, pos, Quaternion.identity, parents);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner);
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
