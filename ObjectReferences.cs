using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Refs
{
    public static Player player;
    public static PlayerCamera playerCamera;
    public static Turtle turtle;
    public static UI ui;
    public static Controls controls { get { return player.controls; } }
    public static GamepadController gamepadController;
    public static MainQuest mainQuest;
    public static WeatherSystem weatherSystem;
    public static Contracts contracts;
    public static Inventory inventory;

    public static void Generate()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerCamera = GameObject.FindObjectOfType<PlayerCamera>();
        turtle = GameObject.FindObjectOfType<Turtle>();
        ui = GameObject.FindObjectOfType<UI>();
        gamepadController = GameObject.FindObjectOfType<GamepadController>();
        mainQuest = GameObject.FindObjectOfType<MainQuest>();
        weatherSystem = GameObject.FindObjectOfType<WeatherSystem>();
        mainQuest = GameObject.FindObjectOfType<MainQuest>();
        contracts = GameObject.FindObjectOfType<Contracts>();
        inventory = GameObject.FindObjectOfType<Inventory>();

    }
}
