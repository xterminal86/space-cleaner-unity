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
    p.AppReference.ResetPowerupLock();
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (collider.name == "ship")
    {
      Player p = collider.gameObject.GetComponentInParent<Player>();

      if (p != null)
      {
        if (this is PowerupClock || this is PowerupRosary)
        {
          p.AppReference.SpecialPowerupPicked();
        }

        Pickup(p);
      }
    }
  }
}
