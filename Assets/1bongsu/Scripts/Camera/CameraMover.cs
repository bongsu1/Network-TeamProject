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

    private void Start()
    {
        followCameraBody = playerFollowCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        highAngleCameraBody = highAngleCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        // 시네머신버추얼카메라에서 CinemachineComponentBase중에 body찾기

        if (followCameraBody is CinemachineTransposer followCameraTransposer)
        {
            // body 타입이 Transposer일때
            this.followCameraTransposer = followCameraTransposer;
        }
        if (highAngleCameraBody is CinemachineTransposer highAngleCameraTransposer)
        {
            this.highAngleCameraTransposer = highAngleCameraTransposer;
        }
    }

    // InputSystem - Value : Axis
    private void OnScroll(InputValue value)
    {
        scrollSign = Mathf.Clamp(value.Get<float>(), -1f, 1f); // 휠이 많이 돌아가도 일정 속도로 줌하기 위해 방향만 구함

        // 팔로우카메라 오프셋 조정
        followCameraTransposer.m_FollowOffset.y -= scrollSign * wheelSensitivity;
        followCameraTransposer.m_FollowOffset.z += scrollSign * wheelSensitivity;

        // 하이앵글 카메라 높이 조정
        highAngleCameraTransposer.m_FollowOffset.y -= scrollSign * wheelSensitivity;
    }

    // InputSystem - Pass Through
    private void OnChangeAngle(InputValue value)
    {
        if (value.isPressed)
        {
            highAngleCamera.Priority = 15;
        }
        else
        {
            highAngleCamera.Priority = 5;
        }
    }
}
