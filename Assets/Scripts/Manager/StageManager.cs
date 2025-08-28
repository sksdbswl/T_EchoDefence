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
        MonsterController.CreateStageMonster(Stage);
    }
    
    public void NextStageSettings()
    {
        StartCoroutine(MapGenerator.GenerateMap());
    }
}