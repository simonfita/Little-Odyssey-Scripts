using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPalm : SaveableBehaviour, I_Interactable
{
    public enum LivingPalmState
    {
        Bud,
        Growing,
        Ripe,
        Harvested
    }

    [System.Serializable]
    public struct StageRendererConfiguration
    {
        public Mesh mesh;
        public Material[] dryMaterials;
        public Material[] wateredMaterials;
    }


    public Animator anim;
    public MeshFilter meshFilter;
    public Renderer rend;
    public List<StageRendererConfiguration> rendConfig;
    public Waterable waterable;

    public float growDistance;

    public AudioClip pickSound;

    [SerializeField]
    public List<List<Material>> materials;

    public List<ItemAndAmount> fruits;
    public List<GameObject> fruitsRenderers;
    private int fruitType;

    private LivingPalmState state = LivingPalmState.Bud;
    private bool watered;


    override protected void Awake()
    {
        base.Awake();

        meshFilter.mesh = rendConfig[0].mesh;
        rend.materials = rendConfig[0].dryMaterials;

        waterable.OnWatered += OnWatered;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, Refs.player.transform.position);


        if(dist>growDistance && !rend.isVisible)
            TryGrow();



    }

    private void TryGrow()
    {
        if (!watered)
            return;

        switch (state)
        {
            case LivingPalmState.Bud:
                state = LivingPalmState.Growing;
                watered = false;
                waterable.enabled = true;
                break;
            case LivingPalmState.Growing:
                state = LivingPalmState.Ripe;
                fruitType = Random.Range(0, fruits.Count);
                break;
            case LivingPalmState.Ripe:
                break;
            case LivingPalmState.Harvested:
                state = LivingPalmState.Bud;
                watered = false;
                waterable.enabled = true;
                break;
        }

        if (state != LivingPalmState.Ripe)
        {
            watered = false;
            waterable.enabled = true;
        }

        SetVisuals(state, watered);
    }

    private void SetVisuals(LivingPalmState newState, bool watered)
    {
        StageRendererConfiguration config = rendConfig[Mathf.Clamp((int)newState,0,2)];

        meshFilter.mesh = config.mesh;

        rend.materials = watered ? config.wateredMaterials : config.dryMaterials;

        for (int i = 0; i < fruitsRenderers.Count; i++)
        {
            fruitsRenderers[i].SetActive(newState == LivingPalmState.Ripe && i == fruitType);
        }
    }

    private void Harvest()
    {
        state = LivingPalmState.Harvested;
        SetVisuals(state, watered);
        
        Refs.inventory.AddItem(fruits[fruitType].item, fruits[fruitType].amount);
        Refs.ui.hud.SpawnGameMessege("I gathered the fruit");
        Sounds.PlayPlayerSound(pickSound);
    }

    public void OnWatered()
    {
        watered = true;

        SetVisuals(state, watered);

        Refs.ui.hud.SpawnGameMessege("I watered the palm");

    }

    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.NotMountedAndMounted; }

    public bool CanBeInteracted() { return state == LivingPalmState.Ripe; }

    public void Interact() { Harvest(); }

    public string GetInteractionText() { return "Harvest"; }

    public Transform GetInteractionTransform() { return transform; }

    [System.Serializable]
    public class CustomSave : Save.UniqueSave
    {
        public int state;
        public bool watered;
        public int fruitType;
    }


    public override void OnSave(Save save)
    {
        CustomSave customSave = new CustomSave();
        customSave.id = GetUnqiueID();
        customSave.state = (int)state;
        customSave.watered = watered;
        customSave.fruitType = fruitType;

        save.palms.Add(customSave);
    }

    public override void OnLoad(Save save)
    {
        CustomSave customSave = save.palms.Find(x => x.id == GetUnqiueID());

        state = (LivingPalmState)customSave.state;
        watered = customSave.watered;
        waterable.enabled = !watered;
        fruitType = customSave.fruitType;

        SetVisuals(state, watered);
    }
}
