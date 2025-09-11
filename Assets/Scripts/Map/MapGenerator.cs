using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // 초기 설정
    [SerializeField] private GameObject bridgeChunkPrefab; // 1줄짜리 블럭 묶음 프리팹
    [SerializeField] private int height = 50; // 총 몇 줄 생성할지
    [SerializeField] private float spawnDelay = 0.01f; // 줄마다 생성 간격 시간
    [SerializeField] private float chunkSpacing = 1f; // 줄 간격 (Z축 기준)
    [SerializeField] private Transform startChunk; // 시작 초기 스폰 위치
    [SerializeField] private Transform endChunk;
    [SerializeField] private float endChunkOffsetZ = 1f;
    [SerializeField] private float startChunkOffsetZ = 4f;
    
    private float mapWidth; // 미리 계산해둔 맵 폭
    private Transform lastEndChunk; // 이어붙일 기준
    
    [SerializeField] private GameObject[] itemPrefabs; // 드롭 가능한 아이템 목록
    //[SerializeField, Range(0f, 1f)] private float itemSpawnChance = 0.05f; // 아이템 확률
    
    private void Awake()
    {
        //PlayerSpawnedPos();
        
        // === Chunk 폭 계산 (한 번만) ===
        if (bridgeChunkPrefab.TryGetComponent<Renderer>(out var rend))
            mapWidth = rend.bounds.size.x;
        else if (bridgeChunkPrefab.TryGetComponent<Collider>(out var col))
            mapWidth = col.bounds.size.x;
        else
            mapWidth = 6f; 
    }

    private void Start()
    {
        StartCoroutine(GenerateMap());
    }
    
    public IEnumerator GenerateMap(Action<Transform> onComplete = null)
    {
        // === StartChunk 먼저 생성 ===
        Vector3 spawnPos;
        if (lastEndChunk == null)
        {
            spawnPos = Vector3.zero; // 첫 스테이지
        }
        else
        {
            // EndChunk 뒤에서 약간 더 떨어져서 시작
            float offset = chunkSpacing * startChunkOffsetZ;
            spawnPos = lastEndChunk.position + new Vector3(0, 0, offset);
        }

        var startChunkObj = Instantiate(startChunk.gameObject, spawnPos, Quaternion.identity, transform);
        startChunk = startChunkObj.transform;

        // === basePos는 startChunk 기준 ===
        Vector3 basePos = startChunk.position;
        Vector3 endPos = basePos;

        // === Bridge 생성 ===
        for (int z = 1; z < height; z++) // z=1부터: 이미 start 있음
        {
            Vector3 pos = basePos + new Vector3(0, 0, z * chunkSpacing);
            Instantiate(bridgeChunkPrefab, pos, Quaternion.identity, transform);
            endPos = pos;

            // === 아이템 생성 ===
            // Z좌표가 20의 배수일 때 아이템 라인 생성
            if (z % 5 == 0)
            {
                int lineIndex = z / 5;
                SpawnItemLine(basePos, lineIndex);
            }
            
            yield return new WaitForSeconds(spawnDelay);
        }
        
        // === EndChunk 생성 ===
        Vector3 endChunkPos = endPos + new Vector3(0, 0, chunkSpacing + endChunkOffsetZ);
        GameObject endChunkObj = Instantiate(endChunk.gameObject, endChunkPos, Quaternion.identity, transform);
        Transform endChunkTransform = endChunkObj.transform;
        StageManager.Instance.BossPos = endChunkObj;
        StageManager.Instance.BossPos.GetComponent<Collider>().enabled = true;
        
        // === 몬스터 스폰 ===
        yield return StartCoroutine(TrySpawnEntry(basePos, endChunkTransform));

        // 다음 스테이지 이어붙이기 기준 갱신
        lastEndChunk = endChunkTransform;
        
        onComplete?.Invoke(startChunk);
    }
    
    [SerializeField] private float itemSpacingZ = 5f; // Z 간격
    [SerializeField] private float itemOffsetX = 0.5f; // 좌/우 아이템 간격
    [SerializeField] private int maxItemLines = 10;    // 최대 라인 수 (안전장치)

    private void SpawnItemLine(Vector3 basePos, int lineIndex)
    {
        if (lineIndex >= maxItemLines) return;

        // 라인의 Z 위치
        float zPos = basePos.z + lineIndex * itemSpacingZ;

        // 좌/우 X 위치
        float[] xOffsets = { -itemOffsetX, +itemOffsetX };

        foreach (float x in xOffsets)
        {
            Vector3 pos = new Vector3(x, basePos.y + 0.05f, zPos);

            // 랜덤 아이템 선택
            int idx = UnityEngine.Random.Range(0, itemPrefabs.Length);
            GameObject prefab = itemPrefabs[idx];

            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }
    
    private IEnumerator TrySpawnEntry(Vector3 basePos, Transform endChunkTransform)
    {
        Vector2 mapSize = new Vector2(mapWidth, height * chunkSpacing);
        yield return StartCoroutine(StageManager.Instance.MonsterController.SpawnMonstersOnMap(basePos, mapSize, endChunkTransform));
    }
    
    public Transform GetLastEndChunk()
    {
        return lastEndChunk;
    }
}