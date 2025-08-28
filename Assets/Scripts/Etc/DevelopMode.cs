using System;
using UnityEngine;

public class DevelopMode : MonoBehaviour
{
    public static DevelopMode Instance { get; private set; }
    public Player Player;
    
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance?.Units == null) return;

        // 1 키: 유닛 +1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Instance.Units.ApplyDelta(1);
        }
        // 2 키: 유닛 +1
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameManager.Instance.Units.ApplyDelta(-1);
        }
        // 3 키: 무기 레벨업
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Player.playerStat.WeaponLevel++;
        }
        // 4 키: 무기 레벨감소
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Player.playerStat.WeaponLevel--;
        }
        
        // 3 키: 무기 스피트 업
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Player.playerStat.Speed++;
        }
        // 4 키: 무기 스피드 다운
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Player.playerStat.Speed--;
        }
    }
}
