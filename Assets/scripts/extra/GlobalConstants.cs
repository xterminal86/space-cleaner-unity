﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Callback();

public static class GlobalConstants
{
  // Maximum asteroids by default
  public const int AsteroidsMaxInstances = 50;
  // Possible maximum of asteroids from every breakdown level
  public const int MaximumAsteroidsOnScreen = AsteroidsMaxInstances * 33;
  // Number of asteroid breakdowns after it has been hit
  public const int AsteroidMaxBreakdownLevel = 4;

  public const int GuiHitpointsShieldBarLength = 20;

  public const float AsteroidMinSpeed = 25.0f;
  public const float AsteroidMaxSpeed = 200.0f;

  public const float AsteroidMinRotationSpeed = 10.0f;
  public const float AsteroidMaxRotationSpeed = 20.0f;

  // Time after which new asteroid will spawn
  // at the start of the game or after player has been killed
  public const float StartingSpawnRate = 10.0f;
  // Fastest spawning time interval
  public const float MaxSpawnRate = 1.0f;
  // Starting time will decrement by this value every spawn
  public const float SpawnRateDelta = 0.1f;
  // Shield restores one bar after this time
  public const float ShieldRechargeTimeout = 10.0f;
  // How much should current spawn rate be decreased
  public const float ClockPowerupValue = 2.0f;

  // Delay of spawned powerup after it disappears
  public const float PowerupLifetime = 6.0f;

  public const float PowerupSpawnPercent = 25.0f;

  // Random spread arc of asteroid from hitting the player
  public const float AsteroidBreakdownHalfArc = 60.0f;

  public const int MaxHighScoreEntries = 10;

  public static Dictionary<int, int> MaxUfosPerPlayerLevel = new Dictionary<int, int>()
  {
    { 0, 1 },
    { 1, 2 },
    { 2, 4 },
    { 3, 6 }
  };

  public static List<UfoController.UfoVariant> AllowedUfosAscendingList = new List<UfoController.UfoVariant>()
  {
    UfoController.UfoVariant.LAME, UfoController.UfoVariant.EMP, UfoController.UfoVariant.ELITE
  };

  public static Dictionary<int, List<UfoController.UfoVariant>> AllowedUfoVariantsByPlayerLevel = new Dictionary<int, List<UfoController.UfoVariant>>()
  {
    { 0, new List<UfoController.UfoVariant>() { UfoController.UfoVariant.LAME } },
    { 1, new List<UfoController.UfoVariant>() { UfoController.UfoVariant.LAME, UfoController.UfoVariant.EMP } },
    { 2, new List<UfoController.UfoVariant>() { UfoController.UfoVariant.LAME, UfoController.UfoVariant.EMP, UfoController.UfoVariant.ELITE } },
    { 3, new List<UfoController.UfoVariant>() { UfoController.UfoVariant.LAME, UfoController.UfoVariant.EMP, UfoController.UfoVariant.ELITE } }
  };

  public static Dictionary<int, int> AsteroidHitpointsByBreakdownLevel = new Dictionary<int, int>()
  {
    { 1, 40 },
    { 2, 20 },
    { 3, 10 },
    { 4, 1 }
  };

  public static Dictionary<int, int> AsteroidScoreByBreakdownLevel = new Dictionary<int, int>()
  {
    { 1, 1 },
    { 2, 2 },
    { 3, 4 },
    { 4, 8 }
  };

  public static Dictionary<UfoController.UfoVariant, int> UfoScoreByVariant = new Dictionary<UfoController.UfoVariant, int>()
  {
    { UfoController.UfoVariant.LAME,  20 },
    { UfoController.UfoVariant.EMP,   40 },
    { UfoController.UfoVariant.ELITE, 80 }
  };

  public static Dictionary<UfoController.UfoVariant, int> MaximumAllowedUfosByVariant = new Dictionary<UfoController.UfoVariant, int>()
  {
    { UfoController.UfoVariant.LAME,  6 },
    { UfoController.UfoVariant.EMP,   2 },
    { UfoController.UfoVariant.ELITE, 1 },
  };

  public static Dictionary<UfoController.UfoVariant, int> UfoSpawnChanceByVariant = new Dictionary<UfoController.UfoVariant, int>()
  {
    { UfoController.UfoVariant.LAME,  50 },
    { UfoController.UfoVariant.EMP,   30 },
    { UfoController.UfoVariant.ELITE, 30 }
  };

  // Number of asteroids to destroy (value) after given level (key) is reached
  public static Dictionary<int, int> ExperienceByLevel = new Dictionary<int, int>()
  {
    // 100, 500, 1000
    {0, 100}, {1, 500}, {2, 1000}
  };

  public const float PlayerRotationSpeed = 100.0f;
  public const float PlayerMoveSpeed = 3.0f;
  public const float BulletLameSpeed = 8.0f;
  public const float BulletMediumSpeed = 8.0f;
  public const float BulletStrongSpeed = 8.0f;
  public const float BulletLaserSpeed = 8.0f;
  public const float BulletEliteSpeed = 6.0f;
  public const float BulletEmpSpeed = 3.0f;

  public static string MenuMoveSound = "menu_move";
  public static string MenuSelectSound = "menu_select";
  public static string MenuBackSound = "menu_back";

  public static string PlayerPrefsConfigDataKey = string.Format("pp-config-v{0}", Application.version);
  public static string PlayerPrefsPlayerNameKey = "pp-player-name";
  public static string PlayerPrefsSoundVolumeKey = "pp-sound-volume";
  public static string PlayerPrefsMusicVolumeKey = "pp-music-volume";

