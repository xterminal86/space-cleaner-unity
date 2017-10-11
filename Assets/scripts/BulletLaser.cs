using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaser : BulletBase 
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

        a.ReceiveDamage(GlobalConstants.AsteroidHitpointsByBreakdownLevel[1], this);

        StartCoroutine(DisableColliderRoutine());
      }
    }
    else if (collider.gameObject.layer == playerLayer)
    {
      Player player = collider.gameObject.GetComponentInParent<Player>();
      if (player != null)
      {        
        Destroy(gameObject);

        player.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.LASER]);
      }
    }
    else if (collider.gameObject.layer == enemyLayer)
    {
      UFO u = collider.gameObject.GetComponentInParent<UFO>();
      if (u != null)
      {
        Destroy(gameObject);

        u.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.LASER]);
      }
    }
  }

  float _timer = 0.0f;
  IEnumerator DisableColliderRoutine()
  {
    Collider.enabled = false;

    while (_timer < 0.25f)
    {
      _timer += Time.smoothDeltaTime;

      yield return null;
    }

    _timer = 0.0f;

    Collider.enabled = true;

    yield return null;
  }
}
