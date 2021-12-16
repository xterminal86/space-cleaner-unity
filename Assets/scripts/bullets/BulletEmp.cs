using UnityEngine;

public class BulletEmp : BulletBase
{
  const float _sparksVolume = 0.3f;

  public override void Propel(Vector2 direction, float bulletSpeed)
  {
    base.Propel(direction, bulletSpeed);

    _borderOffset = 3.0f;

    _app.ActiveEmpBullets++;

    SoundManager.Instance.PlaySoundLooped("bullet-emp2", 0.8f, 0.5f);
  }

  void OnDestroy()
  {
    _app.ActiveEmpBullets--;

    if (_app.ActiveEmpBullets == 0 && SoundManager.isInstantinated)
    {
      SoundManager.Instance.StopLoopedSound("bullet-emp2");
    }
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
        SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.EMP], _sparksVolume, 1.0f, false);

        if (player.Shieldpoints == 0)
        {
          //
          // Sometimes collider may not get deactivated,
          // because OnTriggerEnter2D() is driven by physics engine
          // and FixedUpdate() as the result, which may be called
          // variable number of times per frame. So, as I see it,
          // shield could gain 1 point and thus enable the collider,
          // but we still get in this method, thus leaving shield collider
          // enabled. Kinda strange, but that's my guess.
          //
          player.ShieldCollider.gameObject.SetActive(false);

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
          player.ProcessDamage(0, this);
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
        SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.EMP], _sparksVolume, 1.0f, false);

        if (u.Shieldpoints == 0)
        {
          // See comments for player case above
          u.ShieldCollider.gameObject.SetActive(false);

          u.SetEMPLockout();
        }
        else
        {
          u.ProcessDamage(0, this);
          u.Shieldpoints = 0;
        }

        Destroy(gameObject);
      }
    }
  }
}
