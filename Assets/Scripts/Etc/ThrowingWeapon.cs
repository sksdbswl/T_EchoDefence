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

    // 라인 렌더러 설정 (머티리얼도 세팅)
    void SetupTrajectoryLine()
    {
        if (!trajectoryLine) return;
        trajectoryLine.startWidth = 0.06f;
        trajectoryLine.endWidth   = 0.06f;
        trajectoryLine.positionCount = 0;
    }

    // 궤적 시작 속도/위치 계산
    private (Vector3 velocity, Vector3 position) CalculateTrajectoryVector(Transform firePos, float force, float angleOffset)
    {
        // angleOffset만큼 pitch 조정 (위아래 각도)
        Vector3 dir = Quaternion.AngleAxis(angleOffset, firePos.right) * firePos.forward;
        Vector3 vel = dir * force;

        Vector3 localStart = new Vector3(0f, 0.5f, 1f);
        Vector3 pos = firePos.TransformPoint(localStart);
        return (vel, pos);
    }
    
    // private (Vector3 velocity, Vector3 position) CalculateTrajectoryVector(Transform firePos)
    // {
    //     Vector3 vel = firePos.forward * throwForce;
    //     // firePos 기준 살짝 앞/위에서 시작 (시각적으로 보기 좋게)
    //     Vector3 localStart = new Vector3(0f, 0.5f, 1f);
    //     Vector3 pos = firePos.TransformPoint(localStart);
    //     return (vel, pos);
    // }

    // 궤적 그리기 (실시간 프리뷰)
// 궤적 그리기 (실시간 프리뷰 - BezierCurve 사용)
    public void UpdateTrajectory(Transform firePos, float force, float angle)
    {
        if (!trajectoryLine) return;
        SetupTrajectoryLine();
        trajectoryLine.positionCount = trajectoryLinePoint;

        // 시작 위치 (firePos 기준 앞/위)
        Vector3 localStart = new Vector3(0f, 0.5f, 1f);
        Vector3 p0 = firePos.TransformPoint(localStart);

        // 제어점 p1 (조준 방향으로 힘 반영)
        Vector3 dir = Quaternion.AngleAxis(angle, firePos.right) * firePos.forward;
        Vector3 p1 = p0 + dir * force * 0.5f;   // 중간 제어점, 힘 세기 반영

        // 끝점 p2 (예시: 바닥 높이 -1.55f 에서 x,z는 조준 방향)
        Vector3 p2 = p0 + dir * force;
        p2.y = -1.55f; // 원하는 착탄 y값으로 고정

        // 베지어 곡선으로 trajectoryLine 그리기
        for (int i = 0; i < trajectoryLinePoint; i++)
        {
            float t = i / (float)(trajectoryLinePoint - 1);
            Vector2 bez = BezierCurve.Quadratic(p0, p1, p2, t);
            trajectoryLine.SetPosition(i, new Vector3(bez.x, bez.y, p0.z)); 
            // ↑ BezierCurve.Quadratic이 Vector2 반환한다면, z는 p0.z 그대로 두거나 필요에 맞게 계산
        }

        trajectoryLine.enabled = true;
    }
    
    // public void UpdateTrajectory(Transform firePos)
    // {
    //     if (!trajectoryLine) return;
    //     SetupTrajectoryLine();
    //     trajectoryLine.positionCount = trajectoryLinePoint;
    //
    //     var (vel, pos) = CalculateTrajectoryVector(firePos);
    //     float dt = 1f / 15f; // 샘플 타임
    //
    //     for (int i = 0; i < trajectoryLinePoint; i++)
    //     {
    //         trajectoryLine.SetPosition(i, pos);
    //         vel += Physics.gravity * dt;
    //         pos += vel * dt;
    //     }
    //     trajectoryLine.enabled = true;
    // }

    // 실제 던지기
    public void Throw(Transform firePos, float force, float angle)
    {
        var (vel, pos) = CalculateTrajectoryVector(firePos, force, angle);
        var rb = GetComponent<Rigidbody>();
        transform.position = pos;

        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = vel;
        }

        if (trajectoryLine) trajectoryLine.enabled = false;
        Explosion();
    }

    // public void Throw(Transform firePos)
    // {
    //     var (vel, pos) = CalculateTrajectoryVector(firePos);
    //     var rb = GetComponent<Rigidbody>();
    //     transform.position = pos;
    //
    //     if (rb)
    //     {
    //         rb.isKinematic = false;
    //         rb.useGravity = true;
    //         rb.linearVelocity = vel;         // ← velocity 사용
    //     }
    //
    //     if (trajectoryLine) trajectoryLine.enabled = false;
    //     Explosion();
    // }

    // 폭발 시퀀스 시작
    public void Explosion()
    {
        StartCoroutine(Explode());
    }

    protected abstract IEnumerator Explode();
}
