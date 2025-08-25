using UnityEngine;

public class Monster : MonoBehaviour
{
    public int maxHp = 10;
    private int currentHp = 1;
    private GameObject monsterPrefab;
    
    public void Init(GameObject prefabRef, Player player)
    {
        monsterPrefab = prefabRef;
    }
    
    private void Awake()
    {
        currentHp = maxHp;
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
        //ObjectPoolManager.Instance.ReturnToPool(monsterPrefab, gameObject);
    }
}
