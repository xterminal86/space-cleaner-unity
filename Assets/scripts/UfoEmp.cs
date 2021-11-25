using UnityEngine;

public class UfoEmp : UfoBase
{
  public GameObject BulletEMP;

  protected override void SetupSpecific(UfoController.UfoVariant variant)
  {
    Hitpoints = 10;
    _maxHitPoints = Hitpoints;

    Shieldpoints = 40;
    _maxShieldPoints = Shieldpoints;

    _moveDirChangeTimeout = Random.Range(0.75f, 1.25f);

    _saucerSpeed = Random.Range(125.0f, 150.0f);

    _shootingCooldown = 4.0f;
  }

  protected override void Logic()
  {
    _timer += Time.smoothDeltaTime;
    _shootingCooldownCounter += Time.smoothDeltaTime;

    if (_timer > _moveDirChangeTimeout)
    {
      _timer = 0.0f;
      _moveDirChangeTimeout = Random.Range(0.75f, 1.25f);
      _direction = GlobalConstants.GetRandomDir();
    }

    CheckEMP();
  }

  void CheckEMP()
  {
    if (_shootingCooldownCounter > _shootingCooldown)
    {
      if (!_player.IsEmpLocked)
      {
        SpawnBullet(BulletEMP, GlobalConstants.BulletEmpSpeed);
        _shootingCooldownCounter = 0.0f;
      }
    }
  }

  protected override void PlayBulletCreatedSound()
  {
  }
}
