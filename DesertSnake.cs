using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSnake : MonoBehaviour, I_Interactable
{
    public AudioSource src;
    public Animator anim;
    public ParticleSystem particles;

    public List<AudioClip> sounds;
    public float aboveTime, resetTime,moveSpeed;
    
    public bool aboveGround;
    //public bool canShow;


    private int currentLevel;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        
        src.clip = sounds[0];
        src.Play();
        aboveGround = true;
        anim.SetTrigger("AboveGround");

        Invoke(nameof(Hide),aboveTime);
    }

    private void Sing()
    {
        if (!aboveGround)
            return;
        currentLevel++;
        src.clip = sounds[currentLevel];
        src.Play();
        particles.Play();
        anim.SetTrigger("NextStage");


        if (currentLevel == sounds.Count - 1)
        {
            Refs.turtle.StartCoroutine(Refs.turtle.Boost(1.5f, currentLevel * 5f));
            var main = Refs.turtle.musicParticles.main;
            main.startLifetime = currentLevel * 5f;
            Refs.turtle.musicParticles.Play();
            Invoke(nameof(Hide), 1f);     
        }

    }

    private void Hide()
    {
        if (!aboveGround)
            return;
        
        anim.SetTrigger("Hide");

        currentLevel = 0;
        aboveGround = false;
    }




    public bool CanBeInteracted()
    {
        return aboveGround && currentLevel < sounds.Count-1;
    }

    public string GetInteractionText()
    {
        return "Sing";
    }

    public Transform GetInteractionTransform()
    {
        return transform;
    }

    public InteractionMountingRequirement GetMountingRequirement()
    {
        return InteractionMountingRequirement.OnlyMounted;
    }

    public void Interact()
    {
        Sing();
    }
}
