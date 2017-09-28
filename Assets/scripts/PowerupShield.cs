using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupShield : PowerupBase 
{
  public override void Pickup(Player p)
  {
    SoundManager.Instance.PlaySound("shield_powerup", 0.125f, 1.0f);

    p.Shieldpoints += 10;

    Destroy(gameObject);
  }
}
