using System;
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
    //private GameObject monsterPrefab;

    public Player _player;
    public bool isDetect = false;
    public float speed = 0.5f;
    private float stopDistance = 0.2f; // 너무 가까우면 멈춤
    
    [Header("HP UI")]
    [SerializeField] private GameObject hpBarPrefab;
    private RectTransform hpBarParentRoot; 
    private RectTransform hpParents;
    private MonsterHP hpBarScript;

    public GameObject hpBarObj;
    
    private void Awake()
    {
        Canvas worldCanvas = GameObject.Find("WorldCanvas")?.GetComponent<Canvas>();
        var hpRoot = worldCanvas.transform.Find("MonsterHpRoot"); 
        if (worldCanvas != null)
        {
            hpRoot = worldCanvas.transform.Find("MonsterHpRoot"); 
            if (hpRoot != null) hpParents = hpRoot.GetComponent<RectTransform>();
        }

        currentHp = maxHp;
    }

    private void Start()
    {
        hpBarObj = ObjectPoolManager.Instance.GetFromPool(hpBarPrefab, Vector3.zero, Quaternion.identity, hpParents);
        hpBarObj.SetActive(true);
        
        HpSetUp();
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
            
            if (transform.position.z <= _player.transform.position.z)
            {
                GameManager.Instance.Units.ApplyDelta(-1);
                Die();
            }
        }
    }

    public void HpSetUp()
    {
        //hpBarObj.GetComponent<MonsterHP>().Setup(this.transform);
        hpBarScript = hpBarObj.GetComponent<MonsterHP>();
        if(hpBarScript != null)
        {
            hpBarScript.Setup(transform);  
            hpBarScript.SetHP(1f);       
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (hpBarScript != null)
            hpBarScript.SetHP((float)currentHp / maxHp);

        if (currentHp <= 0) Die();
    }
    
    public void Die()
    {
        if (MonsterType == MonsterType.Boss)
        {
            // === 보스 사망 처리 ===
            Debug.Log("보스가 사망했습니다. 스테이지 클리어!");

            // 남아있는 모든 몬스터 제거
            foreach (var m in FindObjectsOfType<Monster>())
            {
                if (m != this) Destroy(m.gameObject);

                ReturnToPoolHP(m);
            }

            GameManager.Instance.IsStageClear = true;
        }

        // 일반 몬스터 or 보스 몬스터 공통 처리
        Destroy(gameObject);
        var hp = hpBarScript.Release();
        ObjectPoolManager.Instance.ReturnToPool(hpBarPrefab, hp, hpParents);
    }

    public void ReturnToPoolHP(Monster monster)
    {
        ObjectPoolManager.Instance.ReturnToPool(hpBarPrefab, monster.hpBarObj, hpParents);
    }
}
