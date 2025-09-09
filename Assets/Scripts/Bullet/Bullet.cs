using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject prefab;
    private Player Player;
    [SerializeField] private ParticleSystem hitParticle;
    
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
        
        ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
    }

    private void OnTriggerEnter(Collider other)
    {
        int endPosLayer = LayerMask.NameToLayer("EndPos");
        if (other.gameObject.layer == endPosLayer)
        {
            ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
            return;
        }
        
        Monster target = other.GetComponent<Monster>();

        if (target)
        {
            var monster = other.GetComponent<Monster>();
            monster.TakeDamage(Player.playerStat.Damage);

            if (hitParticle != null)
            {
                Vector3 hitPos = other.ClosestPoint(transform.position);

                hitParticle.transform.position = hitPos;
                hitParticle.transform.rotation = Quaternion.LookRotation(transform.forward);
                hitParticle.gameObject.SetActive(true);
                
                hitParticle.Play();

                StartCoroutine(ReturnAfterParticle());
            }
            else
            {
                ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
            }
        }
    }

    private IEnumerator ReturnAfterParticle()
    {
        var main = hitParticle.main;
        float duration = main.duration + main.startLifetime.constantMax;

        yield return new WaitForSeconds(duration);

        ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
    }
}

