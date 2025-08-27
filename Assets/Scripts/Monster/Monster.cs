using UnityEngine;

public class Monster : MonoBehaviour
{
    public int maxHp = 10;
    private int currentHp = 1;
    private GameObject monsterPrefab;

    public Player _player;
    public bool isDetect = false;
    public float speed = 0.005f;
    
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

    private void Die()
    {
        Debug.Log($"{name} 사망");
        
        // TODO :: 오브젝트 풀 적용
        Destroy(monsterPrefab);
        //ObjectPoolManager.Instance.ReturnToPool(monsterPrefab, gameObject);
    }
    
    
}
