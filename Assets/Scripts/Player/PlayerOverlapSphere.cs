using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overlapsphere : MonoBehaviour
{
    public float radius = 0f;
    public LayerMask layer;
    public Collider[] colliders;
    
    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, radius, layer);

        foreach (var unit in colliders)
        {
            var monster = unit.GetComponent<Monster>();

            if (monster)
            {
                monster._player = GetComponent<Player>();
                monster.isDetect = true;
            }
        }
    }
   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}