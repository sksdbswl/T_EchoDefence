using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public MonsterController MonsterController;
    [SerializeField]public MapGenerator MapGenerator;

    public GameObject BossPos;
    public int Stage = 1;
    public bool StageActive = false; // 스테이지 셋팅 완료 여부 확인
    
    private void Awake()
    {
        Instance = this;
        
        MonsterController = GetComponent<MonsterController>();
    }

    private void Start()
    {
        CreateStageMonster(Stage);
    }

    public void CreateStageMonster(int stage)
    {
        MonsterController.CreateStageMonster(stage);
    }
    
    public void StageSettings(Action<Transform> onComplete = null)
    {
        StartCoroutine(MapGenerator.GenerateMap(startPos =>
        {
            onComplete?.Invoke(startPos);
        }));
    }
}