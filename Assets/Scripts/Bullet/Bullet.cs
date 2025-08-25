using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 0f;
    private GameObject prefab;

    public void Init(GameObject prefabRef, Player player)
    {
        prefab = prefabRef;
        speed = player.Speed;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(3.0f);
        
        Debug.Log($"Bullet ReturnToPool::{prefab.name}");
        
        ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject);
    // }
}

