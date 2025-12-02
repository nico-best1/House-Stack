using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSourcePrefab;

    [Header("Volume Settings")]
    [Range(0, 1)] public float musicVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🎵 Música
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // 💥 Sonido puntual
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSourcePrefab == null) return;

        AudioSource tempSource = Instantiate(sfxSourcePrefab, transform.position, Quaternion.identity);
        tempSource.clip = clip;
        tempSource.volume = Mathf.Clamp01(volume); // Asegura que esté entre 0 y 1
        tempSource.Play();
        Destroy(tempSource.gameObject, clip.length);
    }


    // 🔀 Sonido aleatorio
    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        PlaySFX(clips[index], volume);
    }

}
