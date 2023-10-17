using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;


public class CameraManager : SerializedMonoBehaviour
{

    [SerializeField] Dictionary<string, CinemachineVirtualCamera> cameraDic = new Dictionary<string, CinemachineVirtualCamera>();

    [SerializeField] CinemachineVirtualCamera currentVirtualCamera;

    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
    }
    /*  */
    public void ChangeCamera(string key)
    {
        CinemachineVirtualCamera find = null;
        cameraDic.TryGetValue(key, out find);

        if (find == null)
        {
            Debug.LogError("해당 Key값의 카메라를 찾지 못했습니다.");
            return;
        }
        else
        {
            if (key == "idle" || key == "follow")
            {
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
            }
            else
                Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);


            currentVirtualCamera.Priority = 10;
            find.Priority = 11;

            currentVirtualCamera = find;
        }

    }

    public void SwapCameraSec(float sec, Transform target)
    {
        var lastTarget = currentVirtualCamera.m_Follow;

        currentVirtualCamera.m_Follow = target;

        this.TaskDelay(sec, () => currentVirtualCamera.m_Follow = lastTarget);
    }
}
