using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public MonsterController MonsterController;
    public MapScrollController MapScrollController;
    [SerializeField]public MapGenerator MapGenerator;
    [SerializeField]public GameObject ClearPanel;
    [SerializeField]public GameObject GameOverPanel;
    
    public GameObject BossPos;
    public int Stage = 1;
    public bool StageActive = false; // 스테이지 셋팅 완료 여부 확인
    
    private void Awake()
    {
        Instance = this;
        
        MonsterController = GetComponent<MonsterController>();
        MapScrollController= GetComponent<MapScrollController>();
    }

    private void Start()
    {
        CreateStageMonster(Stage);
    }

    public void CreateStageMonster(int stage)
    {
        MapScrollController.scrollSpeed += 0.15f;
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