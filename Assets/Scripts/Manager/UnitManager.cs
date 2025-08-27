using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Unit Pool")]
    [SerializeField] private GameObject unitPrefab;
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
    
    private Coroutine _fireLoop;
    [SerializeField] private float fireInterval = 1.5f;

    void Awake()
    {
        ObjectPoolManager.Instance.CreatePool(unitPrefab, prewarmCount, parents);
    }

    public void Init(Player owner)
    {
        _owner = owner;
    }

    public void RegisterPlayer(IMuzzleProvider playerProvider)
    {
        _playerProvider = playerProvider;        
    }

    public void ApplyDelta(int delta)
    {
        if (delta > 0) AddUnits(delta);
        else if (delta < 0) RemoveUnits(-delta);
    }

    private void AddUnits(int count)
    {
        // 현재 활성 유닛 수를 기준으로 “연속 인덱스”를 부여
        int baseIndex = _activeUnits.Count;

        for (int i = 0; i < count; i++)
        {
            Vector3 center = _owner.transform.position;

            int globalIndex = baseIndex + i; // 전체에서의 인덱스
            Vector3 pos = GetSpawnPosAroundPlayer(globalIndex, center);

            Quaternion rot = Quaternion.identity;

            var go = ObjectPoolManager.Instance.GetFromPool(unitPrefab, pos, rot, parents);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner);
            _activeUnits.Add(agent);
        }
    }

    private void RemoveUnits(int count)
    {
        for (int i = 0; i < count && _activeUnits.Count > 0; i++)
        {
            int last = _activeUnits.Count - 1;
            var agent = _activeUnits[last];
            _activeUnits.RemoveAt(last);

            agent.OnDespawn();
            ObjectPoolManager.Instance.ReturnToPool(unitPrefab, agent.gameObject, parents);
        }
    }

    /// <summary>
    /// globalIndex(0부터 시작)를 기준으로 어느 링/몇 번째 슬롯인지 계산해서 위치 반환
    /// </summary>
    private Vector3 GetSpawnPosAroundPlayer(int globalIndex, Vector3 center)
    {
        // 몇 번째 링인지/그 링의 수용량은 얼마인지 계산
        int ring = 0;
        int capacityThisRing = firstRingCapacity;
        int idxInRing = globalIndex;

        while (idxInRing >= capacityThisRing)
        {
            idxInRing -= capacityThisRing;
            ring++;
            capacityThisRing += ringCapacityStep; // 다음 링은 더 많은 슬롯
        }

        float radius = baseRadius + ring * ringGap;
        float t = (idxInRing + 0.5f) / capacityThisRing; // 0~1 분포(0.5 오프셋으로 겹침 방지)
        float angle = t * Mathf.PI * 2f;

        // 약간의 랜덤 흔들림
        float r = radius + Random.Range(-jitter, jitter);
        float x = center.x + Mathf.Cos(angle) * r;
        float z = center.z + Mathf.Sin(angle) * r;
        var pos = new Vector3(x, center.y, z);

        return pos;
    }

    // ===== (있다면) 발사 루프 =====
    public void StartFireLoop()
    {
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
            // 플레이어 스냅샷
            if (_playerProvider != null)
            {
                var pos = _playerProvider.Muzzle.transform.position;
                var rot = _playerProvider.Muzzle.rotation;
                
                GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
            }
            
            // 유닛 스냅샷
            var list = _activeUnits.ToArray();
            foreach (var agent in list)
            {
                if (agent == null || agent.Muzzle == null || _owner == null) continue;

                var pos = agent.Muzzle.position;
                var rot = agent.Muzzle.rotation;
                GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
            }

            yield return new WaitForSeconds(fireInterval);
        }
    }
}
