using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SF;
public class Player : SaveableBehaviour
{
    public Token token;
    public CharacterController controller;
    public TokenRenderer tokenRenderer;

    public Controls controls;

    public string playerName;

    [Range(1, 15)]
    public float basePlayerSpeed;

    public float dashTime;
    public float dashSpeed;
    public Quaternion walkRot;
    public ParticleSystem dashParticles;
    public AudioSource dashAudio;
    public Item defaultHat;


    public LocationSettlement currentSettlement, lastSettlement;
    public WorldRegion currentRegion = WorldRegion.OutOfBounds;
    private Coroutine outOfBoundsRoutine;

    [HideInInspector]
    public NPC talkedToNPC;

    public bool isDashing;

    override protected void Awake()
    {
        base.Awake();
        controls = new Controls();
        controls.Enable();

    }

    private void Start()
    {
        Refs.inventory.AddItem(defaultHat);
    }

    private void Update()
    {
        if (token.canMove)
        {
            controller.Move(Vector3.down * 10*Time.deltaTime*60); // gravity
            Walking();
            if (controls.Movement.Automove.WasPerformedThisFrame())
                StartCoroutine(Dash());
        }
        CheckRegion();
    }

    public float GetCurrentSpeed()
    {
        float returns = basePlayerSpeed;
        RaycastHit info;
        if (Physics.Raycast(transform.position, Vector3.down, out info, 50f))
        {
            if (!info.transform.CompareTag("Road")&&currentSettlement ==null)
                returns *=0.7f;
        }

        if (isDashing)
            returns += dashSpeed;

        return returns;

    
    }


    private void Walking()
    {

        Vector3 _move = controls.Movement.Move.ReadValue<Vector2>().XZ();
        _move= walkRot * _move;

        _move.Normalize();
        _move *= GetCurrentSpeed();
        _move *= Time.deltaTime;

        if (_move.sqrMagnitude == 0)
            return;

        if (CanStepOn(_move))
        {
            controller.Move(_move);
            transform.forward = _move;
            Refs.ui.inputHints.CompletedAction("Move");
        }                   
    }

    private IEnumerator Dash()
    {
        if (isDashing)
            yield break;
        Refs.ui.inputHints.CompletedAction("Automove");
        dashAudio.Play();
        dashParticles.Play();
        isDashing = true;
        token.anim.speed = 0;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        token.anim.speed = 1;
        yield break;
    }

    public void Teleport(Vector3 to)
    {
        controller.enabled = false;
        transform.position = to;
        controller.enabled = true;
    }


    public void StartDialogue(NPC npc)
    {
        token.canMove = false;
        talkedToNPC = npc;
        tokenRenderer.enabled = false;
        Refs.playerCamera.ChangeCameraMode(CameraModes.Dialogue, true);
        if (Refs.turtle.isMounted)
            Refs.playerCamera.cameraComponent.cullingMask = ~LayerMask.GetMask("Turtle");
    }
    public void EndDialogue()
    {
        
        talkedToNPC = null;
        tokenRenderer.enabled = true;
        if (!Refs.turtle.isMounted)
        {
            token.canMove = true;
            Refs.playerCamera.ChangeCameraMode(CameraModes.Player, true);
        }
        else
        {
            Refs.playerCamera.ChangeCameraMode(CameraModes.Turtle, true);
            Refs.playerCamera.cameraComponent.cullingMask = ~LayerMask.GetMask();
        }
    }

    public void EnterSettlement(LocationSettlement settlement)
    {
        if (currentSettlement != null)
            return;

        currentSettlement = settlement;
        lastSettlement = settlement;

        Refs.ui.hud.locationDisplay.DisplayLocation(settlement.locationName);
    }

    public void ExitSettlement(LocationSettlement settlement)
    {
        if (currentSettlement != settlement)
            return;

        currentSettlement = null;
    }

    private bool CanStepOn(Vector3 direction)
    {

        Vector3 start = transform.position + direction +Vector3.up;



        RaycastHit hit;

        if (!Physics.Raycast(start, Vector3.down, out hit, 10f))
            return false;

        return hit.collider.gameObject.layer != 4; //water
        

    }

    public void ChangeHat(int hatID)
    {
        tokenRenderer.SetHat(hatID);
    }

    private void CheckRegion()
    {
        if (RegionBounds.GetPlayerRegion() != currentRegion)
        {
            currentRegion = RegionBounds.GetPlayerRegion();
            Debug.Log("Entered region: "+currentRegion);
            switch (currentRegion)
            {
                case WorldRegion.LongShore:
                    Refs.ui.hud.locationDisplay.DisplayLocation("LONGSHORE");
                    break;
                case WorldRegion.StarRock:
                    Refs.ui.hud.locationDisplay.DisplayLocation("STAR ROCK");
                    break;
                case WorldRegion.GrainPlains:
                    Refs.ui.hud.locationDisplay.DisplayLocation("GRAIN PLAINS");
                    break;
            }

            if (currentRegion == WorldRegion.OutOfBounds)
            {
                if (outOfBoundsRoutine == null)
                    outOfBoundsRoutine = StartCoroutine(OutOfBoundsRoutine());
            }
#if DEMO
            else if (currentRegion != WorldRegion.LongShore)
            {
                FindObjectOfType<SaveSystem>().SaveGame("Demo");
                SceneManager.LoadScene("DemoEnd");
            }
#endif           
        }
    }

    private IEnumerator OutOfBoundsRoutine()
    {
        float allowedTime = 10;

        Shader.SetGlobalInteger("_InSandstorm", 1);
        Refs.ui.hud.SpawnGameMessege("I have to get out of the deep desert!");

        for (float t = 0; t < allowedTime; t += Time.deltaTime)
        {
            Shader.SetGlobalFloat("_SandstormAdditional", t/allowedTime);

            if ((RegionBounds.GetPlayerRegion() != WorldRegion.OutOfBounds))
            {
                Shader.SetGlobalInteger("_InSandstorm", 0);
                Shader.SetGlobalFloat("_SandstormAdditional", 0);
                outOfBoundsRoutine = null;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        Refs.turtle.Mount();
        Refs.turtle.Teleport(lastSettlement.GetPosition());

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            Shader.SetGlobalFloat("_SandstormAdditional", 1-t);

            yield return new WaitForEndOfFrame();
        }
        Refs.ui.hud.SpawnGameMessege("I hot blew back by the Deep Desert wind...");
        Shader.SetGlobalInteger("_InSandstorm", 0);
        Shader.SetGlobalFloat("_SandstormAdditional", 0);
        outOfBoundsRoutine = null;
        yield break;
    }

    private void OnDisable()
    {
        Shader.SetGlobalInteger("_InSandstorm", 0);
        Shader.SetGlobalFloat("_SandstormAdditional", 0);
    }

    public override void OnSave(Save save)
    {
        save.playerPos = transform.position;     
    }

    public override void OnLoad(Save save)
    {
        if (save.isMounted)
            Refs.turtle.Mount();
        else
            Teleport(save.playerPos);
    }
}
