using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPreview : MonoBehaviour
{
    [Header("Sampling")]
    [SerializeField] float timeStep = 0.05f;     // 샘플 간격(작을수록 부드러움)
    [SerializeField] float maxTime  = 3.0f;      // 최대 예측 시간
    [SerializeField] int   maxBounces = 1;       // 최대 바운스 수(수류탄이면 1~2)
    
    [Header("Physics")]
    [SerializeField] float grenadeRadius = 0.15f; // 수류탄 반지름(콜라이더 기준)
    [SerializeField] float restitution   = 0.5f;  // 바운스 탄성(0~1)
    [SerializeField] LayerMask hitMask   = ~0;    // 충돌할 레이어

    LineRenderer lr;
    const float SKIN = 0.01f;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 0;
        transform.rotation = Quaternion.identity;
    }

    public void Hide() => lr.positionCount = 0;

    /// <summary> 시작 위치/초기속도로 궤적을 계산하여 라인으로 그림 </summary>
    public void Draw(Vector3 startPos, Vector3 initialVelocity)
    {
        var points = SamplePath(startPos, initialVelocity);
        lr.positionCount = points.Count;
        if (points.Count > 0) lr.SetPositions(points.ToArray());
    }

    List<Vector3> SamplePath(Vector3 startPos, Vector3 vel0)
    {
        var pts = new List<Vector3>(128);
        Vector3 g = Physics.gravity;

        Vector3 pos = startPos;
        Vector3 vel = vel0;
        float t = 0f;
        int bounces = 0;

        pts.Add(pos);

        while (t < maxTime)
        {
            // 다음 위치 예측(등가속도)
            Vector3 nextPos = pos + vel * timeStep + 0.5f * g * (timeStep * timeStep);
            Vector3 delta = nextPos - pos;
            float dist = delta.magnitude;

            // 이동 구간만큼 스피어캐스트하여 충돌 검사
            if (Physics.SphereCast(pos, grenadeRadius, delta.normalized, out RaycastHit hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {
                // 충돌 지점까지 추가
                pts.Add(hit.point);
                
                if (bounces >= maxBounces) break;

                // 반사 속도 계산(탄성 적용)
                Vector3 vNext = vel + g * timeStep;
                Vector3 reflected = Vector3.Reflect(vNext, hit.normal) * Mathf.Clamp01(restitution);

                // 다음 스텝을 위해 살짝 밀어냄(SKIN)
                pos = hit.point + hit.normal * (grenadeRadius + SKIN);
                vel = reflected;
                bounces++;
            }
            else
            {
                // 충돌 없으면 다음 포인트 추가
                pts.Add(nextPos);
                pos = nextPos;
                vel += g * timeStep;
            }

            t += timeStep;
            if (pts.Count > 400) break; // 안전장치
        }

        return pts;
    }
}
