using System.Collections;
using UnityEngine;

public class UfoBase : MonoBehaviour
{
  public Rigidbody2D RigidbodyComponent;
  public Collider2D UfoCollider;
  public Collider2D ShieldCollider;
  public SpriteRenderer ShieldSprite;

  public GameObject BulletLaserPrefab;
  public GameObject EMPLockoutObject;

  public AudioSource HitSound;
  public AudioSource EnergyHitSound;

  public Animator CurrentAnimator;

  [HideInInspector]
  public int Shieldpoints = 20;

  [HideInInspector]
  public int Hitpoints = 20;

  protected int _maxHitPoints = 20;
  protected int _maxShieldPoints = 20;

  float _shieldRechargeTimer = 0.0f;
  Color _shieldColor = Color.white;

  protected Vector2 _direction = Vector2.zero;

  protected GameScript _app;
  protected Player _player;

  protected UfoController.UfoVariant _currentVariant = UfoController.UfoVariant.LAME;
  public UfoController.UfoVariant CurrentVariant
  {
    get { return _currentVariant; }
  }

  float[] _screenRect;
  void Awake()
  {
    _app = GameObject.Find("App").GetComponent<GameScript>();
    _player = GameObject.Find("player").GetComponent<Player>();

    _screenRect = _app.ScreenRect;

    _shieldColor.a = 0.0f;

    Physics2D.IgnoreCollision(ShieldCollider, UfoCollider);

    _direction = GlobalConstants.GetRandomDir();
  }

  protected UfoController _ufoController;
  public void Setup(UfoController controllerRef, UfoController.UfoVariant variant)
  {
    _ufoController = controllerRef;
    _currentVariant = variant;

    _ufoController.SpawnedUfosByVariant[variant]++;

    SetupSpecific(variant);
  }

  protected virtual void SetupSpecific(UfoController.UfoVariant variant)
  {
    Hitpoints = 20;
    _maxHitPoints = Hitpoints;

    Shieldpoints = 20;
    _maxShieldPoints = Shieldpoints;

    _moveDirChangeTimeout = Random.Range(1.0f, 2.0f);

    _saucerSpeed = Random.Range(50.0f, 100.0f);

    _shootingCooldown = 3.0f;
  }

  protected float _shootingCooldown = 3.0f;
  protected float _moveDirChangeTimeout = 0.0f;
  protected float _timer = 0.0f;
  protected float _shootingCooldownCounter = 0.0f;
  protected virtual void Logic()
  {
    _timer += Time.smoothDeltaTime;
    _shootingCooldownCounter += Time.smoothDeltaTime;

    if (_timer > _moveDirChangeTimeout)
    {
      _timer = 0.0f;
      _moveDirChangeTimeout = Random.Range(1.0f, 2.0f);
      _direction = GlobalConstants.GetRandomDir();
    }

    if (_shootingCooldownCounter > _shootingCooldown)
    {
      _shootingCooldownCounter = 0.0f;
      SpawnBullet(BulletLaserPrefab, GlobalConstants.BulletLaserSpeed);
    }
  }

  protected void SpawnBullet(GameObject bulletPrefab, float bulletSpeed)
  {
    Vector2 shotDir = _player.RigidbodyComponent.position - RigidbodyComponent.position;
    shotDir.Normalize();

    float angle = Vector2.Angle(Vector2.up, shotDir);
    Vector3 cross = Vector3.Cross(Vector2.up, shotDir);

    if (cross.z < 0.0f)
    {
      angle = 360 - angle;
    }

    Quaternion q = Quaternion.Euler(0.0f, 0.0f, angle);

    var go = Instantiate((bulletPrefab == null) ? BulletLaserPrefab : bulletPrefab, RigidbodyComponent.position, q);
    BulletBase bc = go.GetComponent<BulletBase>();
    Physics2D.IgnoreCollision(ShieldCollider, bc.Collider);
    Physics2D.IgnoreCollision(UfoCollider, bc.Collider);
    bc.Propel(shotDir, bulletSpeed);

    PlayBulletCreatedSound();
  }

  protected virtual void PlayBulletCreatedSound()
  {
    SoundManager.Instance.PlaySound("laser", 0.25f, 1.0f, true);
  }

  void Update()
  {
    if (_empLocked)
    {
      AdjustShieldAlpha();
      return;
    }

    ShieldCollider.gameObject.SetActive(Shieldpoints != 0);

    RechargeShield();
    AdjustShieldAlpha();

    if (!_app.IsGameOver)
    {
      Logic();
    }
  }

  Vector2 _position = Vector2.zero;
  protected float _saucerSpeed = 0.0f;
  float _offset = 0.5f;
  Vector2 _velocity = Vector2.zero;
  void FixedUpdate()
  {
    _position = RigidbodyComponent.position;

    if (RigidbodyComponent.position.x < _screenRect[0] - _offset)
    {
      _position.x = _screenRect[2] + _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.x > _screenRect[2] + _offset)
    {
      _position.x = _screenRect[0] - _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.y < _screenRect[1] - _offset)
    {
      _position.y = _screenRect[3] + _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.y > _screenRect[3] + _offset)
    {
      _position.y = _screenRect[1] - _offset;
      RigidbodyComponent.position = _position;
    }

    _velocity = _direction * (_saucerSpeed * Time.fixedDeltaTime);

    RigidbodyComponent.velocity = _velocity;
  }

