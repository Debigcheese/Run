using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVC;
    public float shakeIntensity = 1f;
    public float frequencyGain = 1f;
    public float shakeTime = 0.2f;

    private float timer = 0f;
    private CinemachineBasicMultiChannelPerlin cbmcp;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVC = GetComponent<CinemachineVirtualCamera>();
        cbmcp = cinemachineVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StopShake();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                StopShake();
            }
        }
    }

    public void ShakeCameraFlex(float newShakeIntensity, float newShakeTime)
    {
        cbmcp.m_AmplitudeGain = newShakeIntensity;
        cbmcp.m_FrequencyGain = frequencyGain;

        timer = newShakeTime;
    }

    public void ShakeCamera()
    {
        cbmcp.m_AmplitudeGain = shakeIntensity;
        cbmcp.m_FrequencyGain = frequencyGain;

        timer = shakeTime;
    }

    public void StopShake()
    {
        cbmcp.m_AmplitudeGain = 0;
        cbmcp.m_FrequencyGain = 0;

        timer = 0;
    }
}
