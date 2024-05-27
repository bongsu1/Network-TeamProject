using UnityEngine;

public class InGameUI : BaseUI
{
    public Transform followTarget;
    public Vector3 followOffset;

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(followTarget.position) + followOffset;
        }
        return;
    }
    public void SetTarget(Transform target)
    {
        followTarget = target;
    }
    public void SetOffset(Vector3 offset)
    {
        followOffset = offset;
    }
    public void Close()
    {
        Manager.UI.CloseInGameUI();
    }
}
