using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private MapScrollController mapScrollController;
    
    [SerializeField] private GameObject bridgeChunkPrefab; // 1줄짜리 블럭 묶음 프리팹
    [SerializeField] private int height = 30; // 총 몇 줄 생성할지
    [SerializeField] private float spawnDelay = 0.01f; // 줄마다 생성 간격 시간
    [SerializeField] private float chunkSpacing = 1f; // 줄 간격 (Z축 기준)
    [SerializeField] private Transform startChunk; // 시작 초기 스폰 위치
    [SerializeField] private Transform endChunk;
    private float mapWidth; // 미리 계산해둔 맵 폭
    
    private void Awake()
    {
        var startChunkPos = Instantiate(startChunk.gameObject, transform.position,
            quaternion.identity, gameObject.transform);
        mapScrollController = GetComponent<MapScrollController>();
        startChunk = startChunkPos.transform;
        
        // === Chunk 폭 계산 (한 번만) ===
        if (bridgeChunkPrefab.TryGetComponent<Renderer>(out var rend))
            mapWidth = rend.bounds.size.x;
        else if (bridgeChunkPrefab.TryGetComponent<Collider>(out var col))
            mapWidth = col.bounds.size.x;
        else
            mapWidth = 6f; // fallback
    }

    private void Start()
    {
        StartCoroutine(GenerateMap());
    }
    
    private IEnumerator GenerateMap()
    {
        Vector3 basePos = startChunk.position;
        Vector3 endPos = basePos;

        for (int z = 3; z < height; z++)
        {
            Vector3 pos = basePos + new Vector3(0, 0, z * chunkSpacing);
            Instantiate(bridgeChunkPrefab, pos, Quaternion.identity, transform);
            endPos = pos;

            yield return new WaitForSeconds(spawnDelay);
        }

        // === 실제 endChunk 인스턴스 생성 ===
        GameObject endChunkObj = Instantiate(endChunk.gameObject, endPos, Quaternion.identity, transform);
        Transform endChunkTransform = endChunkObj.transform;

        // === 몬스터 스폰 ===
        TrySpawnEntry(basePos, endChunkTransform);
    }

    private void TrySpawnEntry(Vector3 basePos, Transform endChunkTransform)
    {
        Vector2 mapSize = new Vector2(mapWidth, height * chunkSpacing);
        StageManager.Instance.MonsterController.SpawnMonstersOnMap(basePos, mapSize, endChunkTransform);
    }
}