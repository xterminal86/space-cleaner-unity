using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConstants 
{
  public static string MenuMoveSound = "menu_move";
  public static string MenuSelectSound = "menu_select";
  public static string MenuBackSound = "menu_back";

  public static List<string> MusicTracks = new List<string>()
  {
    "metroid",
    "green-planet",
    "pb1",
    "pb3",
    "pb4",
    "pb5",
    "pb7",
    "pb8",
  };

  public static Dictionary<string, Vector2> MusicTrackLoopPointsByName = new Dictionary<string, Vector2>()
  {
    { "metroid", new Vector2(320100, 2704000) },
    { "green-planet", new Vector2(231910, 2536208) },
    { "pb1", new Vector2(694560, 3676320) },
    { "pb3", new Vector2(421300, 2097000) },
    { "pb4", new Vector2(480000, 2400000) },
    { "pb5", new Vector2(290300, 3362300) },
    { "pb7", new Vector2(962400, 3266400) },
    { "pb8", new Vector2(694272, 1960512) }
  };
}