  void RechargeShield()
  {
    if (Shieldpoints < _maxShieldPoints)
    {
      _shieldRechargeTimer += Time.smoothDeltaTime;

      if (_shieldRechargeTimer > GlobalConstants.ShieldRechargeTimeout)
      {
        _shieldRechargeTimer = 0.0f;
        ReceiveShieldDamage(-1);
      }
    }
  }

  void AdjustShieldAlpha()
  {
    if (_shieldColor.a > 0.0f)
    {
      _shieldColor.a -= Time.smoothDeltaTime;
    }

    _shieldColor.a = Mathf.Clamp(_shieldColor.a, 0.0f, 1.0f);

    ShieldSprite.color = _shieldColor;
  }

  public void ReceiveShieldDamage(int damageReceived)
  {
    Shieldpoints -= damageReceived;

    Shieldpoints = Mathf.Clamp(Shieldpoints, 0, _maxShieldPoints);
  }

  public void ForceDestroy()
  {
    SoundManager.Instance.PlaySound("ship_explode", 0.25f);

    var go = Instantiate(_app.PlayerDeathEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
    Destroy(go, 2.0f);

    Destroy(gameObject);
  }

  void OnDestroy()
  {
    _ufoController.SpawnedUfosByVariant[_currentVariant]--;
  }

  public void ReceiveDamage(int damageReceived)
  {
    Hitpoints -= damageReceived;

    Hitpoints = Mathf.Clamp(Hitpoints, 0, _maxHitPoints);

    if (Hitpoints == 0)
    {
      _app.Score += GlobalConstants.UfoScoreByVariant[_currentVariant];
      _app.UfosKilled[_currentVariant]++;
      _player.AddExperience(GlobalConstants.UfoScoreByVariant[_currentVariant]);

      SoundManager.Instance.PlaySound("ship_explode", 0.25f);

      var go = Instantiate(_app.PlayerDeathEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
      Destroy(go, 2.0f);

      _app.SpawnPowerup(RigidbodyComponent.position);

      Destroy(gameObject);
    }
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    int playerLayer = LayerMask.NameToLayer("Player");

    /*
    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");

    if (other.gameObject.layer == asteroidsLayer)
    {
      Asteroid asteroid = other.gameObject.GetComponentInParent<Asteroid>();

      // GetComponent returns null if object is inactive
      if (asteroid == null)
      {
        return;
      }

      int damageDealt = (GlobalConstants.AsteroidMaxBreakdownLevel + 1) - asteroid.BreakdownLevel;

      ProcessDamage(damageDealt);

      //Vector2 v = RigidbodyComponent.position - asteroid.RigidbodyComponent.position;
      Vector2 v = asteroid.RigidbodyComponent.position - RigidbodyComponent.position;

      float angle = Random.Range(-GlobalConstants.AsteroidBreakdownHalfArc, GlobalConstants.AsteroidBreakdownHalfArc);

      Vector2 newDir = GlobalConstants.RotateVector2(v, angle);
      newDir.Normalize();

      asteroid.HandleCollision(newDir);
    }
    */
    if (other.gameObject.layer == playerLayer)
    {
      ProcessDamage(1, null);
    }
  }

  bool _empLocked = false;
  public void SetEMPLockout()
  {
    if (!_empLocked)
    {
      EMPLockoutObject.SetActive(true);
      _empLocked = true;
      StartCoroutine(EMPUnblockRoutine());
    }
  }

  const float _empBlockTimeSeconds = 5.0f;
  IEnumerator EMPUnblockRoutine()
  {
    Vector3 pos = transform.position;

    float timer = 0.0f;
    while (timer < _empBlockTimeSeconds)
    {
      pos = transform.position;
      pos.y += 0.6f;
      pos.z = -2.0f;
      EMPLockoutObject.transform.position = pos;
      timer += Time.smoothDeltaTime;
      yield return null;
    }

    _empLocked = false;

    EMPLockoutObject.SetActive(false);

    yield return null;
  }

  public void ProcessDamage(int damage, BulletBase from)
  {
    bool isOk = (from != null && !(from is BulletEmp)) || (from == null);

    if (Shieldpoints != 0)
    {
      if (isOk)
      {
        EnergyHitSound.volume = 0.1f * SoundManager.Instance.SoundVolume;
        EnergyHitSound.Play();
      }

      _shieldColor.a = 1.0f;
      ReceiveShieldDamage(1);
    }
    else
    {
      if (isOk)
      {
        HitSound.volume = 0.5f * SoundManager.Instance.SoundVolume;
        HitSound.Play();
      }

      ReceiveDamage(damage);
    }
  }
}
