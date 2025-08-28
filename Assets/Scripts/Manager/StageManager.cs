using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public MonsterController MonsterController;
    public int Stage = 1;
    
    private void Awake()
    {
        Instance = this;
        
        MonsterController = GetComponent<MonsterController>();
    }

    private void Start()
    {
        MonsterController.CreateStageMonster(Stage);
    }
    
    public void StartNextStage(Transform startChunk, Transform endChunk)
    {
        Stage++;
        Debug.Log($"===== Stage {Stage} 시작 =====");

        // 플레이어 위치 초기화
        //Player.ResetToStart(startChunk.position);

        // 카메라 초기화
        //CameraController.Instance.ResetToStart(startChunk.position);

        // 몬스터 생성
        //MonsterController.SpawnMonstersOnMap(startChunk.position, mapSize, endChunk);

        // 보스 생성은 MonsterController가 몬스터 스폰 끝난 후 자동
    }
}