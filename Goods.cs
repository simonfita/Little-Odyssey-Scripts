using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoodsTypes
{
    Bag,
    Vase,
    Box,
    Other,
    Animal,
    Charging,
    Sack,
    Barrel,
}


public class Goods : MonoBehaviour
{
    public GoodsTypes type;

    public ObjectLabel label;
    public AudioClip leaveAudio;

}
