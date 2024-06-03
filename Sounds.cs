using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sounds : MonoBehaviour
{


    public AudioMixerGroup defaultGroup;
    

    public static Music music;

    private static Sounds instance;

    private List<AudioSource> sourcesPool = new List<AudioSource>();



    private void Awake()
    {
        instance = this;
        music = GetComponent<Music>();
    }

    private static AudioSource AllocateSource()
    {
        foreach (AudioSource src in instance.sourcesPool)
        {
            if (!src.isPlaying)
                return src;
        }
        
        instance.sourcesPool.Add(instance.gameObject.AddComponent<AudioSource>());
        return instance.sourcesPool[instance.sourcesPool.Count - 1];
    
    }

    public static void PlayPlayerSound(AudioClip clip)
    {

        AudioSource src = AllocateSource();
        src.outputAudioMixerGroup = instance.defaultGroup;
        src.clip = clip;
        src.Play();

    }
}
