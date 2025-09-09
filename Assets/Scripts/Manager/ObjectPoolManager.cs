using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    public static ObjectPoolManager Instance { get { return instance; } }
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 풀 생성 (필요한 프리팹과 개수 지정)
    /// </summary>
    public void CreatePool(GameObject prefab, int count, Transform parent)
    {
        // 이미 풀 존재하면 무시
        if (pools.ContainsKey(prefab)) return; 

        Queue<GameObject> newPool = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }

        pools.Add(prefab, newPool);
    }
    
    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!pools.ContainsKey(prefab))
        {
            // 없으면 즉시 풀 생성
            CreatePool(prefab, 1, parent); 
        }
    
        Queue<GameObject> pool = pools[prefab];
        GameObject obj;
    
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            // 부족하면 새로 생성
            obj = Instantiate(prefab, parent); 
        }
    
        obj.SetActive(true);
        obj.transform.SetPositionAndRotation(position, rotation);
    
        return obj;
    }

    /// <summary>
    /// 오브젝트를 풀로 반환
    /// </summary>
    public void ReturnToPool(GameObject prefab, GameObject obj, Transform parent)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parent);

        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new Queue<GameObject>());
        }

        pools[prefab].Enqueue(obj);
    }
    
    /// <summary>
    /// UI오브젝트를 풀로 반환
    /// </summary>
    public void ReturnToPoolUI(GameObject prefab, GameObject obj, RectTransform parent)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parent);

        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new Queue<GameObject>());
        }

        pools[prefab].Enqueue(obj);
    }
}