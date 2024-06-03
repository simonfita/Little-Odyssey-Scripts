using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransporterLicences
{
    public enum LicenceType
    { 
        None,
        Temprorary,
        ShortDistance,
        MediumDistance,
        ExtendedMediumDistance,
        Sailing,
        LongDistance,
        Royal,
    }

    public static LicenceType currentLicence { get; private set; } = LicenceType.None;
    
    public static void SetCurrentLicence(LicenceType type)
    { 
        currentLicence = type;
        Refs.mainQuest.GotTransporterLicence(type);

        foreach(NoticeBoard board in GameObject.FindObjectsOfType<NoticeBoard>())
            board.Invoke(nameof(board.Progress),0.1f); // quick fix - progressing contracts happen on next update
    }

    public static int GetMaxContractAmount()
    {
        switch (currentLicence)
        {
            case LicenceType.Temprorary:
                return 2;
            case LicenceType.ShortDistance:
                return 3;
            case LicenceType.MediumDistance:
                return 3;
            case LicenceType.ExtendedMediumDistance:
                return 4;
            case LicenceType.Sailing:
                return 4;
            case LicenceType.LongDistance:
                return 4;
            case LicenceType.Royal:
                return 5;
        }
        return 0;
    }

    public static bool HasThisOrHigher(LicenceType type)
    {
        return (int)currentLicence >= (int)type;
    }

    public static void OnSave(Save save)
    {
        save.licence = (int)currentLicence;

    }

    public static void OnLoad(Save save)
    {
        SetCurrentLicence((LicenceType)save.licence);
    }

}
