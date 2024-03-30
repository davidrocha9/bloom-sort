using System.Collections.Generic;
using UnityEngine;

public class Vector
{
    public int x { get; set; }
    public int y { get; set; }
}


public static class Constants
{
    public const string COLOR_RED = "#FF0000";
    public const string COLOR_GREEN = "#00FF00";
    public const string COLOR_BLUE = "#0000FF";

    public static List<string> AVAILABLE_COLORS = new List<string>
    {
        "#FF0000", // Red
        "#0000FF", // Blue
        "#FFFF00"  // Yellow
    };

    
    public static int MAX_NUMBER_OF_CHUNKS = 6;
    public static int NUMBER_OF_TILES = 24;
    public static List<Vector3> SPAWN_COORDS = new List<Vector3>
    {
        new Vector3(-4, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(4, 0, 0)
    };

    public static int SCREEN_WIDTH = Screen.width;
    public static int SCREEN_HEIGHT = Screen.height;
    public static float CANVAS_LOCAL_SCALE;
    public static float MAX_X = -7.15f;
    public static float MAX_Y = 25.2f;
    public static float MIN_X = 7.2f;
    public static float MIN_Y = 3.6f;
    public static float UNIT = 3.6f;

    public static Vector TOP = new Vector { x = 0, y = 1 };
    public static Vector BOTTOM = new Vector { x = 0, y = -1 };
    public static Vector LEFT = new Vector { x = 1, y = 0 };
    public static Vector RIGHT = new Vector { x = -1, y = 0 };


    public static List<Vector> directions = new List<Vector> {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    };
}