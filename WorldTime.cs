using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime : SaveableBehaviour
{
    public static float CurrentDayTime = 800;

    public const float DayLength = 2400;
    public const float sunrise = 800;
    public const float sunset = 2100;

    public static float TimeScale = 2;

    public static System.Action OnNextDay;

    public float startingDayTime;

    override protected void Awake()
    {
        base.Awake();
        CurrentDayTime = startingDayTime;
        TimeScale = 2;
        OnNextDay = null;
    }

    private void Update()
    {
        //Debug.Log(CurrentDayTime);
        CurrentDayTime += Time.deltaTime * TimeScale;
        if (CurrentDayTime > DayLength)
        {
            //Next day
            OnNextDay?.Invoke();
            CurrentDayTime %= DayLength;
        }
    }

    public static float GetDayPercent()
    {
        return CurrentDayTime / DayLength;
    }

    public static bool IsNight()
    {
        return CurrentDayTime < sunrise || CurrentDayTime > sunset;
    }

    public override void OnSave(Save save)
    {
        save.worldTime = CurrentDayTime;
    }

    public override void OnLoad(Save save)
    {
        CurrentDayTime = save.worldTime;
    }
}
