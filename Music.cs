using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public List<AudioClip> musicTracks;

    public List<AudioClip> firstRegionTracks, secondRegionTracks, thirdRegionTracks, nightTracks;

    public float minSongDelay, maxSongDelay;

    [SerializeField]
    private AudioSource src;

    private bool invoked;

    private void Update()
    {
        if (!src.isPlaying &&!invoked)
        {
            Invoke(nameof(PlayNextTrack),Random.Range(minSongDelay,maxSongDelay));
            invoked = true;
        }
    }


    private void PlayNextTrack()
    {
        invoked = false;
        src.clip = SelectNextTrack();

        src.Play();
    }

    private AudioClip SelectNextTrack()
    {
        AudioClip randomTrack;
        List<AudioClip> source = new List<AudioClip>();
        if (WorldTime.IsNight())
            source = nightTracks;
        else
        {
            switch (RegionBounds.GetPlayerRegion())
            {
                case WorldRegion.LongShore:
                    source = firstRegionTracks;
                    break;
                case WorldRegion.StarRock:
                    source = secondRegionTracks;
                    break;
                case WorldRegion.GrainPlains:
                    source = thirdRegionTracks;
                    break;
            }
        }

        do
        {
            randomTrack = source[Random.Range(0, source.Count)];
        } while (randomTrack == src.clip);


        return randomTrack;
    }
}
