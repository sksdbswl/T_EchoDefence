using UnityEngine;
using System.Collections;

public class GrenadeThrowController : MonoBehaviour
{
    [Header("Grenade Settings")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform spawnPoint;   // 화면 중앙 or 캐릭터 손 위치
    ///[SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 20; // 곡선 샘플 개수

    private LineRenderer lineRenderer;
    private LineRenderer circleRenderer;
    [SerializeField] private int circleResolution = 40;     // 원 세분화
    [SerializeField] private float circleRadius = 0f;     // 원 반지름
    [SerializeField] private Material circleMaterial;       // 원 표시용 머티리얼
    
    private GameObject currentGrenade;

    private Vector2 dragStart, dragEnd;
    private Vector3 p0, p1, p2;

    private void Awake()
    {
        PosCircleSettings();
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.enabled = false;
        circleRenderer.enabled = false;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    // ========================
    // 입력 처리
    // ========================
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1)) // 우클릭 시작
        {
            lineRenderer.enabled = true;
            circleRenderer.enabled = true;
            
            dragStart = Input.mousePosition;
            SpawnGrenade();
        }
        else if (Input.GetMouseButton(1)) // 드래그 중
        {
            dragEnd = Input.mousePosition;
            UpdateTrajectory();
        }
        else if (Input.GetMouseButtonUp(1)) // 던지기
        {
            lineRenderer.enabled = false;
            circleRenderer.enabled = false;
            
            ThrowGrenade();
            DetectMonster(p2);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lineRenderer.enabled = true;
                circleRenderer.enabled = true;
                
                dragStart = t.position;
                SpawnGrenade();
            }
            else if (t.phase == TouchPhase.Moved)
            {
                dragEnd = t.position;
                UpdateTrajectory();
            }
            else if (t.phase == TouchPhase.Ended)
            {
                lineRenderer.enabled = false;
                circleRenderer.enabled = false;
            
                ThrowGrenade();
                DetectMonster(p2);
            }
        }
    }

    // ========================
    // 수류탄 생성
    // ========================
    private void SpawnGrenade()
    {
        if (currentGrenade != null) Destroy(currentGrenade);

        Vector3 spawnPos = spawnPoint ? spawnPoint.position : Vector3.zero;
        currentGrenade = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);

        p0 = spawnPos; // 시작점
        lineRenderer.positionCount = 0;
    }

    // ========================
    // 궤적 미리보기
    // ========================
    
    private void UpdateTrajectory()
    {
        if (currentGrenade == null) return;

        // 드래그 벡터
        Vector2 dragVec = dragEnd - dragStart;

        // 시작점
        Vector3 p0 = spawnPoint.position;

        // 카메라의 전방 벡터를 가져와서 Y축을 0으로 만들어 수평 방향으로 고정
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // 카메라의 우측 벡터를 가져와서 Y축을 0으로 만들어 수평 방향으로 고정
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // 드래그 방향에 따라 힘 계산
        // dragVec.y: 수직 드래그는 투척 거리와 높이에 영향
        // dragVec.x: 수평 드래그는 좌우 방향에 영향
        float forwardDistance = dragVec.y * 0.05f; // 드래그 상하 이동량 → 전진 거리
        float sideDistance = dragVec.x * 0.02f;    // 드래그 좌우 이동량 → 좌우 이동 거리
        float height = dragVec.y * 0.03f;         // 드래그 상하 이동량 → 높이

        // 도착점 (p2)
        // p0를 기준으로 카메라 전방 및 우측 방향으로 이동
        p2 = p0 + (cameraForward * forwardDistance) + (cameraRight * sideDistance);
        p2.y = 0;
        
        // 중간 제어점 (p1)
        // p0와 p2의 중간에 높이를 더해 포물선 모양 생성
        Vector3 midPoint = Vector3.Lerp(p0, p2, 0.5f);
        p1 = midPoint + Vector3.up * height;

        // 궤적 그리기
        lineRenderer.positionCount = curveResolution;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 pos = BezierCurve.Quadratic(p0, p1, p2, t);
            lineRenderer.SetPosition(i, pos);
        }
        
        DrawCircle(p2);
    }
    

    // ========================
    // 던지기
    // ========================
    private void ThrowGrenade()
    {
        if (currentGrenade == null) return;

        lineRenderer.positionCount = 0; // 궤적 지우기
        StartCoroutine(ShotArrowCoroutine(currentGrenade.transform, p0, p1, p2));
        
        //currentGrenade = null;
    }

    // ========================
    // 베지어 이동 코루틴
    // ========================
    private IEnumerator ShotArrowCoroutine(Transform target, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 previousPos = p0;
        float elapsedTime = 0f;
        float Duration = 1f; // 이동 시간 (조정 가능)

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / Duration;

            Vector3 pos = BezierCurve.Quadratic(p0, p1, p2, t);
            target.position = pos;

            // 3D 공간에서 진행 방향을 바라보게 회전
            Vector3 direction = (pos - previousPos).normalized;
            if (direction != Vector3.zero)
            {
                target.rotation = Quaternion.LookRotation(direction);
            }

            previousPos = pos;
            yield return null;
        }
    }
    
    //도착점 표시
    public void DrawCircle(Vector3 center)
    {
        circleRenderer.positionCount = circleResolution;
        for (int i = 0; i < circleResolution; i++)
        {
            float angle = i * Mathf.PI * 2f / circleResolution;
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;
            circleRenderer.SetPosition(i, new Vector3(center.x + x, center.y, center.z + z));
        }
    }
    
    // 도착지점 설정
    public void PosCircleSettings()
    {
        GameObject circleObj = new GameObject("TargetCircle");
        circleRenderer = circleObj.AddComponent<LineRenderer>();
        circleRenderer.loop = true;
        circleRenderer.widthMultiplier = 0.5f;
        circleRenderer.material = circleMaterial;
        circleRenderer.positionCount = 0;
    }
    
    // 몬스터 감지
    public void DetectMonster(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, circleRadius, LayerMask.GetMask("Monster"));
        var Grenade = currentGrenade.GetComponent<Grenade>();

        Grenade.hitMoster = hits;
    }
}
