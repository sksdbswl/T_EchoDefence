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
    [SerializeField] private float baseRadius = 1.0f;     // 첫 링 반지름
    [SerializeField] private float ringGap    = 1.0f;     // 링 간격
    [SerializeField] private int   firstRingCapacity = 6; // 첫 링 수용 개수
    [SerializeField] private int   ringCapacityStep  = 6; // 링 올라갈 때마다 추가 수용량
    [SerializeField] private float jitter = 0.15f;        // 약간의 랜덤 흔들림(겹침 방지)

    private Player _owner;
    private IMuzzleProvider _playerProvider; 
    private readonly List<UnitAgent> _activeUnits = new();
    public IReadOnlyList<UnitAgent> ActiveUnits => _activeUnits;
    
    private Coroutine _fireLoop;
    private float fireInterval = 1f;

    private bool isPlayerUpgraded = false; // 플레이어 합체 상태 여부

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
    /// 유닛 수 증감
    /// </summary>
    public void ApplyDelta(int delta)
    {
        if (delta > 0) AddUnits(delta);
        else if (delta < 0) RemoveUnits(-delta);

        CheckUpgradeState();
    }

    private void AddUnits(int count)
    {
        // 1. 플레이어가 업그레이드 안되어있다면
        if (!isPlayerUpgraded)
        {
            if (count >= 10)
            {
                // 업그레이드 조건 충족 → 플레이어 업그레이드
                MergePlayer();
                count -= 10; // 유닛 10개는 합쳐져서 플레이어 강화됨
            }
        }

        // 2. 플레이어가 업그레이드된 상태라면
        if (isPlayerUpgraded)
        {
            int countUpgrade = count / 10; // 몫: 강화 유닛 수
            int countNormal  = count % 10; // 나머지: 일반 유닛 수

            // (A) 강화 유닛 추가
            for (int i = 0; i < countUpgrade; i++)
            {
                Vector3 pos = GetSpawnPosAroundPlayer(_activeUnits.Count, _owner.transform.position);
                var go = ObjectPoolManager.Instance.GetFromPool(UpgradeUnitPrefab, pos, Quaternion.identity, parents);
                var agent = go.GetComponent<UnitAgent>();
                agent.Bind(_owner);
                _activeUnits.Insert(0, agent); // 리스트 맨 앞에 강화유닛
            }

            // (B) 일반 유닛 추가
            int baseIndex = _activeUnits.Count;
            for (int i = 0; i < countNormal; i++)
            {
                Vector3 pos = GetSpawnPosAroundPlayer(baseIndex + i, _owner.transform.position);
                var go = ObjectPoolManager.Instance.GetFromPool(NormalUnitPrefab, pos, Quaternion.identity, parents);
                var agent = go.GetComponent<UnitAgent>();
                agent.Bind(_owner);
                _activeUnits.Add(agent);
            }
        }
        else
        {
            // 업그레이드 조건이 안되면 전부 일반 유닛 추가
            int baseIndex = _activeUnits.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = GetSpawnPosAroundPlayer(baseIndex + i, _owner.transform.position);
                var go = ObjectPoolManager.Instance.GetFromPool(NormalUnitPrefab, pos, Quaternion.identity, parents);
                var agent = go.GetComponent<UnitAgent>();
                agent.Bind(_owner);
                _activeUnits.Add(agent);
            }
        }
    }
    
    // private void AddUnits(int count)
    // {
    //     int baseIndex = _activeUnits.Count;
    //
    //     for (int i = 0; i < count; i++)
    //     {
    //         Vector3 center = _owner.transform.position;
    //         int globalIndex = baseIndex + i;
    //         Vector3 pos = GetSpawnPosAroundPlayer(globalIndex, center);
    //
    //         Quaternion rot = Quaternion.identity;
    //
    //         var go = ObjectPoolManager.Instance.GetFromPool(NormalUnitPrefab, pos, rot, parents);
    //         var agent = go.GetComponent<UnitAgent>();
    //         agent.Bind(_owner);
    //         _activeUnits.Add(agent);
    //     }
    //
    //     // 업그레이드 상태라면 → 10단위 체크 후 맨 앞 유닛 업그레이드
    //     if (isPlayerUpgraded && _activeUnits.Count > 0 && _activeUnits.Count % 10 == 0)
    //     {
    //         UpgradeFrontUnit();
    //     }
    // }

    private void RemoveUnits(int count)
    {
        for (int i = 0; i < count && _activeUnits.Count > 0; i++)
        {
            int last = _activeUnits.Count - 1;
            var agent = _activeUnits[last];
            _activeUnits.RemoveAt(last);

            agent.OnDespawn();
            ObjectPoolManager.Instance.ReturnToPool(NormalUnitPrefab, agent.gameObject, parents);
        }
    }

    /// <summary>
    /// 유닛 수를 확인해서 플레이어 업그레이드/다운그레이드 여부 결정
    /// </summary>
    private void CheckUpgradeState()
    {
        if (!isPlayerUpgraded && _activeUnits.Count >= 9)
        {
            // 조건 충족 → 플레이어 업그레이드
            MergePlayer();
        }
        else if (isPlayerUpgraded && _activeUnits.Count < 9)
        {
            // 조건 해제 → 플레이어 다운그레이드
            DividePlayer();
        }
    }

    private void MergePlayer()
    {
        isPlayerUpgraded = true;
        _owner.MergeToUpgradedPlayer();

        // 유닛 9개 제거
        for (int i = 0; i < 9 && _activeUnits.Count > 0; i++)
        {
            var agent = _activeUnits[0];
            _activeUnits.RemoveAt(0);
            agent.OnDespawn();
            ObjectPoolManager.Instance.ReturnToPool(NormalUnitPrefab, agent.gameObject, parents);
        }
    }

    private void DividePlayer()
    {
        isPlayerUpgraded = false;
        _owner.DivideToUpgradedPlayer();
    }

    /// <summary>
    /// 맨 앞 유닛을 업그레이드 프리팹으로 교체
    /// </summary>
    private void UpgradeFrontUnit()
    {
        var first = _activeUnits[0];
        _activeUnits.RemoveAt(0);

        first.OnDespawn();
        ObjectPoolManager.Instance.ReturnToPool(NormalUnitPrefab, first.gameObject, parents);

        Vector3 pos = GetSpawnPosAroundPlayer(0, _owner.transform.position);
        var upgradeGo = ObjectPoolManager.Instance.GetFromPool(UpgradeUnitPrefab, pos, Quaternion.identity, parents);
        var upgradeAgent = upgradeGo.GetComponent<UnitAgent>();
        upgradeAgent.Bind(_owner);
        _activeUnits.Insert(0, upgradeAgent);
    }

    /// <summary>
    /// globalIndex(0부터 시작)를 기준으로 어느 링/몇 번째 슬롯인지 계산해서 위치 반환
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

        // 슬롯 계산: 원형 정렬
        float t = (float)idxInRing / capacityThisRing;
        float angle = t * Mathf.PI * 2f;

        float r = radius; // jitter 없이 원형 고정
        float x = center.x + Mathf.Cos(angle) * r;
        float z = center.z + Mathf.Sin(angle) * r;
        return new Vector3(x, center.y, z);
    }
    
    // private Vector3 GetSpawnPosAroundPlayer(int globalIndex, Vector3 center)
    // {
    //     int ring = 0;
    //     int capacityThisRing = firstRingCapacity;
    //     int idxInRing = globalIndex;
    //
    //     while (idxInRing >= capacityThisRing)
    //     {
    //         idxInRing -= capacityThisRing;
    //         ring++;
    //         capacityThisRing += ringCapacityStep;
    //     }
    //
    //     float radius = baseRadius + ring * ringGap;
    //     float t = (idxInRing + 0.5f) / capacityThisRing;
    //     float angle = t * Mathf.PI * 2f;
    //
    //     float r = radius + Random.Range(-jitter, jitter);
    //     float x = center.x + Mathf.Cos(angle) * r;
    //     float z = center.z + Mathf.Sin(angle) * r;
    //     return new Vector3(x, center.y, z);
    // }

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
            // 플레이어 발사
            if (_playerProvider != null)
            {
                var pos = _playerProvider.Muzzle.transform.position;
                var rot = _playerProvider.Muzzle.rotation;
                GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
            }

            // 유닛 발사
            var list = _activeUnits.ToArray();
            foreach (var agent in list)
            {
                if (agent == null || agent.Muzzle == null || _owner == null) continue;
                GameManager.Instance.BulletController.Shoot(agent.Muzzle.position, agent.Muzzle.rotation, _owner);
            }

            yield return new WaitForSeconds(fireInterval);
        }
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
}