  public static string HighscoreEntryPlayerNameKey = "e-name";
  public static string HighscoreEntryPlayerScoreKey = "e-score";
  public static string HighscoreEntryPlayerPhaseKey = "e-phase";
  public static string HighscoreEntryLameUfoCount = "e-ufo-1";
  public static string HighscoreEntryEMPUfoCount = "e-ufo-2";
  public static string HighscoreEntryEliteUfoCount = "e-ufo-3";

  public enum PowerupType
  {
    HEALTH = 0,
    SHIELD
  }

  public enum BulletType
  {
    LAME = 0,
    MEDIUM,
    STRONG,
    LASER,
    ELITE,
    EMP
  }

  public static Dictionary<BulletType, float> BulletSpeedByType = new Dictionary<BulletType, float>()
  {
    { BulletType.LAME,   BulletLameSpeed   },
    { BulletType.MEDIUM, BulletMediumSpeed },
    { BulletType.STRONG, BulletStrongSpeed }
  };

  public static Dictionary<BulletType, int> BulletDamageByType = new Dictionary<BulletType, int>()
  {
    { BulletType.LAME,    1 },
    { BulletType.MEDIUM,  4 },
    { BulletType.STRONG, 10 },
    { BulletType.LASER,   2 },
    { BulletType.ELITE,   6 }
  };

  public static Dictionary<BulletType, string> BulletSoundByType = new Dictionary<BulletType, string>()
  {
    { BulletType.LAME,   "ship_fire_lame"    },
    { BulletType.MEDIUM, "ship_fire_medium2" },
    { BulletType.STRONG, "ship_fire_strong"  }
  };

  public static Dictionary<BulletType, string> BulletSoundHitByType = new Dictionary<BulletType, string>()
  {
    { BulletType.LAME,   "hit_lame" },
    { BulletType.MEDIUM, "hit"      },
    { BulletType.STRONG, "hit"      },
    { BulletType.EMP,    "sparks"   }
  };

  public static Dictionary<BulletType, float> BulletSoundVolumesByType = new Dictionary<BulletType, float>()
  {
    { BulletType.LAME,   0.25f  },
    { BulletType.MEDIUM, 0.125f },
    { BulletType.STRONG, 0.25f  }
  };

  // Music will play in the order of listing here
  public static List<KeyValuePair<string, string>> MusicTracks = new List<KeyValuePair<string, string>>()
  {
    new KeyValuePair<string, string>("pb1", "Power Blade - Sector 7: The Computer"),
    new KeyValuePair<string, string>("dd", "Battletoads & Double Dragon - Robo Manus"),
    new KeyValuePair<string, string>("metroid", "Metroid - Brinstar"),
    new KeyValuePair<string, string>("green-planet", "Bucky O'Hare - Green Planet"),
    new KeyValuePair<string, string>("pb3", "Power Blade - Sector 5: Tanker"),
    new KeyValuePair<string, string>("pb4", "Power Blade - Sector 1: Space Center"),
    new KeyValuePair<string, string>("pb5", "Power Blade - Sector 2: Power Windmills"),
    new KeyValuePair<string, string>("pb7", "Power Blade - Sector 6: City"),
    new KeyValuePair<string, string>("city-lights", "Street Fighter 2010: The Final Fight - City Lights")
  };

  public static Dictionary<string, Int2> MusicTrackLoopPointsByName = new Dictionary<string, Int2>()
  {
    { "metroid",      new Int2(291104, 2481000) },
    { "green-planet", new Int2(211355, 2328000) },
    { "pb1",          new Int2(638145, 3377000) },
    { "pb3",          new Int2(385171, 1923000) },
    { "pb4",          new Int2(423377, 2116000) },
    { "pb5",          new Int2(264518, 3086200) },
    { "pb7",          new Int2(882204, 2998300) },
    { "dd",           new Int2(0,      2738500) },
    { "city-lights",  new Int2(0,      2560300) }

    /*
    { "metroid", new Vector2(320100, 2704000) },
    { "green-planet", new Vector2(231910, 2536208) },
    { "pb1", new Vector2(694560, 3676320) },
    { "pb3", new Vector2(421300, 2097000) },
    { "pb4", new Vector2(480000, 2400000) },
    { "pb5", new Vector2(290300, 3362300) },
    { "pb7", new Vector2(962400, 3266400) },
    { "pb8", new Vector2(694272, 1960512) }
    */
  };

  static List<Vector2> _dirRanges = new List<Vector2>()
  {
    new Vector2(-1.0f, -0.1f),
    new Vector2(0.1f, 1.0f)
  };

  public static Vector2 GetRandomDir()
  {
    int indexX = Random.Range(0, 2);
    int indexY = Random.Range(0, 2);

    Vector2 dx = _dirRanges[indexX];
    Vector2 dy = _dirRanges[indexY];

    float dirX = Random.Range(dx.x, dx.y);
    float dirY = Random.Range(dy.x, dy.y);

    return new Vector2(dirX, dirY).normalized;
  }

  public static Vector2 RotateVector2(this Vector2 v, float degrees)
  {
    return Quaternion.Euler(0, 0, degrees) * v;
  }
}

public class Int2
{
  int _x = 0;
  int _y = 0;

  public Int2()
  {
    _x = 0;
    _y = 0;
  }

  public Int2(int x, int y)
  {
    _x = x;
    _y = y;
  }

  public Int2(float x, float y)
  {
    _x = (int)x;
    _y = (int)y;
  }

  public Int2(Vector2 v2)
  {
    _x = (int)v2.x;
    _y = (int)v2.y;
  }

  public int X
  {
    set { _x = value; }
    get { return _x; }
  }

  public int Y
  {
    set { _y = value; }
    get { return _y; }
  }

  public override string ToString()
  {
    return string.Format("[Int2: X={0}, Y={1}]", X, Y);
  }
}
