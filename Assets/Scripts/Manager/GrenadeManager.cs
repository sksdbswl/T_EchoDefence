using System;
using TMPro;
using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    private Player _owner;

    [Header("Grenade Settings")]
    [SerializeField] private GameObject GrenadePrefab;
    [SerializeField] private TMP_Text GrenadeCount;
    [SerializeField] private TrajectoryPreview preview;
    [SerializeField] private Transform startPos; // 손/총구 위치

    [Header("Aim (Drag Up/Down)")]
    [SerializeField, Min(0f)] private float baseThrowSpeed = 12f;
    [SerializeField] private float minPitchDeg = 5f;     // 최소 각도
    [SerializeField] private float maxPitchDeg = 60f;    // 최대 각도
    [SerializeField] private float minPower = 0.6f;      // 속도 배수 최소
    [SerializeField] private float maxPower = 1.6f;      // 속도 배수 최대
    [SerializeField] private float dragSensitivity = 0.003f; // 드래그 → 0~1 맵핑 스케일

    // 상태
    private bool _isAiming;           // 조준 모드 여부
    private bool _armedForThrow;      // 다음 다운 이후의 업만 투척 허용
    private Vector2 _dragAnchor;      // 드래그 기준점
    private float _aim01 = 0.5f;      // 0~1 (아래~위)
    private float _curPitchDeg;
    private float _curPowerMul;
    private Vector3 _lastVel;

    // 생성된(손에 들린) 수류탄
    private GameObject _activeGrenade;
    private Rigidbody _activeRb;

    public void Init(Player owner)
    {
        _owner = owner;
        if (startPos == null) startPos = transform;
    }

    private void Update()
    {
        if (_owner != null && GrenadeCount != null)
            GrenadeCount.text = Mathf.Max(0, _owner.playerStat.Grenade).ToString();

        if (_isAiming)
        {
            // 손에 들고 있을 때는 계속 손/총구 위치를 따라가게
            if (_activeGrenade)
            {
                _activeGrenade.transform.position = startPos.position;
                // 바라보는 방향도 최신 pitch 반영(시각적 정렬용 선택)
                _activeGrenade.transform.rotation = Quaternion.LookRotation(_lastVel.normalized, Vector3.up);
            }

            HandleAimingInput();
        }
    }

    /// <summary>
    /// UI 버튼(OnClick) 연결: 누르면 수류탄 "생성만" 하고 조준 모드 진입/취소 토글
    /// </summary>
    public void UseGrenade()
    {
        if (_isAiming)
        {
            // 이미 조준 중이면 취소
            CancelAiming(destroyGrenade: true);
            return;
        }

        if (_owner == null || _owner.playerStat.Grenade <= 0) return;

        // 1) 수류탄 생성 (손에 들린 상태: 물리/중력 비활성)
        SpawnHeldGrenade();

        // 2) 조준 모드 진입
        _isAiming = true;
        _armedForThrow = false; // 첫 Up 무시
        _dragAnchor = GetPointerPosition();
        _aim01 = 0.5f;          // 중간값 시작

        UpdateAimFrom01();
        DrawPreview();
    }

    private void SpawnHeldGrenade()
    {
        if (_activeGrenade) Destroy(_activeGrenade);

        Vector3 pos = startPos ? startPos.position : transform.position;
        Quaternion rot = Quaternion.identity;

        _activeGrenade = Instantiate(GrenadePrefab, pos, rot);
        _activeRb = _activeGrenade.GetComponent<Rigidbody>();

        if (_activeRb)
        {
            _activeRb.isKinematic = true;
            _activeRb.useGravity = false;
            _activeRb.linearVelocity = Vector3.zero;
            _activeRb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleAimingInput()
    {
        // 조준 진입 후 "새 포인터 다운"이 오면 그때부터 업 허용
        if (IsPointerDownThisFrame())
        {
            _dragAnchor = GetPointerPosition();
            _armedForThrow = true;
        }

        if (IsPointerHeld())
        {
            Vector2 cur = GetPointerPosition();
            float dy = cur.y - _dragAnchor.y;     // 위/아래 드래그
            float delta01 = dy * dragSensitivity; // 스케일 변환

            _aim01 = Mathf.Clamp01(0.5f + delta01);

            UpdateAimFrom01();
            DrawPreview();
        }

        if (_armedForThrow && IsPointerUpThisFrame())
        {
            // 던지기
            ThrowGrenade();
            CancelAiming(destroyGrenade: false); // 던졌으니 파괴는 안 함
        }
    }

    private void UpdateAimFrom01()
    {
        _curPitchDeg = Mathf.Lerp(minPitchDeg, maxPitchDeg, _aim01);
        _curPowerMul = Mathf.Lerp(minPower, maxPower, _aim01);

        // 방향: 전방 기준 pitch만 회전 (좌우 회전은 유지)
        Vector3 fwd = transform.forward;
        Vector3 right = transform.right;

        Quaternion pitchRot = Quaternion.AngleAxis(_curPitchDeg, right);
        Vector3 dir = (pitchRot * fwd).normalized;

        _lastVel = dir * (baseThrowSpeed * _curPowerMul);
    }

    private void DrawPreview()
    {
        if (preview == null) return;
        Vector3 pos = startPos ? startPos.position : transform.position;
        preview.Draw(pos, _lastVel);
    }

    private void ThrowGrenade()
    {
        if (_owner == null || _owner.playerStat.Grenade <= 0) return;
        if (!_activeGrenade || !_activeRb) return;

        // 손에서 던짐: 물리 활성화 + 초기속도 부여
        _activeGrenade.transform.position = startPos ? startPos.position : transform.position;
        _activeGrenade.transform.rotation = Quaternion.LookRotation(_lastVel.normalized, Vector3.up);

        _activeRb.isKinematic = false;
        _activeRb.useGravity = true;
        _activeRb.linearVelocity = _lastVel;

        // 인벤 개수 차감
        _owner.playerStat.Grenade--;

        // 참조 해제 (더 이상 들고 있지 않음)
        _activeGrenade = null;
        _activeRb = null;

        // 미리보기 숨김
        preview?.Hide();
    }

    private void CancelAiming(bool destroyGrenade)
    {
        _isAiming = false;
        _armedForThrow = false;

        if (destroyGrenade && _activeGrenade)
        {
            Destroy(_activeGrenade);
            _activeGrenade = null;
            _activeRb = null;
        }

        preview?.Hide();
    }

    // ===== 입력 유틸 (모바일/에디터 공용) =====
    private static Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
    }

    private static bool IsPointerDownThisFrame()
    {
        return Input.GetMouseButtonDown(0) ||
               (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    private static bool IsPointerUpThisFrame()
    {
        return Input.GetMouseButtonUp(0) ||
               (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended ||
                                         Input.GetTouch(0).phase == TouchPhase.Canceled));
    }

    private static bool IsPointerHeld()
    {
        return Input.GetMouseButton(0) ||
               (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
    }
}
