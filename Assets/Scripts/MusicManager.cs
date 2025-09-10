using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
