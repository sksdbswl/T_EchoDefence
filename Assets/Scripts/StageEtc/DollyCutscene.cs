using UnityEngine;
using UnityEngine.Splines;

public class DollyCutscene : MonoBehaviour
{
    [SerializeField] private SplineAnimate splineAnimate;

    private float elapsed = 0f;
    private float duration = 3f;

    void Update()
    {
        if (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 0 ~ 1 구간에서 카메라 위치/회전 강제로 제어
            splineAnimate.NormalizedTime = t;
        }
    }
    
    // public void PlayCutscene()
    // {
    //     splineAnimate.Restart(true);   // 처음부터 경로 따라가기
    // }
    //
    // public void StopCutscene()
    // {
    //     splineAnimate.Pause();         // 일시정지
    // }
}