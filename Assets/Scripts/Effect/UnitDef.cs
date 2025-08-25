using System;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class UnitDef : MonoBehaviour
{
    public int unitValue;
    
    private void Awake()
    {
        unitValue = Random.Range(-15, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
        GameManager.Instance.Units.ApplyDelta(unitValue); 
        Destroy(gameObject);
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     var player = other.GetComponent<Player>();
    //
    //     if (player)
    //     {
    //         int incrementUnit = player.playerStat.UnitCnt + unitValue;
    //         
    //         for (int i = 0; i < incrementUnit; i++)
    //         { 
    //             Instantiate(player, transform.position, Quaternion.identity);
    //         }
    //         
    //         Destroy(gameObject);
    //     }
    // }
}
