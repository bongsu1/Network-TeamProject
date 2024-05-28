using Photon.Pun;
using UnityEngine;

public enum Parameter
{
    SetBool,
    SetFloat,
    SetInteger,
    SetTrigger
}

// 확장자로 만들어서 애니메이션을 실행 하려고 했는데 PhotonView를 가지고 있는 객체가 RPC함수를 가지고 있어야 실행이 됨
public static class AnimatorRPC
{
    private static Animator animator;

    /// <summary>
    /// Animator.Play("clipName") to RPC
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="clipName"></param>
    public static void Play(this PhotonView photonView, string clipName)
    {
        photonView.RPC("PlayAnimation", RpcTarget.All, clipName);
    }

    [PunRPC]
    private static void PlayAnimation(string clipName)
    {
        animator.Play(clipName);
    }

    /// <summary>
    /// object에는 tpye에 맞는 자료형을 집어 넣어 주세요, 기본이 널인 이유는 SetTrigger는 value를 사용하지 않기 때문
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    public static void SetParmeter(this PhotonView photonView, Parameter type, string parameterName, object value = null)
    {
        photonView.RPC("SetAnimationParameter", RpcTarget.All, type, parameterName, value);
    }

    [PunRPC]
    public static void SetAnimationParameter(Parameter type, string parameterName, object value)
    {
        switch (type)
        {
            case Parameter.SetBool:
                if (value is bool boolean)
                    animator.SetBool(parameterName, boolean);
                break;
            case Parameter.SetFloat:
                if (value is float single)
                    animator.SetFloat(parameterName, single);
                break;
            case Parameter.SetInteger:
                if (value is int integer)
                    animator.SetInteger(parameterName, integer);
                break;
            case Parameter.SetTrigger:
                animator.SetTrigger(parameterName);
                break;
        }
    }
}
