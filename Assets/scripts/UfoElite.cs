using System.Collections;
using UnityEngine;

public class UfoElite : UfoBase
{
  public GameObject BulletEMP;

  protected override void SetupSpecific(UfoController.UfoVariant variant)
  {
    Hitpoints = 200;
    _maxHitPoints = Hitpoints;

    Shieldpoints = 400;
    _maxShieldPoints = Shieldpoints;

    _moveDirChangeTimeout = Random.Range(2.0f, 4.0f);

    _saucerSpeed = Random.Range(25.0f, 50.0f);

    _shootingCooldown = 6.0f;
  }

  float _empShootCounter = 0.0f;
  const float _empShootTimeout = 4.0f;
  protected override void Logic()
  {
    _timer += Time.smoothDeltaTime;
    _shootingCooldownCounter += Time.smoothDeltaTime;
    _empShootCounter += Time.smoothDeltaTime;

    if (_timer > _moveDirChangeTimeout)
    {
      _timer = 0.0f;
      _moveDirChangeTimeout = Random.Range(2.0f, 4.0f);
      _direction = GlobalConstants.GetRandomDir();
    }

    if (_shootingCooldownCounter > _shootingCooldown)
    {
      _shootingCooldownCounter = 0.0f;
      StartCoroutine(ShootRoutine());
    }

    if (_empShootCounter > _empShootTimeout)
    {
      _empShootCounter = 0.0f;
      SpawnBullet(BulletEMP, GlobalConstants.BulletEmpSpeed);
    }
  }

  IEnumerator ShootRoutine()
  {
    int bullets = 0;
    while (bullets < 3)
    {
      SpawnBullet(null, GlobalConstants.BulletLaserSpeed);
      bullets++;
      yield return new WaitForSeconds(0.3f);
    }

    yield return null;
  }

  protected override void PlayBulletCreatedSound(BulletBase bc)
  {
    if (!(bc is BulletEmp))
    {
      SoundManager.Instance.PlaySound("laser", 0.4f, 0.5f, true);
    }
  }
}
