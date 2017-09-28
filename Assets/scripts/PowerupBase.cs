using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBase : MonoBehaviour 
{
  float _timer = 0.0f;
  void Update()
  {
    if (_timer > GlobalConstants.PowerupLifetime)
    {
      Destroy(gameObject);
      return;
    }

    _timer += Time.smoothDeltaTime;
  }

  public virtual void Pickup(Player p)
  {
  }
}
