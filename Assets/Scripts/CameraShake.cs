using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originalPos;
    private float shakeTime;
    private float shakeStrength;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            transform.localPosition =
                originalPos + (Random.insideUnitSphere * shakeStrength);
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void ShakeCamera(float duration, float strength)
    {
        shakeTime = duration;
        shakeStrength = strength;
    }
}
