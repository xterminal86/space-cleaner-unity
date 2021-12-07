using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHealth : PowerupBase
{
  public override void Pickup(Player p)
  {
    base.Pickup(p);

    SoundManager.Instance.PlaySound("health_powerup", 0.5f, 1.0f);

    p.ReceiveDamage(-10);

    Destroy(gameObject);
  }
}
