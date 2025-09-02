using UnityEngine;

/// <summary>
/// 버튼으로 수류탄을 (0,0,0)에 생성하고,
/// 마우스 우클릭 드래그(위/아래)로 피치/파워를 조절하며 궤적을 그리고,
/// 우클릭을 떼면 실제로 던지는 컨트롤러.
/// </summary>
public class GrenadeThrowController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject grenadePrefab;   // 수류탄 프리팹(ThrowingWeapon + Rigidbody + LineRenderer)
    [SerializeField] private Camera cam;                 // 메인 카메라 (없으면 자동 할당)

    [Header("Fire Origin")] [Tooltip("던지기 기준점(회전/방향만 사용). 비워두면 (0,0,0)에서 동적 생성.")] [SerializeField]
    private Transform firePos;        // 기준 트랜스폼

    [Header("Aim (Right Mouse Drag Up/Down)")]
    [SerializeField] private float baseThrowForce = 12f; // ThrowingWeapon.throwForce에 적용
    [SerializeField] private float minPitchDeg = 5f;
    [SerializeField] private float maxPitchDeg = 60f;
    [SerializeField] private float minPowerMul = 0.6f;   // throwForce 배수 최소
    [SerializeField] private float maxPowerMul = 1.6f;   // throwForce 배수 최대
    [SerializeField] private float dragSensitivity = 0.003f;

    // 상태
    private GameObject _activeGrenadeGO;
    private ThrowingWeapon _activeWeapon;
    private Rigidbody _activeRb;

    private bool _isAiming;           // 우클릭 드래그 중 여부
    private Vector2 _dragAnchor;      // 드래그 기준 스크린 좌표
    private float _aim01 = 0.5f;      // 0~1 (아래~위) 맵
    private float _curPitchDeg;
    private float _curPowerMul;

    // 초기화
    private void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!firePos)
        {
            var go = new GameObject("FirePos (Runtime)");
            firePos = go.transform;
            firePos.position = new Vector3(0,2,0); 
        }
    }

    private void Update()
    {
        if (!_activeWeapon) return;

        // 우클릭 시작 → 조준 시작
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("조준시작");
            _isAiming = true;
            // 마우스 드래그 감지 ( 세로 이동량 )
            _dragAnchor = Input.mousePosition; 

            // 시작 시 기본값
            _aim01 = 0.5f;
            UpdateAimFromDrag(0f);
            ApplyAimToFirePos();
            UpdatePreview();
        }

        // 우클릭 드래그 중 → 궤적 업데이트
        if (_isAiming && Input.GetMouseButton(1))
        {
            Debug.Log("궤적 업데이트");
            Vector2 cur = Input.mousePosition;
            float dy = cur.y - _dragAnchor.y;
            UpdateAimFromDrag(dy);
            ApplyAimToFirePos();
            UpdatePreview();

            // 손에 들려있는 동안은 기준 위치를 따라감
            HoldGrenadeAtOrigin();
        }

        // 우클릭 해제 → 실제 던지기
        if (_isAiming && Input.GetMouseButtonUp(1))
        {
            Debug.Log("투척");
            _isAiming = false;
            ApplyAimToFirePos();
            ThrowNow();
        }
    }

    /// <summary>
    /// UI 버튼 OnClick에 연결: 수류탄 생성만 / 정지 상태
    /// </summary>
    public void ActivateGrenade()
    {
        // 이전 것이 남아있다면 정리
        if (_activeGrenadeGO) Destroy(_activeGrenadeGO);
        _isAiming = false;

        // (0,1,0)에 생성
        _activeGrenadeGO = Instantiate(grenadePrefab, new Vector3(0,1,0), Quaternion.identity);
        _activeWeapon = _activeGrenadeGO.GetComponent<ThrowingWeapon>();
        _activeRb = _activeGrenadeGO.GetComponent<Rigidbody>();

        if (!_activeWeapon)
        {
            Debug.LogError("[GrenadeThrowController] ThrowingWeapon 컴포넌트가 필요합니다.");
            return;
        }

        // 초기 물리 정지
        if (_activeRb)
        {
            _activeRb.isKinematic = true;
            _activeRb.useGravity = false;
            //_activeRb.linearVelocity = Vector3.zero;
            //_activeRb.angularVelocity = Vector3.zero;
        }

        // 기준 회전(카메라 전방의 수평 투영을 기본 yaw로)
        Vector3 camFwdXZ = Vector3.ProjectOnPlane(cam ? cam.transform.forward : Vector3.forward, Vector3.up).normalized;
        if (camFwdXZ.sqrMagnitude < 1e-4f) camFwdXZ = Vector3.forward;

        firePos.position = Vector3.zero;
        firePos.rotation = Quaternion.LookRotation(camFwdXZ, Vector3.up);

        // 무기 기본 힘 세팅
        _activeWeapon.throwForce = baseThrowForce;

        // 궤적(우클릭 전이므로 숨김)
        if (_activeWeapon.trajectoryLine) _activeWeapon.trajectoryLine.enabled = false;

        // 손에 들려있는 동안은 (0,0,0)에 고정 표시
        HoldGrenadeAtOrigin();
    }

    private void HoldGrenadeAtOrigin()
    {
        if (_activeGrenadeGO)
        {
            //_activeGrenadeGO.transform.position = Vector3.zero;
            // 시각적으로 방향 맞추고 싶으면 아래 라인 사용
            _activeGrenadeGO.transform.rotation = firePos.rotation;
        }
    }

    private void UpdateAimFromDrag(float dy)
    {
        float delta01 = dy * dragSensitivity; // dragSensitivity 민감도 보정
        _aim01 = Mathf.Clamp01(0.5f + delta01);
        _curPitchDeg = Mathf.Lerp(minPitchDeg, maxPitchDeg, _aim01);
        _curPowerMul = Mathf.Lerp(minPowerMul, maxPowerMul, _aim01);

        // 무기 힘 갱신(배수 반영)
        if (_activeWeapon) _activeWeapon.throwForce = baseThrowForce * _curPowerMul;
    }

    private void ApplyAimToFirePos()
    {
        // yaw는 유지(카메라 수평 전방), pitch만 올림
        Vector3 fwdXZ = firePos.forward; // 현재 yaw 유지
        Vector3 right = Vector3.Cross(Vector3.up, Vector3.Cross(fwdXZ, Vector3.up)).normalized; // 수평 fwd 기준의 right
        if (right.sqrMagnitude < 1e-4f) right = Vector3.right;

        Quaternion pitchRot = Quaternion.AngleAxis(_curPitchDeg, right);
        Vector3 dir = (pitchRot * fwdXZ).normalized;

        firePos.position = Vector3.zero;
        firePos.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private void UpdatePreview()
    {
        if (_activeWeapon) _activeWeapon.UpdateTrajectory(firePos);
    }

    private void ThrowNow()
    {
        if (!_activeWeapon) return;

        // 던지기 (물리 ON + 초기속도)
        _activeWeapon.Throw(firePos);

        // 더 이상 컨트롤하지 않음
        _activeGrenadeGO = null;
        _activeWeapon = null;
        _activeRb = null;
    }
}
