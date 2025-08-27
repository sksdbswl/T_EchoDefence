using UnityEngine;

public class MapScrollController : MonoBehaviour
{
    [SerializeField] private Transform mapRoot; 
    [SerializeField] private float scrollSpeed = 1f;  // 일정 속도
    
    public bool isScrolling = false;
    
    void Awake()
    {
        if (!mapRoot) mapRoot = transform;
    }
    
    void Update()
    {
        if (!isScrolling) return;
        mapRoot.position += Vector3.back * (scrollSpeed * Time.deltaTime);
    }
    
    // public void SetScrolling(bool on) => isScrolling = on;
    // public float ScrollSpeed => scrollSpeed;
}