using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleStockpile : Stockpile
{
    private Vector3[] previousPosition = new Vector3[2];
    private Quaternion[] previousRotation = new Quaternion[2];

    override protected void Awake()
    {
        Size = 2;
    }



    public void FreeMiddle()
    {
        Size -= 2;

        if (Size >= 1)
        {
            previousPosition[0] = itemPlaces[0].localPosition;
            previousRotation[0] = itemPlaces[0].localRotation;
            itemPlaces[0].localPosition = itemPlaces[2].localPosition;
            itemPlaces[0].localRotation = itemPlaces[2].localRotation;
        }

        if (Size >= 2)
        {
            previousPosition[1] = itemPlaces[1].localPosition;
            previousRotation[1] = itemPlaces[1].localRotation;
            itemPlaces[1].localPosition = itemPlaces[3].localPosition;
            itemPlaces[1].localRotation = itemPlaces[3].localRotation;
        }
    }
    public void ReclaimMiddle()
    {
        Size += 2;
        
        if (Size >= 3)
        {
            itemPlaces[0].localPosition = previousPosition[0];
            itemPlaces[0].localRotation = previousRotation[0];
        }

        if (Size >= 4)
        {
            itemPlaces[1].localPosition = previousPosition[1];
            itemPlaces[1].localRotation = previousRotation[1];
        }

        previousPosition = new Vector3[2];
        previousRotation = new Quaternion[2];
    }
}
