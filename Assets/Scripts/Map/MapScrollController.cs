using UnityEngine;


public enum ScrollStopType
{
    None,
    MoveToStart,  // startPos가 (0,0,0)에 올 때 멈춤
    BossZone      // endPos.z <= stopZ일 때 멈춤
}

public class MapScrollController : MonoBehaviour
{
    [SerializeField] private Transform mapRoot;
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float stopZ = 9.0f; // 보스존 트리거 거리
    
    private bool isScrolling = false;
    private Transform targetStartPos; 
    private ScrollStopType stopType = ScrollStopType.None;

    void Awake()
    {
        if (!mapRoot) mapRoot = transform;
    }

    void Update()
    {
        if (!isScrolling) return;

        mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);

        switch (stopType)
        {
            case ScrollStopType.MoveToStart:
                if (targetStartPos != null && targetStartPos.position.z <= 0f)
                {
                    isScrolling = false;
                    stopType = ScrollStopType.None;
                    Debug.Log("[Scroll] 새 StartPos 도착 → 스크롤 정지");
                    onScrollComplete?.Invoke(); 
                    onScrollComplete = null;    
                }
                break;
            
            case ScrollStopType.BossZone:
                if (stopZ >= StageManager.Instance.MapGenerator.GetLastEndChunk().position.z)
                {
                    isScrolling = false;
                    stopType = ScrollStopType.None;
                    Debug.Log("[Scroll] 보스존 도착 → 스크롤 정지");
                }
                break;
        }
    }

    private System.Action onScrollComplete; 
    
    // === 외부에서 호출 ===
    public void ScrollUntilStartAtZero(Transform startPos, System.Action onComplete = null)
    {
        targetStartPos = startPos;
        stopType = ScrollStopType.MoveToStart;
        isScrolling = true;
        onScrollComplete = onComplete;
    }

    public void ScrollUntilBossZone()
    {
        stopType = ScrollStopType.BossZone;
        isScrolling = true;
    }
}


// public class MapScrollController : MonoBehaviour
// {
//     [SerializeField] private Transform mapRoot; 
//     [SerializeField] private float scrollSpeed = 2f;
//
//     private bool isScrolling = false;
//     private Transform targetStop; // 멈출 목표 (예: 새 startPos)
//     private float stopThreshold = 0.05f; // 멈출 때 오차 허용 범위
//
//     void Awake()
//     {
//         if (!mapRoot) mapRoot = transform;
//     }
//
//     void Update()
//     {
//         if (!isScrolling || targetStop == null) return;
//
//         mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);
//
//         // 목표 Z에 도달했는지 체크
//         if (targetStop.position.z <= 0f + stopThreshold)
//         {
//             isScrolling = false;
//             Debug.Log("[MapScrollController] 스크롤 정지: startPos 원점 도착");
//         }
//     }
//
//     // === 외부에서 호출 ===
//     public void ScrollUntilStartAtZero(Transform startPos)
//     {
//         targetStop = startPos;
//         isScrolling = true;
//     }
// }

// public class MapScrollController : MonoBehaviour
// {
//     [SerializeField] private Transform mapRoot; 
//     [SerializeField] private float scrollSpeed;
//     
//     public bool isMovePlayerStartPos = false;
//     public bool isScrolling = false;
//     private float stopZ = 9.0f;      // 멈출 지점 (endPos.z) : 보스 존에 감지 되는 거리
//     public GameObject endPos; 
//     private Transform targetStartPos; // 새로 생성된 startPos
//     
//     void Awake()
//     {
//         if (!mapRoot) mapRoot = transform;
//     }
//     
//     void Update()
//     {
//         if (isMovePlayerStartPos)
//         {
//                 
//         }
//         
//         if (!isScrolling) return;
//
//         mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);
//
//         // mapRoot의 z가 endPos.z 이하로 내려가면 멈춤
//         if (stopZ >= endPos.transform.position.z)
//         {
//             isScrolling = false;
//            // Debug.Log($"[MapScrollController] 맵 스크롤 정지 at z={mapRoot.position.z}");
//         }
//     }
//     
//     public void SetTargetStart(Transform startPos)
//     {
//         targetStartPos = startPos;
//     }
//     
//     // public void SetScrolling(bool on) => isScrolling = on;
//     // public float ScrollSpeed => scrollSpeed;
// }