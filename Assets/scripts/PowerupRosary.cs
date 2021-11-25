public class PowerupRosary : PowerupBase
{
  public override void Pickup(Player p)
  {
    base.Pickup(p);

    SoundManager.Instance.PlaySound("powerup-rosary", 0.5f, 2.0f);

    p.InitiateRosaryPowerup();

    Destroy(gameObject);
  }
}
