using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

public abstract class ThrowingWeapon : MonoBehaviour
{
    [Header("Throwing Weapon")]
    public float explosiondelay = 3f;      // 폭발 시간
    public float explosionRadius = 5f;     // 폭발 반경
    public float throwForce = 10f;         // 던지는 기본 힘

    [Header("Trajectory")]
    public LineRenderer trajectoryLine;    // 궤적 라인
    [SerializeField] int trajectoryLinePoint = 40;

    protected virtual void Awake()
    {
        if (!trajectoryLine) trajectoryLine = GetComponent<LineRenderer>();
        if (trajectoryLine) trajectoryLine.enabled = false;
    }

    // 라인 렌더러 설정 (원하면 머티리얼도 세팅)
    void SetupTrajectoryLine()
    {
        if (!trajectoryLine) return;
        trajectoryLine.startWidth = 0.06f;
        trajectoryLine.endWidth   = 0.06f;
        trajectoryLine.positionCount = 0;
    }

    // 궤적 시작 속도/위치 계산
    private (Vector3 velocity, Vector3 position) CalculateTrajectoryVector(Transform firePos)
    {
        Vector3 vel = firePos.forward * throwForce;
        // firePos 기준 살짝 앞/위에서 시작 (시각적으로 보기 좋게)
        Vector3 localStart = new Vector3(0f, 0.5f, 1f);
        Vector3 pos = firePos.TransformPoint(localStart);
        return (vel, pos);
    }

    // 궤적 그리기 (실시간 프리뷰)
    public void UpdateTrajectory(Transform firePos)
    {
        if (!trajectoryLine) return;
        SetupTrajectoryLine();
        trajectoryLine.positionCount = trajectoryLinePoint;

        var (vel, pos) = CalculateTrajectoryVector(firePos);
        float dt = 1f / 15f; // 샘플 타임

        for (int i = 0; i < trajectoryLinePoint; i++)
        {
            trajectoryLine.SetPosition(i, pos);
            vel += Physics.gravity * dt;
            pos += vel * dt;
        }
        trajectoryLine.enabled = true;
    }

    // 실제 던지기
    public void Throw(Transform firePos)
    {
        var (vel, pos) = CalculateTrajectoryVector(firePos);
        var rb = GetComponent<Rigidbody>();
        transform.position = pos;

        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = vel;         // ← velocity 사용
        }

        if (trajectoryLine) trajectoryLine.enabled = false;
        Explosion();
    }

    // 폭발 시퀀스 시작
    public void Explosion()
    {
        StartCoroutine(Explode());
    }

    protected abstract IEnumerator Explode();
}
