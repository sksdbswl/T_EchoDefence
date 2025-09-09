// using UnityEngine;
// using UnityEngine.UI;
//
// public class MonsterHP : MonoBehaviour
// {
//     [Header("UI")]
//     [SerializeField] private Slider hpSlider;
//
//     private Transform target;       // 따라다닐 몬스터
//     private Camera mainCam;         // 실제 렌더링 카메라
//     private Vector3 offset = new Vector3(0, 0.8f, 0); // 머리 위 오프셋
//
//     [Header("Distance Scale")]
//     [SerializeField] private float minScale = 0.5f;
//     [SerializeField] private float maxScale = 1f;
//     [SerializeField] private float scaleFactor = 5f; // 카메라 거리 기준 조절
//
//     private RectTransform rectTransform;
//     private RectTransform canvasRect;
//
//     private void Awake()
//     {
//         if (hpSlider == null)
//             hpSlider = GetComponentInChildren<Slider>();
//
//         rectTransform = GetComponent<RectTransform>();
//         mainCam = Camera.main;
//
//         if (rectTransform.parent != null)
//             canvasRect = rectTransform.parent.GetComponent<RectTransform>();
//     }
//
//     private void Update()
//     {
//         if (target == null || hpSlider == null || mainCam == null || canvasRect == null)
//             return;
//
//         // 월드 좌표 → 화면 좌표
//         Vector3 worldPos = target.position + offset;
//         Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
//
//         // 카메라 뒤쪽이면 숨김
//         if (screenPos.z < 0)
//         {
//             if (hpSlider.gameObject.activeSelf)
//                 hpSlider.gameObject.SetActive(false);
//             return;
//         }
//         else
//         {
//             if (!hpSlider.gameObject.activeSelf)
//                 hpSlider.gameObject.SetActive(true);
//         }
//
//         // 화면 좌표를 Canvas localPosition으로 변환
//         Vector2 localPoint;
//         if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint))
//         {
//             rectTransform.localPosition = localPoint;
//
//             // 거리 + 카메라 줌 보정
//             float distance = Vector3.Distance(mainCam.transform.position, target.position);
//             float scale = Mathf.Clamp(scaleFactor / distance, minScale, maxScale);
//             rectTransform.localScale = Vector3.one * scale;
//         }
//     }
//
//     /// <summary>
//     /// 어떤 몬스터를 따라다닐지 세팅
//     /// </summary>
//     public void Setup(Transform followTarget)
//     {
//         target = followTarget;
//         hpSlider.value = 1f;
//     }
//
//     /// <summary>
//     /// HP 비율(0~1) 업데이트
//     /// </summary>
//     public void SetHP(float ratio)
//     {
//         if (hpSlider != null)
//             hpSlider.value = Mathf.Clamp01(ratio);
//     }
//
//     /// <summary>
//     /// 몬스터가 죽었을 때 호출 → 풀에 반납하거나 비활성화
//     /// </summary>
//     public void Release()
//     {
//         target = null;
//         hpSlider.value = 1f;
//         gameObject.SetActive(false);
//     }
// }


using UnityEngine;
using UnityEngine.UI;

public class MonsterHP : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    private Transform target;   // 따라다닐 몬스터
    private Vector3 offset = new Vector3(0, 0.8f, 0); // 머리 위 오프셋
    private Camera mainCam;

    [Header("Distance Scale")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float scaleFactor = 5f;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (hpSlider == null)
            hpSlider = GetComponentInChildren<Slider>();

        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;

        // World Space Canvas는 반드시 RectTransform 필요
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (target == null || hpSlider == null || mainCam == null) return;

        // 월드 좌표 기준 머리 위 위치
        Vector3 worldPos = target.position + offset;
        rectTransform.position = worldPos;

        // 카메라 바라보기
        rectTransform.LookAt(mainCam.transform);
        rectTransform.forward = -rectTransform.forward; // Slider가 뒤집히는 경우

        // 거리 기반 스케일
        float distance = Vector3.Distance(mainCam.transform.position, target.position);
        float scale = Mathf.Clamp(scaleFactor / distance, minScale, maxScale);
        rectTransform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// 따라갈 몬스터 세팅
    /// </summary>
    public void Setup(Transform followTarget)
    {
        target = followTarget;
        hpSlider.value = 1f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// HP 비율 업데이트
    /// </summary>
    public void SetHP(float ratio)
    {
        if (hpSlider != null)
            hpSlider.value = Mathf.Clamp01(ratio);
    }

    /// <summary>
    /// 몬스터가 죽었을 때 호출
    /// </summary>
    public GameObject Release()
    {
        target = null;
        hpSlider.value = 1f;
        gameObject.SetActive(false);

        return this.gameObject;
    }
}
