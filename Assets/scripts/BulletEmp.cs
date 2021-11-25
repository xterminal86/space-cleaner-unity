using UnityEngine;

public class BulletEmp : BulletBase
{
  public override void Propel(Vector2 direction, float bulletSpeed)
  {
    base.Propel(direction, bulletSpeed);

    _borderOffset = 3.0f;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    /*
    Rigidbody2D rb = collider.gameObject.GetComponentInParent<Rigidbody2D>();
    Vector2 v = RigidbodyComponent.position - rb.position;

    // Prevent collision on all objects after asteroid breakdown
    if (v.magnitude < 0.15f)
    {
      return;
    }
    */

    var go = Instantiate(HitAnimationPrefab, new Vector3(_rigidbodyComponent.position.x, _rigidbodyComponent.position.y, 0.0f), Quaternion.identity);
    Destroy(go, 1.0f);

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int playerLayer = LayerMask.NameToLayer("Player");
    int enemyLayer = LayerMask.NameToLayer("Enemy");

    if (collider.gameObject.layer == asteroidsLayer)
    {
      Asteroid a = collider.gameObject.GetComponentInParent<Asteroid>();
      if (a != null)
      {
        SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.MEDIUM], 0.25f, 1.0f, false);
        Destroy(gameObject);
      }
    }
    else if (collider.gameObject.layer == playerLayer)
    {
      Player player = collider.gameObject.GetComponentInParent<Player>();
      if (player != null)
      {
        if (player.Shieldpoints == 0)
        {
          SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.EMP], 0.25f, 1.0f, false);
          player.SetEMPLockout();
        }
        else
        {
          //
          // ProcessDamage calls ReceiveShieldDamage() inside
          // so if this call were to be before conditions above, we could still
          // get to the 'Shieldpoints == 0' branch
          // even if player had 1 shieldpoint left, because shield always
          // gets 1 point of damage.
          //
          player.ProcessDamage(0);

          player.Shieldpoints = 0;
        }

        Destroy(gameObject);
      }
    }
    else if (collider.gameObject.layer == enemyLayer)
    {
      UfoBase u = collider.gameObject.GetComponentInParent<UfoBase>();
      if (u != null)
      {
        if (!(u is UfoEmp))
        {
          if (u.Shieldpoints == 0)
          {
            SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.EMP], 0.25f, 1.0f, false);
            u.SetEMPLockout();
          }
          else
          {
            u.ProcessDamage(0);
            u.Shieldpoints = 0;
          }
        }

        Destroy(gameObject);
      }
    }
  }
}
