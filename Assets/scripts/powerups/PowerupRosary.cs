public class PowerupRosary : PowerupBase
{
  public override void Pickup(Player p)
  {
    base.Pickup(p);

    SoundManager.Instance.PlaySound("powerup-rosary", 1.0f, 2.0f);

    p.InitiateRosaryPowerup();

    Destroy(gameObject);
  }
}
