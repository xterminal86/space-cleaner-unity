using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupClock : PowerupBase
{
  public override void Pickup(Player p)
  {
    base.Pickup(p);

    SoundManager.Instance.PlaySound("powerup-clock", 2.0f, 2.0f);

    p.AppReference.ReduceSpawnRate();

    Destroy(gameObject);
  }
}
