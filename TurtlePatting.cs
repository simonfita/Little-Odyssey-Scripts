using System.Collections;
using UnityEngine;

public class TurtlePatting : MonoBehaviour, IDudResponder
{
    public ParticleSystem particles;
    public Animator anim;


    public float boostAmount = 0.5f;
    public float boostDuration = 30f;

    public float maxPattingOffset;
    public Texture2D hoverCursor, pattingCursor;

    [Header("Quality Requirements")]
    public float minTime;
    public float maxTime;
    public float minSpeed, maxSpeed;

    private float cooldown = 0;
    private bool patted = false;
    private bool beingPatted = false;

    public AudioSource pattingSrc, heartSrc;

    public void OnDuded()
    {
        if (Refs.gamepadController.usingGamepad)
            return;
        StartCoroutine(Patting(false));
    }
    public void OnDudHovered()
    {
        if (Refs.gamepadController.usingGamepad)
            return;

        if (!beingPatted && !patted)
            Cursor.SetCursor(hoverCursor, Vector2.one/2, CursorMode.Auto);
    }
    public void OnDudUnhovered()
    {
        if (Refs.gamepadController.usingGamepad)
            return;

        if (!beingPatted)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator Patting(bool usingGamepad)
    {
        if (patted || beingPatted)
            yield break;
        
        beingPatted = true;
        pattingSrc.Play();


        Cursor.SetCursor(pattingCursor, Vector2.one / 2, CursorMode.Auto);

        if (usingGamepad)
        {
            Refs.gamepadController.RequestPointingMode("Patting");
            Vector2 screenPos = Refs.playerCamera.cameraComponent.WorldToScreenPoint(transform.position);
            Refs.gamepadController.PointerWarp(screenPos);
            Refs.gamepadController.controllerSensitivity /= 2;
            yield return null;
        }

        Vector2 startingPos = Refs.controls.Other.MousePosition.ReadValue<Vector2>();
        float distance = 0;
        float time = 0;
        Vector2 lastPos = startingPos;

        while (usingGamepad ? Refs.controls.Other.GamepadPatting.IsPressed() : Refs.controls.Other.Dud.IsPressed())
        {
            Vector2 currentPos = Refs.controls.Other.MousePosition.ReadValue<Vector2>();
            distance += (lastPos-currentPos).magnitude;
            lastPos = currentPos;
            time += Time.deltaTime;
            if ((startingPos - currentPos).magnitude > maxPattingOffset)
            {
                    break;
            }
            yield return new WaitForEndOfFrame();
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (usingGamepad)
        {
            Refs.gamepadController.UnrequestPointingMode("Patting");
            Refs.gamepadController.controllerSensitivity *= 2;
        }

        ShowLove(GetPatQuality(distance, time));

        beingPatted = false;
        pattingSrc.Stop(); ;

        yield break;
    }

    private int GetPatQuality(float distance, float time)
    {
        int q = 1;
        if (time >= minTime && time <= maxTime)
            q++;
        float speed = distance / time;
        if (speed >= minSpeed && speed <= maxSpeed)
            q++;

        //Debug.Log("Time: " + time + " Petting speed: " + distance / time+" Quality: "+q);

        return q;
    }

    private void Update()
    {
        if (patted)
        {
            cooldown = Mathf.Max(cooldown - Time.deltaTime, 0);
            if (cooldown == 0)
            {
                patted = false;
                anim.SetFloat("TailWagging", 0);
            }
        }

        if (Refs.controls.Other.GamepadPatting.WasPressedThisFrame())
        {
            StartCoroutine(Patting(true));
        }
    }

    private void ShowLove(int quality)
    {
        patted = true;

        particles.emission.SetBurst(0, new ParticleSystem.Burst(0, quality));
        particles.Play();
        heartSrc.Play();
        
        StartCoroutine(Refs.turtle.Boost(boostAmount, boostDuration * quality));
        cooldown = boostDuration * quality;
        anim.SetFloat("TailWagging", quality * 0.3f);

        Refs.ui.inputHints.CompletedAction("Dud");
    }

}
