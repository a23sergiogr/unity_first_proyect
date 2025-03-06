using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } 

    public List<AudioClip> backgroundMusicList; 
    public bool shufflePlaylist = false;
    public float volume = 1.0f; 

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.loop = false;

        PlayNextTrack();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        Debug.Log("Musica sonando");
        if (backgroundMusicList == null || backgroundMusicList.Count == 0)
        {
            Debug.LogWarning("No hay canciones en la lista.");
            return;
        }

        if (shufflePlaylist)
        {
            currentTrackIndex = Random.Range(0, backgroundMusicList.Count); 
        }
        else
        {
            currentTrackIndex = (currentTrackIndex + 1) % backgroundMusicList.Count; 
        }

        audioSource.clip = backgroundMusicList[currentTrackIndex];
        audioSource.Play();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp(newVolume, 0.0f, 1.0f);
        audioSource.volume = volume;
    }

    public void TogglePause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    public void SkipTrack()
    {
        PlayNextTrack();
    }
}