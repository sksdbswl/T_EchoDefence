using System;
using UnityEngine;

using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Monster Prefabs")]
    public GameObject[] Monsters;
    public GameObject[] BossMonsters;

    [Header("Settings")]
    public Transform monsterParent;
    [SerializeField] private float spawnInterval = 0.01f; // 몬스터 소환 시간
    [SerializeField] private int minMonsterCount = 50;
    [SerializeField] private int maxMonsterCount = 100;

    private GameObject stageMonsterPrefab;
    private Transform endChunkTransform;
    
    // 풀 생성
    public void CreateStageMonster(int stage)
    {
        stageMonsterPrefab = Monsters[stage - 1];
        ObjectPoolManager.Instance.CreatePool(stageMonsterPrefab, maxMonsterCount, monsterParent);
    }
    
    public void SpawnMonstersOnMap(Vector3 mapOrigin, Vector2 mapSize, Transform endChunk)
    {
        this.endChunkTransform = endChunk;
        StartCoroutine(SpawnRoutine(mapOrigin, mapSize));
    }

    private IEnumerator SpawnRoutine(Vector3 mapOrigin, Vector2 mapSize)
    {
        int spawnCount = Random.Range(minMonsterCount, maxMonsterCount + 1);

        float halfWidth = mapSize.x / 2f;
        float safeZone = 5f;

        for (int i = 0; i < spawnCount; i++)
        {
            float randZ = Random.Range(safeZone, mapSize.y);
            float randX = Random.Range(-halfWidth * 0.9f, halfWidth * 0.9f);

            Vector3 pos = mapOrigin + new Vector3(randX, 0, randZ);

            ObjectPoolManager.Instance.GetFromPool(
                stageMonsterPrefab,
                pos,
                Quaternion.Euler(0, 180f, 0),
                monsterParent
            );

            yield return new WaitForSeconds(0f);
        }

        // === 일반 몬스터 다 스폰 후 보스 생성 ===
        CreateBossMonster(endChunkTransform, StageManager.Instance.Stage);
    }


    // private IEnumerator SpawnRoutine(Vector3 mapOrigin, Vector2 mapSize)
    // {
    //     int spawnCount = Random.Range(minMonsterCount, maxMonsterCount + 1);
    //
    //     float halfWidth = mapSize.x / 2f;
    //
    //     for (int i = 0; i < spawnCount; i++)
    //     {
    //         // 맵 중앙 기준 좌우 범위 내에서만 랜덤
    //         float randX = Random.Range(-halfWidth * 0.9f, halfWidth * 0.9f); // 살짝 줄여서 안전 margin
    //         float randZ = Random.Range(0, mapSize.y);
    //
    //         Vector3 pos = mapOrigin + new Vector3(randX, 0, randZ);
    //
    //         ObjectPoolManager.Instance.GetFromPool(stageMonsterPrefab, pos, Quaternion.Euler(0,180f,0), monsterParent);
    //
    //         yield return new WaitForSeconds(spawnInterval);
    //     }
    // }
    
    public void CreateBossMonster(Transform endChunk, int stage)
    {
        Vector3 bossPos = endChunk.position;

        // 정확한 중앙 좌표 보정
        if (endChunk.TryGetComponent<Renderer>(out var rend))
            bossPos = rend.bounds.center;
        else if (endChunk.TryGetComponent<Collider>(out var col))
            bossPos = col.bounds.center;

        if (BossMonsters.Length >= stage && BossMonsters[stage - 1] != null)
        {
            Instantiate(BossMonsters[stage - 1], bossPos, Quaternion.Euler(0,180f,0), monsterParent);
            Debug.Log($"[MonsterController] Stage {stage} 보스 스폰 at {bossPos}");
        }
        else
        {
            Debug.LogWarning("[MonsterController] BossMonster 프리팹이 없습니다.");
        }
    }

}
