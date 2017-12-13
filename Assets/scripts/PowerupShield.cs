using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupShield : PowerupBase 
{
  public override void Pickup(Player p)
  {
    base.Pickup(p);

    SoundManager.Instance.PlaySound("shield_powerup", 0.25f, 1.0f);

    p.ReceiveShieldDamage(-10);

    Destroy(gameObject);
  }
}
