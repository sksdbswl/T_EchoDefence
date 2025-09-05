using UnityEngine;

public static class BezierCurve
{
    /// <summary>
    /// 3차원 2차 베지어 곡선(Quadratic Bézier Curve)
    /// </summary>
    /// <param name="p0">시작점</param>
    /// <param name="p1">제어점</param>
    /// <param name="p2">끝점</param>
    /// <param name="t">시간 (0 ~ 1)</param>
    /// <returns>곡선 위의 위치</returns>
    public static Vector3 Quadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // 베지어 곡선 공식: B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2(1-t)t * P1
        p += tt * p2;        // t^2 * P2

        return p;
    }
}