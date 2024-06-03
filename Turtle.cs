using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using SF;

public class Turtle : SaveableBehaviour
{
    public TokenSlot slot;
    public CharacterController controller;
    public Animator anim;
    public Hydration hydration;
    public TurtleStockpile leftStockpile, rightStockpile;
    public TurtleVisualUpgrades visualUpgrades;
    public Transform heavyCargoPoint;
    public ParticleSystem musicParticles;
    public GameObject teleportationParticles;
    public AudioClip teleportationSound,jumpSound;

    public float mountSpeed;
    public float mountTurnRate;
    public float mountBoostAmount;
    public int mountBoostTimes;
    public float moveTreshold;
    public float groundSpeedMultiplier;

    public float maxMountDistance;

    public float terrainLerpingSpeed;
    
    public bool isMounted {get; private set;}
    public bool isMoving { get { return isMovingForward || isMovingBackward || isRotating; } }
    
    [HideInInspector]
    public bool isMovingForward, isMovingBackward, isRotating, isRotatingRight;
    private bool isAutomoving;

    public Action OnMounted;
    public Action OnUnmounted;

    private RaycastHit cashedHit;
    private Vector3 groundNormal = Vector3.up;
    

    public HeavyCargo currentHeavyCargo;

    private void Start()
    {
        StartCoroutine(AnimateIdle());
    }

    private void OnEnable()
    {
        //enable entering/exiting
        slot.enabled = true;
    }
    private void OnDisable()
    {
        //disable entering/exiting
        slot.enabled = false;
        anim.SetBool("Walking", false);
    }

   

    private void Update()
    {
        controller.Move(Vector3.down*10 * Time.deltaTime); //gravity

        Physics.Raycast(transform.position, Vector3.down, out cashedHit, 50f, LayerMask.GetMask("Default", "Water"));

        UpdateGroundMultiplier();
        MountInput();
        MovementInputs();
        UpdateModel();
    }

    #region Mounting

    private void MountInput()
    {
        if (!Refs.controls.Other.Mounting.WasPerformedThisFrame())
            return;

        if (!isMounted)
        {
            if (Refs.player.token.canMove && GetIsPlayerInMountingRange())
                Mount();
        }
        else
        {
            Unmount();
        }

    }
    public void Mount()
    {
        slot.EnterSlot(Refs.player.token);
        Sounds.PlayPlayerSound(jumpSound);
        isMounted = true;
        anim.SetBool("Sitting", false);

        Refs.playerCamera.ChangeCameraMode(CameraModes.Turtle, false);

        Refs.ui.inputHints.CompletedAction("Mounting");
        OnMounted?.Invoke();
    }
    public void Unmount()
    {
        slot.ExitSlot();
        Sounds.PlayPlayerSound(jumpSound);

        isMounted = false;
        anim.SetBool("Sitting", true);
        Refs.playerCamera.ChangeCameraMode(CameraModes.Player, false);
        Stop();

        OnUnmounted?.Invoke();
    }
    public bool GetIsPlayerInMountingRange()
    {
        if (Refs.player.token.heldGoods != null)
            return false; // cannot mount while carrying
        
        return Vector3.Distance(Refs.player.transform.position, transform.position) <= maxMountDistance;
    }

    #endregion

    public float CalculateCurrentSpeed(bool backward)
    {
        float returns = mountSpeed + mountBoostAmount;
        
        returns *= groundSpeedMultiplier;

        if (hydration.currentHydration == 0 || Sandstorm.playerInSandstorm)
            returns *= 0.7f;

        if (backward)
            returns *= 0.6f;
        return returns;
    }

    private void UpdateGroundMultiplier()
    {
        if (cashedHit.transform.CompareTag("Road") || Refs.player.currentSettlement != null)
        { 
            groundSpeedMultiplier = 1; 
        }
        else
        {
            groundSpeedMultiplier = Math.Clamp(groundSpeedMultiplier - Time.deltaTime * 0.1f, 0.75f, 1);
        }
    }

    private void MovementInputs()
    {
        if (!isMounted)
            return;
        #region Automove

        bool canAutomove = !isAutomoving && isMovingForward;
        Refs.ui.hud.SetShowAutomovePrompt(canAutomove);

        if (isAutomoving && Refs.controls.Movement.Automove.WasPerformedThisFrame() || isMovingBackward)
        {

            isAutomoving = false;
        }
        else if (canAutomove && Refs.controls.Movement.Automove.WasPerformedThisFrame())
        {
            isAutomoving = true;
        }



        #endregion

        Vector3 _movementInput = Refs.controls.Movement.Move.ReadValue<Vector2>().XZ();


        

        isMovingForward = isAutomoving || _movementInput.z < -moveTreshold;

        isMovingBackward = _movementInput.z > moveTreshold;

        isRotating = _movementInput.x != 0;

        isRotatingRight = isRotating && _movementInput.x > 0;


        if (isRotating)
            transform.Rotate(0, -_movementInput.x * Time.deltaTime * mountTurnRate, 0);

        Vector3 movement = Vector3.zero;
        
        if (isMovingForward)
            movement =transform.forward * CalculateCurrentSpeed(false) * Time.deltaTime;

        if (isMovingBackward)
            movement = -transform.forward * CalculateCurrentSpeed(true) * Time.deltaTime;

        if (CanStepOn(movement))
            controller.Move(movement);

    }

