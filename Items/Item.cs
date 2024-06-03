using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class ItemDisplayData
{
    public string displayName;
    public string displayDescription;
    public Sprite image;
    

}
[System.Serializable]
public class ItemAndAmount
{
    public Item item;
    public int amount;
}

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{


    public ItemDisplayData displayData;

    public int row;

    public bool unique;

    public bool clickable = false;
    public AudioClip clickSound;

    public UnityEvent OnClick, OnGet, OnLose;



    #region Item Events
    public void ChangeHat()
    {
        Refs.player.ChangeHat(int.Parse(name.Substring(3)));
    }

    public void AcquireLicence(int licence)
    {
        TransporterLicences.SetCurrentLicence((TransporterLicences.LicenceType)licence);

        switch (licence)
        {
            case 2:
                Refs.inventory.RemoveItem("TemporaryLicence");
                break;
            case 3:
                Refs.inventory.RemoveItem("ShortDistanceLicence");
                break;
            case 4:
                Refs.inventory.RemoveItem("MediumDistanceLicence");
                break;
            case 6:
                Refs.inventory.RemoveItem("ExtendedMediumDistanceLicence");
                break;
            case 7:
                Refs.inventory.RemoveItem("LongDistanceLicence");
                break;
        }
    }

    public void UseBoostFruit()
    {
        Refs.turtle.StartCoroutine(Refs.turtle.Boost(3, 40));
        Refs.inventory.RemoveItem(this);
    }
    public void UseWaterBottle()
    {
        Refs.turtle.hydration.currentHydration += 300;
        Refs.inventory.RemoveItem(this);
    }

    public void UseSleepingBag()
    {
        if (WorldTime.IsNight())
        {
            Refs.controls.Disable();
            Refs.gamepadController.UnrequestPointingMode("Backpack"); // quick hack
            Refs.playerCamera.StartCoroutine(Refs.playerCamera.Fade(5, 3, () => { 
                WorldTime.CurrentDayTime = 900; Refs.controls.Enable(); Sounds.PlayPlayerSound(FindObjectOfType<NPC>().sleepSound); 
            }));
        }
        else
            Refs.ui.hud.SpawnGameMessege("I can only sleep at night!");
    }

    public void EquipHydrationTank()
    {
        Refs.mainQuest.UpdateDescription();
        Refs.turtle.hydration.maxHydration += 500;

        if (Refs.inventory.items.First(x => x.item.name == "HydrationTank").amount == 1) // is first tank
            Refs.turtle.visualUpgrades.ShowUpgrade(TurtleVisualUpgrades.TurtleVisualUpgrade.LeftTank);
        else
            Refs.turtle.visualUpgrades.ShowUpgrade(TurtleVisualUpgrades.TurtleVisualUpgrade.RightTank);
    }

    public void UnlockLeftPack()
    {
        Refs.turtle.UnlockLeftPackUpgrade();
    }
    public void UnlockRightPack()
    {
        Refs.turtle.UnlockRightPackUpgrade();
        Refs.mainQuest.UpdateDescription();
    }

    public void UnlockWateringCan()
    {
        Refs.turtle.visualUpgrades.ShowUpgrade(TurtleVisualUpgrades.TurtleVisualUpgrade.WateringCan);
    }

    public void EndDemo()
    {
        Refs.ui.OpenReadable("End of Demo");
    }

    public void UseExperimentalStone()
    {
        Refs.mainQuest.TrySetStage(MainQuest.MainQuestStage.C4);
        Refs.inventory.RemoveItem(this);

        Vector3 pos = GameObject.Find("ExperimentalCallbackStone").transform.position + Vector3.down;

        Refs.turtle.StartCoroutine(Refs.turtle.TeleportWithAnimation(pos));

        
    }


    public void UseCallbackStone()
    {
        Vector3? pos =  FindObjectsOfType<Well>().First(x =>x.region == RegionBounds.GetPlayerRegion()).GetCallbackPosition();

        if (!pos.HasValue)
        {
            Refs.ui.hud.SpawnGameMessege("I don't have callback stone in this region!");
            return;
        }
        Refs.inventory.RemoveItem(this);


        Refs.turtle.StartCoroutine(Refs.turtle.TeleportWithAnimation(pos.Value));

    }

    #endregion
}
