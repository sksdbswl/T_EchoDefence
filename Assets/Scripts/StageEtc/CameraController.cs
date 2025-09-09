using System.Collections;
using TMPro;
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
    public CinemachineCamera prevCam;
    public CinemachineCamera fightCam;
    public CinemachineCamera bossCam;
    public CinemachineCamera clearCam; // 필요하면 추가
    private CameraState currentState;

    [Header("Count UI Settings")] 
    [SerializeField] private TMP_Text[] CountText;

    public void SetCameraState(CameraState state)
    {
        currentState = state;
        
        prevCam.Priority = 1;
        fightCam.Priority = 1;
        bossCam.Priority = 1;
        clearCam.Priority = 1;
        
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

    public IEnumerator BossIntroSequence()
    {
        Debug.Log("Boss Intro Sequence");
        
        GameManager.Instance.CameraController.SetCameraState(CameraState.Boss);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.CameraController.SetCameraState(CameraState.Prev);
        
        for (int i = 3; i >= 0; i--)
        {
            CountText[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            CountText[i].gameObject.SetActive(false);
        }

        StageManager.Instance.StageActive = true;
    }
    
    public IEnumerator IntroSequence()
    {
        GameManager.Instance.CameraController.SetCameraState(CameraState.Boss);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.CameraController.SetCameraState(CameraState.Prev);
        
        for (int i = 3; i > 0; i--)
        {
            CountText[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            CountText[i].gameObject.SetActive(false);
        }

        StageManager.Instance.StageActive = true;
    }
}