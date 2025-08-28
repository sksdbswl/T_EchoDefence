using UnityEngine;

public enum MonsterType
{
    Normal,
    Boss
}

public class Monster : MonoBehaviour
{
    [SerializeField]private MonsterType MonsterType;
    
    public int maxHp = 10;
    private int currentHp = 1;
    private GameObject monsterPrefab;

    public Player _player;
    public bool isDetect = false;
    public float speed = 0.005f;
    private float stopDistance = 0.2f; // 너무 가까우면 멈춤
    
    public void Init(GameObject prefabRef, Player player)
    {
        monsterPrefab = prefabRef;
    }
    
    private void Awake()
    {
        monsterPrefab = this.gameObject;
        currentHp = maxHp;
    }

    private void Update()
    {
        if (isDetect)
        {
            Vector3 dir = (_player.transform.position - transform.position).normalized;
            
            // 일정 범위에 도착하면 멈춤
            float dist = dir.magnitude;
            if (dist <= stopDistance) return;
            
            // 위치 이동
            transform.position += dir * (speed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{name} : {damage} 데미지 받음, 남은 체력 {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // private void Die()
    // {
    //     Debug.Log($"{name} 사망");
    //     
    //     // TODO :: 오브젝트 풀 적용
    //     Destroy(monsterPrefab);
    //     //ObjectPoolManager.Instance.ReturnToPool(monsterPrefab, gameObject);
    // }
    
    
    private void Die()
    {
        Debug.Log($"{name} 사망");

        if (MonsterType == MonsterType.Boss)
        {
            // === 보스 사망 처리 ===
            Debug.Log("보스가 사망했습니다. 스테이지 클리어!");

            // 남아있는 모든 몬스터 제거
            foreach (var m in FindObjectsOfType<Monster>())
            {
                if (m != this) Destroy(m.gameObject);
            }

            GameManager.Instance.IsBattleClear = true;
        }

        // 일반 몬스터 or 보스 몬스터 공통 처리
        Destroy(gameObject);
        // ObjectPoolManager.Instance.ReturnToPool(monsterPrefab, gameObject);
    }
}
