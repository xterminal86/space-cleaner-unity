﻿using System.Collections;
using UnityEngine;

public class BulletElite : BulletBase
{
  public override void Propel(Vector2 direction, float bulletSpeed)
  {
    base.Propel(direction, bulletSpeed);

    _borderOffset = 3.0f;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
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
        WaitForEndOfTrailDestroy();

        player.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.ELITE], this);
      }
    }
    else if (collider.gameObject.layer == enemyLayer)
    {
      UfoBase u = collider.gameObject.GetComponentInParent<UfoBase>();
      if (u != null)
      {
        WaitForEndOfTrailDestroy();

        u.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.ELITE], this);
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
