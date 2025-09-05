using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public MonsterController MonsterController;
    [SerializeField]public MapGenerator MapGenerator;
    
    public int Stage = 1;
    
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
    
    // public void NextStageSettings(Action onComplete = null)
    // {
    //     StartCoroutine(MapGenerator.GenerateMap(() =>
    //     {
    //         // 맵 생성이 끝났을 때 실행
    //         onComplete?.Invoke();
    //     }));
    // }
    
    public void StageSettings(Action<Transform> onComplete = null)
    {
        StartCoroutine(MapGenerator.GenerateMap(startPos =>
        {
            onComplete?.Invoke(startPos);
        }));
    }
}