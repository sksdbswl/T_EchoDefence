using System;
using UnityEngine;

public class DevelopMode : MonoBehaviour
{
    public static DevelopMode Instance { get; private set; }

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
            Debug.Log("유닛 증가");
            GameManager.Instance.Units.ApplyDelta(1);
        }
        // 1 키: 유닛 +1
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("유닛 감소");

            GameManager.Instance.Units.ApplyDelta(-1);
        }
    }
}
