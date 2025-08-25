using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject prefab;
    private Player Player;
    
    public void Init(GameObject prefabRef, Player player)
    {
        Player = player;
        prefab = prefabRef;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (Player.playerStat.Speed * Time.deltaTime));
        
        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(3.0f);
        
        ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Monster target = other.GetComponent<Monster>();
        
        if (target)
        {
            Debug.Log("Monster 감지 ============================================");
            var monster = other.gameObject.GetComponent<Monster>();
            
            monster.TakeDamage(Player.playerStat.Damage);

            StopCoroutine(ReturnToPool());
            ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject);
        }
        
        UnitDef unitDef = other.GetComponent<UnitDef>();
        if (unitDef)
        {
            unitDef.unitValue++;
        }
    }
}

