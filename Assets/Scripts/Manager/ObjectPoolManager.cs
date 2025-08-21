using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 풀 생성 (필요한 프리팹과 개수 지정)
    /// </summary>
    public void CreatePool(GameObject prefab, int count)
    {
        // 이미 풀 존재하면 무시
        if (pools.ContainsKey(prefab)) return; 

        Queue<GameObject> newPool = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }

        pools.Add(prefab, newPool);
    }

    /// <summary>
    /// 풀에서 오브젝트 꺼내오기
    /// </summary>
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogWarning($"풀에 {prefab.name} 이(가) 등록되지 않아 새로 생성합니다.");
            CreatePool(prefab, 1); // 없으면 즉시 풀 생성
        }

        Queue<GameObject> pool = pools[prefab];
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, transform); // 부족하면 새로 생성
        }

        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);

        return obj;
    }

    /// <summary>
    /// 오브젝트를 풀로 반환
    /// </summary>
    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new Queue<GameObject>());
        }

        pools[prefab].Enqueue(obj);
    }
}