using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class WeatherSystem : SaveableBehaviour
{
    [Header("Sun")]
    public Light sunLight;
    public LensFlareComponentSRP sunFlare;
    public Gradient sunGradient;
    public Gradient skyboxGradient;
    public AnimationCurve shadowsCurve;
    public AnimationCurve sunFlareCurve;
    public float cloudRotationSpeed;

    [Header("HeatHaze")]
    public AnimationCurve heatHazeCurve;

    [Header("Sandstorms")]
    public Sandstorm sandstormPrefab;
    public List<Transform> sandstormSpawns;
    public float minSandstormDelay, maxSandstormDelay;
    private float sandstormDelay;
    private bool sandstormsEnabled;
    private Sandstorm currentSandstorm;

    private Vector4[] minilights = new Vector4[128];
    private int lightIndex =0;

    override protected void Awake()
    {
        base.Awake();
        RenderSettings.skybox = new Material(RenderSettings.skybox);
        
    }

    public void AddMiniLight(Vector3 pos)
    {
        if (lightIndex >= 128)
            return;
        minilights[lightIndex] = pos;
        lightIndex++;
        Shader.SetGlobalVectorArray("_MiniLights", minilights);
    }

    private void Update()
    {
        float t = WorldTime.GetDayPercent();
        UpdateSun(t);
        UpdateHeatHaze(t);
        UpdateSandstorms();
        RenderSettings.skybox.SetFloat("_Rotation", (Time.time * cloudRotationSpeed) % 360);
        Shader.SetGlobalInteger("_EnableMiniLights", WorldTime.IsNight() ? 1:0);
    }


    private void UpdateSun(float t)
    {     
        Shader.SetGlobalColor("_GlobalTint", sunGradient.Evaluate(t));
        RenderSettings.skybox.SetColor("_Tint", skyboxGradient.Evaluate(t));
        sunLight.shadowStrength = shadowsCurve.Evaluate(t);
        sunFlare.scale = sunFlareCurve.Evaluate(t);
    }

    private void UpdateHeatHaze(float t)
    {
        Shader.SetGlobalFloat("_HeatHazeStrength", heatHazeCurve.Evaluate(t));
    }

    public void UpdateSandstorms()
    {
        if (!Application.isPlaying)
            return;

        if (!sandstormsEnabled)
            return;

        if (currentSandstorm != null)
            return;

        sandstormDelay -= Time.deltaTime*WorldTime.TimeScale;
        if (sandstormDelay <= 0)
        { 
            SpawnSandstorm();
            sandstormDelay = Random.Range(minSandstormDelay, maxSandstormDelay);
        }

    }

    public void EnableSandstorms()
    {
        if (sandstormsEnabled)
            return;

        sandstormsEnabled = true;
        Transform randomSpawn = sandstormSpawns[1];
        currentSandstorm = Instantiate(sandstormPrefab, randomSpawn.position, randomSpawn.rotation);
    }

    private void SpawnSandstorm()
    {
        Transform randomSpawn = sandstormSpawns[Random.Range(0,sandstormSpawns.Count)];
        currentSandstorm = Instantiate(sandstormPrefab, randomSpawn.position, randomSpawn.rotation);
    }

    override protected void OnDestroy()
    {
        base.OnDestroy();
        Shader.SetGlobalFloat("_HeatHazeStrength", 0);
        Shader.SetGlobalColor("_GlobalTint", sunGradient.Evaluate(0.5f));

    }

    public override void OnSave(Save save)
    {
        save.sanstormsEnabled = sandstormsEnabled;
        save.sandstormDelay = sandstormDelay;
    }

    public override void OnLoad(Save save)
    {
        sandstormsEnabled = save.sanstormsEnabled;
        sandstormDelay = save.sandstormDelay;
        /*if (sandstormsEnabled)
        {
            Invoke(nameof(SpawnSandstorm), Random.Range(minSandstormDelay, maxSandstormDelay));
        }*/
    }
}
