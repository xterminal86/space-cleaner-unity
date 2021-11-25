using UnityEngine;

public class BulletLame : BulletBase
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    if (_isColliding) return;

    _isColliding = true;

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int enemyLayer = LayerMask.NameToLayer("Enemy");

    if (collider.gameObject.layer == asteroidsLayer)
    {
      Asteroid a = collider.gameObject.GetComponentInParent<Asteroid>();
      if (a != null)
      {
        if (a.BreakdownLevel == GlobalConstants.AsteroidMaxBreakdownLevel)
        {
          a.ReceiveDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.LAME], this);
        }
        else
        {
          var go = Instantiate(HitAnimationPrefab, new Vector3(_rigidbodyComponent.position.x, _rigidbodyComponent.position.y, 0.0f), Quaternion.identity);

          Destroy(go, 1.0f);

          SoundManager.Instance.PlaySound(GlobalConstants.BulletSoundHitByType[GlobalConstants.BulletType.LAME], 0.25f);
        }
      }
    }
    else if (collider.gameObject.layer == enemyLayer)
    {
      UfoBase saucer = collider.gameObject.GetComponentInParent<UfoBase>();
      if (saucer != null)
      {
        var go = Instantiate(HitAnimationPrefab, new Vector3(_rigidbodyComponent.position.x, _rigidbodyComponent.position.y, 0.0f), Quaternion.identity);
        Destroy(go, 1.0f);

        saucer.ProcessDamage(GlobalConstants.BulletDamageByType[GlobalConstants.BulletType.LAME]);
      }
    }

    Destroy(gameObject);
  }
}
