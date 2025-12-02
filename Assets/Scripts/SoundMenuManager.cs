using UnityEngine;
using UnityEngine.UI;

public class SoundMenuManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;    // Para efectos de sonido (SFX)
    public AudioSource musicSource; // Para música de fondo

    [Header("Audio Clips")]
    public AudioClip buttonClickClip; // Sonido de clic para botones

    // Llamar a este método desde un botón para reproducir el sonido de clic
    public void PlayButtonClick()
    {
        if (buttonClickClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickClip);
        }
    }

    // Llamar a este método desde un botón y pasarle el nuevo clip de música
    public void ChangeMusic(AudioClip newMusic)
    {
        if (musicSource != null && newMusic != null)
        {
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }
}
