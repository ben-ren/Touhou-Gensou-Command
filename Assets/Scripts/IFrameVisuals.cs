using UnityEngine;
using System.Collections.Generic;

public class IFrameVisuals : MonoBehaviour
{
    [Header("I-Frame Settings")]
    [SerializeField] private Transform visualsRoot; // assign "Visuals" transform
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private bool useEmission = true; // toggle for emission flashing

    private List<Renderer> renderers = new();
    private List<Material[]> originalMaterials = new();
    private List<Color[]> originalEmissionColors = new();

    void Awake()
    {
        if (!visualsRoot) visualsRoot = transform.Find("Visuals");

        if (visualsRoot == null)
        {
            Debug.LogWarning("IFrameVisuals: No 'Visuals' root found!");
            return;
        }

        Renderer[] allRenderers = visualsRoot.GetComponentsInChildren<Renderer>(true);
        foreach (var r in allRenderers)
        {
            renderers.Add(r);
            originalMaterials.Add(r.materials);

            // Store original emission colors
            Color[] emissionColors = new Color[r.materials.Length];
            for (int i = 0; i < r.materials.Length; i++)
            {
                if (r.materials[i].HasProperty("_EmissionColor"))
                    emissionColors[i] = r.materials[i].GetColor("_EmissionColor");
                else
                    emissionColors[i] = Color.black;
            }
            originalEmissionColors.Add(emissionColors);
        }
    }

    public void StartIFrameVisual(float duration)
    {
        StopAllCoroutines();
        if(this.gameObject != null)
        {
            StartCoroutine(FlashRoutine(duration));
        }
    }

    private System.Collections.IEnumerator FlashRoutine(float duration)
    {
        float timer = 0f;
        bool toggle = false;

        while (timer < duration)
        {
            timer += flashInterval;
            toggle = !toggle;

            for (int i = 0; i < renderers.Count; i++)
            {
                var mats = renderers[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    // Flash base color
                    mats[j].color = toggle ? flashColor : originalMaterials[i][j].color;

                    // Flash emission if enabled and material supports it
                    if (useEmission && mats[j].HasProperty("_EmissionColor"))
                    {
                        mats[j].EnableKeyword("_EMISSION");
                        mats[j].SetColor("_EmissionColor", toggle ? flashColor : originalEmissionColors[i][j]);
                    }
                }
                renderers[i].materials = mats;
            }

            yield return new WaitForSeconds(flashInterval);
        }

        // Restore original colors & emission
        for (int i = 0; i < renderers.Count; i++)
        {
            var mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j].color = originalMaterials[i][j].color;
                if (useEmission && mats[j].HasProperty("_EmissionColor"))
                    mats[j].SetColor("_EmissionColor", originalEmissionColors[i][j]);
            }
            renderers[i].materials = mats;
        }
    }
}
