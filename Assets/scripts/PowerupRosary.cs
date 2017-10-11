using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupRosary : PowerupBase 
{
  public override void Pickup(Player p)
  {
    p.InitiateRosaryPowerup();

    Destroy(gameObject);
  }
}
