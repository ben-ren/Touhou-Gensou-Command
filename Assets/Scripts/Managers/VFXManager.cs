using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeCamera(duration, magnitude);
    }
}
