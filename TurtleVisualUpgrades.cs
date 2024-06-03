using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class TurtleVisualUpgrades : MonoBehaviour
{
    public enum TurtleVisualUpgrade
    { 
        WateringCan,
        LeftPack,
        RightPack,
        LeftTank,
        RightTank,

    }

    public SerializedDictionary<TurtleVisualUpgrade, List<GameObject>> upgrades;


    private void Awake()
    {
        foreach (var kvp in upgrades)
        {
            foreach (GameObject gmb in upgrades[kvp.Key])
            {
                gmb.SetActive(false);
            }
        }
    }

    public void ShowUpgrade(TurtleVisualUpgrade upgrade)
    {
        foreach (GameObject gmb in upgrades[upgrade])
        {
            gmb.SetActive(true);
        }
    }
}
