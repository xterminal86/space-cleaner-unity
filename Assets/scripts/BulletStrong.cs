using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStrong : BulletBase 
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    if (_isColliding) return;

    _isColliding = true;

    var go = Instantiate(HitAnimationPrefab, new Vector3(_rigidbodyComponent.position.x, _rigidbodyComponent.position.y, 0.0f), Quaternion.identity);

    Destroy(go, 1.0f);

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int playerLayer = LayerMask.NameToLayer("Player");

    if (collider.gameObject.layer == asteroidsLayer)
    {
      Asteroid a = collider.gameObject.GetComponentInParent<Asteroid>();
      if (a != null)
      {
        SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.MEDIUM], 0.25f);

        a.ReceiveDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.STRONG], this);
      }
    }
    else if (collider.gameObject.layer == playerLayer)
    {
      UFO saucer = collider.gameObject.GetComponentInParent<UFO>();
      if (saucer != null)
      {         
        saucer.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.STRONG]);
      }
    }

    Destroy(gameObject);
  }
}
