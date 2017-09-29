using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoSingleton<GameStats> 
{
  public Config GameConfig = new Config();

  public override void Initialize()
  {
    GameConfig.ReadConfig();
  }
}
