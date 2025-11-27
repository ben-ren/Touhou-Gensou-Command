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

    public void GenerateParticleVFX(ParticleSystem particleSystemPrefab, Transform spawnPoint, float lifespan)
    {
        // Instantiate a new particle system at the spawn point
        ParticleSystem particles = Instantiate(particleSystemPrefab, spawnPoint.position, Quaternion.identity);
        
        // Optional: set rotation to match spawnPoint
        particles.transform.rotation = spawnPoint.rotation;

        // Destroy after lifespan
        Destroy(particles.gameObject, lifespan);

        // Play the particles immediately
        particles.Play();
    }
}
