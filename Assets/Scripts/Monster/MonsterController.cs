using System;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public GameObject[] Monsters;
    public GameObject[] BossMonsters;
    public Transform monsterParent;

    public void CreateStageMonster(int stage)
    {
        ObjectPoolManager.Instance.CreatePool(Monsters[stage - 1], 30, monsterParent);
    }

    public void CreateBossMonster(Vector3 endPos, int stage)
    {
        // // === 보스 스폰 위치 계산 ===
        // Vector3 bossPos = endPos;                      // 기본: 마지막 줄의 중앙(x=0 가정)
        // bossPos.y += bossY;                            // y 보정
        // bossPos += bossOffset;                         // 추가 오프셋
        //
        // // endChunk에 Renderer/Collider가 있으면 Bounds.center를 사용 (보다 정확한 중앙)
        // if (endChunkGO.TryGetComponent<Renderer>(out var rend))
        //     bossPos = rend.bounds.center + new Vector3(bossOffset.x, bossY + bossOffset.y, bossOffset.z);
        // else if (endChunkGO.TryGetComponent<Collider>(out var col))
        //     bossPos = col.bounds.center + new Vector3(bossOffset.x, bossY + bossOffset.y, bossOffset.z);
        //
        // // === 보스 생성 ===
        // if (bossPrefab != null)
        // {
        //     Instantiate(bossPrefab, bossPos, Quaternion.identity, transform);
        // }
        // else
        // {
        //     Debug.LogWarning("[MapGenerator] bossPrefab이 비어있습니다.");
        // }
    }
}