using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    // 시네머신브레인 세팅 Default Blend : Hard Out
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] CinemachineVirtualCamera highAngleCamera;
    [SerializeField] float wheelSensitivity; // 마우스 휠 감도

    private CinemachineComponentBase followCameraBody;
    private CinemachineComponentBase highAngleCameraBody;
    private CinemachineTransposer followCameraTransposer;
    private CinemachineTransposer highAngleCameraTransposer;
    private float scrollSign; // 휠 방향
    private float zoomDistance; // 줌되어 있는 정도
    private bool isChatting;

    private void Start()
    {
        followCameraBody = playerFollowCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        highAngleCameraBody = highAngleCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        // 시네머신버추얼카메라에서 CinemachineComponentBase중에 body찾기

        if (followCameraBody is CinemachineTransposer followCameraTransposer)
        {
            // body 타입이 Transposer일때
            this.followCameraTransposer = followCameraTransposer;
            zoomDistance = this.followCameraTransposer.m_FollowOffset.y;
        }
        if (highAngleCameraBody is CinemachineTransposer highAngleCameraTransposer)
        {
            this.highAngleCameraTransposer = highAngleCameraTransposer;
        }
    }

    private void SetCameraOffset(float delta)
    {
        // 팔로우카메라 오프셋 조정
        followCameraTransposer.m_FollowOffset.y = delta;
        followCameraTransposer.m_FollowOffset.z = -delta;

        // 하이앵글 카메라 높이 조정
        highAngleCameraTransposer.m_FollowOffset.y = delta + 3f; // 팔로우카메라와 거리가 약간 다르기때문에 오차 조정
    }

    // InputSystem - Value : Axis
    private void OnScroll(InputValue value)
    {
        if (isChatting)
            return;

        scrollSign = Mathf.Clamp(value.Get<float>(), -1f, 1f); // 휠이 많이 돌아가도 일정 속도로 줌하기 위해 방향만 구함

        zoomDistance -= scrollSign * wheelSensitivity;
        zoomDistance = Mathf.Clamp(zoomDistance, 5f, 10f); // 줌 가능범위 설정

        SetCameraOffset(zoomDistance);
    }

    // InputSystem - Pass Through
    private void OnChangeAngle(InputValue value)
    {
        if (value.isPressed && !isChatting)
        {
            highAngleCamera.Priority = 15;
        }
        else if (!value.isPressed)
        {
            highAngleCamera.Priority = 5;
        }
    }

    public void SetIsChatting(bool isChatting)
    {
        this.isChatting = isChatting;
    }
}
