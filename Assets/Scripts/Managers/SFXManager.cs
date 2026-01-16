using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    //Generates class as a Singleton, allowing fucntions within to be called from anywhere in the
    public static SFXManager instance;
    [SerializeField] private AudioSource audioSourcePrefab;
    [SerializeField] private AudioMixerGroup SFXMixerGroup;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // prevent duplicates
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFXClip(AudioClip clip, Transform spawnPoint, float volume)
    {
        //spawn gameObject
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnPoint.position, Quaternion.identity);
        //assign audioClip
        audioSource.clip = clip;
        //assign MasterMixer SFXVolume group
        audioSource.outputAudioMixerGroup = SFXMixerGroup;
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get clip length
        float clipLength = audioSource.clip.length;
        //destroy clip after completion
        Destroy(audioSource.gameObject, clipLength);
    }
}
