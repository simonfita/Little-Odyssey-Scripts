using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TokenOutfits", menuName = "ScriptableObjects/Outfits")]
public class TokenOutfits : ScriptableObject
{
    public List<Material> hairColors;

    public List<Sprite> faces = new List<Sprite>();
    public List<Sprite> facialHairs = new List<Sprite>();
    public List<Sprite> hairs = new List<Sprite>();
    public List<Sprite> hats = new List<Sprite>();

    public List<Sprite> bodies = new List<Sprite>();
    public List<Sprite> backArms = new List<Sprite>();
    public List<Sprite> frontArms = new List<Sprite>();

    public List<Sprite> backArmsHolding = new List<Sprite>();
    public List<Sprite> frontArmsHolding = new List<Sprite>();

}
