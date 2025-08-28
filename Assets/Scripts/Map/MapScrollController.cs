using UnityEngine;

public class MapScrollController : MonoBehaviour
{
    [SerializeField] private Transform mapRoot; 
    [SerializeField] private float scrollSpeed;
    
    public bool isScrolling = false;
    private float stopZ = 9.0f;      // 멈출 지점 (endPos.z) : 보스 존에 감지 되는 거리
    public GameObject endPos; 
    
    void Awake()
    {
        if (!mapRoot) mapRoot = transform;
    }
    
    void Update()
    {
        if (!isScrolling) return;

        mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);

        // mapRoot의 z가 endPos.z 이하로 내려가면 멈춤
        if (stopZ >= endPos.transform.position.z)
        {
            isScrolling = false;
           // Debug.Log($"[MapScrollController] 맵 스크롤 정지 at z={mapRoot.position.z}");
        }
    }
    
    // public void SetScrolling(bool on) => isScrolling = on;
    // public float ScrollSpeed => scrollSpeed;
}