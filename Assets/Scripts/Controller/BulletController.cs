using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject[] BulletPrefabs; // 레벨별 총알 프리팹
    [SerializeField] private Vector3 BulletPos;
    
    // void Start()
    // {
    //     // 기본 총알 생성  => 현재 플레이어 레벨별 프리팹 변경 필요
    //     ObjectPoolManager.Instance.GetFromPool(BulletPrefab, BulletPos, Quaternion.identity);
    //     
    //     // 레벨별 풀 초기화
    //     for (int i = 0; i < bulletPrefabs.Length; i++)
    //     {
    //         ObjectPoolManager.Instance.InitPool(bulletPrefabs[i], 20); 
    //     }
    // }
    //
    // public void Shoot(Vector3 pos, Quaternion rot)
    // {
    //     var bulletPrefab = bulletPrefabs[currentLevel];
    //     GameObject bullet = ObjectPoolManager.Instance.GetFromPool(bulletPrefab, pos, rot);
    // }
    //
    // public void LevelUp()
    // {
    //     currentLevel = Mathf.Min(currentLevel + 1, bulletPrefabs.Length - 1);
    // }
}
