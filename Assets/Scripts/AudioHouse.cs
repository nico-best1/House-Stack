using UnityEngine;

public class AudioHouse : MonoBehaviour
{
    [Header("Sonidos disponibles")]
    public AudioClip[] houseSounds;

    [Header("Volumen basado en velocidad")]
    public float minVolume = 0.2f;
    public float maxVolume = 1.0f;
    public float maxSpeed = 10f; // velocidad máxima para volumen al 100%

    private void OnCollisionEnter(Collision collision)
    {
        if (houseSounds.Length == 0) return;

        // Obtener la velocidad relativa del impacto
        float impactSpeed = collision.relativeVelocity.magnitude;

        // Escalar velocidad a volumen (clamp entre minVolume y maxVolume)
        float volume = Mathf.Clamp01(impactSpeed / maxSpeed);
        volume = Mathf.Lerp(minVolume, maxVolume, volume);

        // Llamar al AudioManager pasando el volumen
        AudioManager.Instance.PlayRandomSFX(houseSounds, volume);
    }
}
