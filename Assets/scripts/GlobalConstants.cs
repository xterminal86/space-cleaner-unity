using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConstants 
{
  // Maximum asteroids by default
  public const int AsteroidsMaxInstances = 40;
  // Number of asteroid breakdowns after it has been hit
  public const int AsteroidMaxBreakdownLevel = 4;

  public const float AsteroidMinSpeed = 0.5f;
  public const float AsteroidMaxSpeed = 2.0f;

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

  // Delay of spawned powerup after it disappears
  public const int PowerupLifetimeMs = 5000;

  // Number of asteroids to destroy (value) after given level (key) is reached
  public static Dictionary<int, int> ExperienceByLevel = new Dictionary<int, int>()
  {
    {0, 25}, {1, 50}, {2, 100}
  };

  public const float PlayerRotationSpeed = 100.0f;
  public const float PlayerMoveSpeed = 3.0f;
  public const float BulletLameSpeed = 5.0f;
  public const float BulletMediumSpeed = 5.0f;
  public const float BulletStrongSpeed = 2.5f;

  public static string MenuMoveSound = "menu_move";
  public static string MenuSelectSound = "menu_select";
  public static string MenuBackSound = "menu_back";

  public enum BulletType
  {
    LAME = 0,
    MEDIUM,
    STRONG
  }

  public static Dictionary<BulletType, float> BulletSpeedByType = new Dictionary<BulletType, float>() 
  {
    { BulletType.LAME, BulletLameSpeed },
    { BulletType.MEDIUM, BulletMediumSpeed },
    { BulletType.STRONG, BulletStrongSpeed }
  };

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
