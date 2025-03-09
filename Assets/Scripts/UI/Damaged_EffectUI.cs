using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Damaged_EffectUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private bool isDamaged = false;

    private void Start()
    {
        if (health == null || postProcessVolume == null)
        {
            return;
        }

        postProcessVolume.profile.TryGetSettings(out vignette);

        health.OnTakeDamage += Health_OnTakeDamage;
        health.OnDie += Health_OnDie;
    }

    private void Health_OnTakeDamage()
    {
        if (vignette != null && !isDamaged)
        {
            StartCoroutine(ApplyVignetteEffect());
        }
    }

    private void Health_OnDie()
    {
        health.OnTakeDamage -= Health_OnTakeDamage;
        health.OnDie -= Health_OnDie;
    }

    private IEnumerator ApplyVignetteEffect()
    {
        isDamaged = true;

        vignette.intensity.value = 0.5f;

        float duration = 0.6f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            vignette.intensity.value = Mathf.Lerp(0.5f, 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = 0f;
        isDamaged = false;
    }
}
