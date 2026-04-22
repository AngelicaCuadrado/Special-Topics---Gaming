using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Música de Fondo (BGM)")]
    public AudioSource musicSource;
    public AudioSource ambienceSource;

    [Header("Efectos de Sonido (SFX)")]
    public AudioSource sfxSource;
    public AudioClip plantingSound;
    public AudioClip wormEatingSound;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}