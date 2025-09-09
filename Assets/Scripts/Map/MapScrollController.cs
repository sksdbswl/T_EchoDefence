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
    [SerializeField] private float resetScrollSpeed = 5f;
    
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
        if (GameManager.Instance.IsGameClear) return;
        
        switch (stopType)
        {
            case ScrollStopType.None:
                mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);

                break;
            
            case ScrollStopType.MoveToStart:
                mapRoot.position += Vector3.back * (resetScrollSpeed * Time.deltaTime);
                
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
                mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);
                
                if (stopZ >= StageManager.Instance.MapGenerator.GetLastEndChunk().position.z)
                {
                    isScrolling = false;
                    stopType = ScrollStopType.None;
                    Debug.Log("[Scroll] 보스존 도착 → 스크롤 정지");
                    StageManager.Instance.BossPos.GetComponent<Collider>().enabled = false;
                }
                break;
        }
    }

    private System.Action onScrollComplete; 
    
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
