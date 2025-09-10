using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject prefab;
    private Player Player;
    [SerializeField] private ParticleSystem fireParticle;
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
                //Vector3 hitPos = other.ClosestPoint(transform.position);

                // 변경
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
                {
                    hitParticle.transform.position = hit.point;

                    // 충돌 표면의 법선을 기준으로 회전
                    hitParticle.transform.rotation = Quaternion.LookRotation(hit.normal);

                    hitParticle.gameObject.SetActive(true);
                    hitParticle.Play();
                }
                
                // 기존 처리 방법
                // hitParticle.transform.position = hitPos;
                // hitParticle.transform.rotation = Quaternion.LookRotation(transform.forward);
                // hitParticle.gameObject.SetActive(true);
                //
                // fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                // hitParticle.Play();

                StartCoroutine(ReturnAfterParticle());
            }
            else
            {
                ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
            }
        }
    }
    
    private IEnumerator DisableParticleAfterPlay(ParticleSystem ps)
    {
        yield return new WaitWhile(() => ps.isPlaying);
        ps.gameObject.SetActive(false);
    }

    private IEnumerator ReturnAfterParticle()
    {
        var main = hitParticle.main;
        float duration = main.duration + main.startLifetime.constantMax;

        yield return new WaitForSeconds(duration);

        ObjectPoolManager.Instance.ReturnToPool(prefab, gameObject, GameManager.Instance.BulletController.Parents);
    }
}

