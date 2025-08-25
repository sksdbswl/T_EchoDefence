using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject[] BulletPrefabs; // 레벨별 총알 프리팹
    
    void Start()
    {
        // 기본 총알 생성 
        Debug.Log("기본 총알 생성");
        ObjectPoolManager.Instance.CreatePool(BulletPrefabs[0], 20);
    }
    
    public void Shoot(Vector3 pos, Quaternion rot, Player player)
    {
        var bulletPrefab = BulletPrefabs[player.playerStat.Level - 1];
    
        // 풀에서 꺼내오기
        var bulletObj = ObjectPoolManager.Instance.GetFromPool(bulletPrefab, pos, rot);
    
        // 꺼낸 오브젝트의 Bullet 초기화
        bulletObj.GetComponent<Bullet>().Init(bulletPrefab, player);
    }

    public void LevelUp()
    {
        // TODO : 렙업 별 프리팹 초기화
    }
}
