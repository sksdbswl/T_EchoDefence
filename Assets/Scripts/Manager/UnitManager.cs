using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Unit Pool")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private int prewarmCount = 20;

    private Player _owner;

    // 발사 소스들
    private IMuzzleProvider _playerProvider;     // ★ 플레이어
    private readonly List<UnitAgent> _units = new(); // ★ 유닛들

    private Coroutine _fireLoop;
    [SerializeField] private float fireInterval = 1.5f;

    void Awake()
    {
        ObjectPoolManager.Instance.CreatePool(unitPrefab, prewarmCount);
    }

    public void Init(Player owner) { _owner = owner; }

    // 플레이어 등록
    public void RegisterPlayer(IMuzzleProvider playerProvider)
    {
        _playerProvider = playerProvider;
    }

    // 유닛 수 증감
    public void ApplyDelta(int delta)
    {
        if (delta > 0) AddUnits(delta);
        else if (delta < 0) RemoveUnits(-delta);
    }

    private void AddUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var go = ObjectPoolManager.Instance.GetFromPool(unitPrefab, _owner.transform.position, Quaternion.identity);
            var agent = go.GetComponent<UnitAgent>();
            agent.Bind(_owner);
            _units.Add(agent);
        }
    }

    private void RemoveUnits(int count)
    {
        for (int i = 0; i < count && _units.Count > 0; i++)
        {
            int last = _units.Count - 1;
            var agent = _units[last];
            _units.RemoveAt(last);

            agent.OnDespawn();
            ObjectPoolManager.Instance.ReturnToPool(unitPrefab, agent.gameObject);
        }
    }

    // ===== 발사 루프 =====
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

    private IEnumerable<IMuzzleProvider> EnumerateSources()
    {
        if (_playerProvider != null && _playerProvider.Muzzle != null)
            yield return _playerProvider; // ★ 플레이어도 포함

        // 유닛들 포함
        for (int i = 0; i < _units.Count; i++)
        {
            var u = _units[i];
            if (u != null && u.Muzzle != null)
                yield return u;
        }
    }

    private IEnumerator FireTick()
    {
        while (true)
        {
            // 스냅샷 떠서 중간 변경에도 안전
            var sources = new List<IMuzzleProvider>(EnumerateSources());

            // 전원 발사 (원하면 분산도 가능)
            foreach (var src in sources)
                ShootFrom(src);

            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void ShootFrom(IMuzzleProvider src)
    {
        if (src?.Muzzle == null || _owner == null) return;

        Vector3 pos = src.Muzzle.position;
        Quaternion rot = src.Muzzle.rotation;

        GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
    }

    // ★ 수동 발사(탭/스킬 버튼용) – 플레이어만 한 발
    public void ShootOnceFromPlayer()
    {
        if (_playerProvider?.Muzzle == null || _owner == null) return;

        Vector3 pos = _playerProvider.Muzzle.position;
        Quaternion rot = _playerProvider.Muzzle.rotation;

        GameManager.Instance.BulletController.Shoot(pos, rot, _owner);
    }
}
