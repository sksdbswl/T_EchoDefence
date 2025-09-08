using UnityEngine;
using Unity.Cinemachine;

public enum CameraState
{
    Prev,
    Fight,
    Boss,
    Clear
}

public class CameraController : MonoBehaviour
{
    [Header("FreeLook Cameras")]
    public CinemachineFreeLook prevCam;
    public CinemachineFreeLook fightCam;
    public CinemachineFreeLook bossCam;
    public CinemachineFreeLook clearCam; // 필요하면 추가

    private CameraState currentState;

    private void Start()
    {
        SetCameraState(CameraState.Prev);
    }

    public void SetCameraState(CameraState state)
    {
        currentState = state;

        // 모든 카메라 Priority 초기화
        prevCam.Priority = 1;
        fightCam.Priority = 1;
        bossCam.Priority = 1;
        if (clearCam != null)
            clearCam.Priority = 1;

        // 현재 상태 카메라 Priority 상승
        switch (state)
        {
            case CameraState.Prev:
                prevCam.Priority = 10;
                break;
            case CameraState.Fight:
                fightCam.Priority = 10;
                break;
            case CameraState.Boss:
                bossCam.Priority = 10;
                break;
            case CameraState.Clear:
                if (clearCam != null)
                    clearCam.Priority = 10;
                break;
        }
    }

    public CameraState GetCurrentState()
    {
        return currentState;
    }
}