using System;
using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int damage = 500;
    [SerializeField] public ParticleSystem hitParticle;
    [SerializeField] private LayerMask monsterLayer;

    private bool exploded = false;

    public Collider[] hitMoster;

    private void Update()
    {
        if (gameObject.transform.position.y <= 0)
        {
            Vector3 hitPos = gameObject.transform.position;
            
            hitParticle.transform.position = hitPos;
            hitParticle.gameObject.SetActive(true);
            hitParticle.Play();

            StartCoroutine(ReturnAfterParticle());
        }
    }

    private IEnumerator ReturnAfterParticle()
    {
        yield return new WaitForSeconds(0.28f);

        foreach (Collider hit in hitMoster)
        {
            Debug.Log("몬스터 감지됨: " + hit.name);
            Monster mc = hit.GetComponent<Monster>();
            
            Destroy(mc.gameObject);
        }
        
        Destroy(gameObject);
    }
}