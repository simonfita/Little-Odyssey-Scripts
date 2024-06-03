using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(fileName = "InputPromptsTable", menuName = "ScriptableObjects/InputPromptsTable")]
public class InputPromptsTable : ScriptableObject
{
    [SerializedDictionary("Key", "Image")]
    public SerializedDictionary<Key, Sprite> images;
}
