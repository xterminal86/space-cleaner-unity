using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLame : BulletBase 
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    if (_isColliding) return;

    _isColliding = true;

    var go = Instantiate(HitAnimationPrefab, new Vector3(_rigidbodyComponent.position.x, _rigidbodyComponent.position.y, 0.0f), Quaternion.identity);

    Destroy(go, 1.0f);

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");

    if (collider.gameObject.layer == asteroidsLayer)
    {
    }

    Destroy(gameObject);
  }
}
