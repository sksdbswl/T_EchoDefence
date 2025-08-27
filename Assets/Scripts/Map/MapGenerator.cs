using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject bridgeChunkPrefab; // 1줄짜리 블럭 묶음 프리팹
    [SerializeField] private int height = 30; // 총 몇 줄 생성할지
    [SerializeField] private float spawnDelay = 0.01f; // 줄마다 생성 간격 시간
    [SerializeField] private float chunkSpacing = 1f; // 줄 간격 (Z축 기준)
    [SerializeField] private Transform startChunk; // 시작 초기 스폰 위치
    [SerializeField] private Transform endChunk;

    private MapScrollController mapScrollController;

    private void Awake()
    {
        var startChunkPos = Instantiate(startChunk.gameObject, transform.position,
            quaternion.identity, gameObject.transform);
        mapScrollController = GetComponent<MapScrollController>();
        startChunk = startChunkPos.transform;
    }

    private void Start()
    {
        StartCoroutine(GenerateMap());
    }

    private IEnumerator GenerateMap()
    {
        Vector3 basePos = startChunk.position;
        Vector3 endPos = basePos;

        for (int z = 3; z < height; z++) // 0은 이미 시작 줄이므로 1부터 시작
        {
            Vector3 pos = basePos + new Vector3(0, 0, z * chunkSpacing);
            Instantiate(bridgeChunkPrefab, pos, Quaternion.identity, transform);
            endPos = pos;

            yield return new WaitForSeconds(spawnDelay);
        }

        Instantiate(endChunk.gameObject, endPos, Quaternion.identity, transform);


        //mapScrollController.isScrolling = true;
    }

    private void TrySpawnEntry()
    {
        
    }

}