    public void Teleport(Vector3 position)
    {
        Teleport(position, transform.rotation);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
    }

    private void Stop()
    {
        isMovingForward = false;
        isMovingBackward = false;
        isRotating = false;

        isAutomoving = false;
        
        Refs.ui.hud.SetShowAutomovePrompt(false);
        //anim.SetFloat("WalkBlend", 0);

    }

    private bool CanStepOn(Vector3 direction)
    {

        Vector3 start = transform.position + direction +Vector3.up*5;//prevents casting from inside collider


        if (!Physics.Raycast(start, Vector3.down, out RaycastHit hit, 30f, LayerMask.GetMask("Default", "Water")))
        {
            Debug.LogWarning("No ground in front");
            return false; 
        }
        if (hit.collider.gameObject.layer != 4)//water
        {
            return true;
        }
        else
        {
            Debug.LogWarning("Can't step on " + hit.collider.gameObject);
            return false;
        }
    }

    public IEnumerator Boost(float amount, float time)
    {
        mountBoostAmount += amount;
        mountBoostTimes++;
        yield return new WaitForSeconds(time);
        mountBoostAmount -= amount;
        mountBoostTimes--;
        yield break;
    }

    private void UpdateModel()
    {
        if (isMovingForward || isMovingBackward)
        {
            anim.SetBool("Walking", true);
            if (isMovingForward)
            {
                anim.SetFloat("WalkingSpeed", CalculateCurrentSpeed(false));
            }
            else
            {
                anim.SetFloat("WalkingSpeed", CalculateCurrentSpeed(true) * -1);
            }
        }
        else
        {
            anim.SetBool("Walking", false);
        }
        if (isRotating)
        {
            anim.SetBool("Rotating", true);
            if (isRotatingRight)
                anim.SetFloat("RotationDir", 1f);
            else
                anim.SetFloat("RotationDir", -1f);
        }
        else
        {
            anim.SetBool("Rotating", false);
        }

        groundNormal = cashedHit.normal;
        Quaternion zToUp = Quaternion.LookRotation(groundNormal, -transform.forward);
        Quaternion yToz = Quaternion.Euler(90, 0, 0);
        anim.transform.rotation = Quaternion.RotateTowards(anim.transform.rotation, zToUp * yToz, Time.deltaTime * terrainLerpingSpeed);

        controller.radius = 2.5f + groundSpeedMultiplier;
        
    }

    private IEnumerator AnimateIdle()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 60f));
            if (isMovingForward)
            {
                anim.SetInteger("IdleIndex", UnityEngine.Random.Range(1, 5));
                anim.SetTrigger("Idle");
            }
        }
    
    }

    #region Heavy Cargo

    public bool TryLoadHeavyCargo(HeavyCargo heavyCargo)
    {
        if (currentHeavyCargo != null)
            return false;
        if (leftStockpile.leftPlaces < 2 || rightStockpile.leftPlaces < 2)         
            return false;


        currentHeavyCargo = heavyCargo;
        currentHeavyCargo.transform.SetParent(heavyCargoPoint.transform);
        currentHeavyCargo.transform.localPosition = new Vector3();
        currentHeavyCargo.transform.localRotation = Quaternion.identity;

        leftStockpile.FreeMiddle();
        rightStockpile.FreeMiddle();
        return true;
    }

    public HeavyCargo UnloadHeavyCargo()
    {
        leftStockpile.ReclaimMiddle();
        rightStockpile.ReclaimMiddle();

        HeavyCargo temp = currentHeavyCargo;
        currentHeavyCargo = null;
        return temp;
    }

    #endregion

    public IEnumerator TeleportWithAnimation(Vector3 to)
    {
        Refs.turtle.Mount();
        float minScale = 15;
        float maxScale = 23;
        float time = 2;

        teleportationParticles.SetActive(true);
        Sounds.PlayPlayerSound(teleportationSound);

        for (float t = 0; t < time; t+=Time.deltaTime)
        {
            teleportationParticles.transform.localScale = Vector3.one* Mathf.Lerp(maxScale, minScale, t/ time);
            yield return null;
        }
        Teleport(to);
        Refs.playerCamera.TeleportToDesired();

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            teleportationParticles.transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, t/ time);
            yield return null;
        }
        teleportationParticles.SetActive(false);
        yield break;
    }

    #region Pack Upgrades
    public void UnlockLeftPackUpgrade()
    {
        visualUpgrades.ShowUpgrade(TurtleVisualUpgrades.TurtleVisualUpgrade.LeftPack);
        leftStockpile.Size = 4;
    }
    public void UnlockRightPackUpgrade()
    {
        visualUpgrades.ShowUpgrade(TurtleVisualUpgrades.TurtleVisualUpgrade.RightPack);
        rightStockpile.Size = 4;
    }

    #endregion


    public override void OnSave(Save save)
    {
        save.turtlePos = transform.position;
        save.turtleRot = transform.rotation;
        save.isMounted = isMounted;
    }

    public override void OnLoad(Save save)
    {
        Teleport(save.turtlePos, save.turtleRot);
    }
}
