using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour 
{
  public GameScript AppReference;

  public Rigidbody2D RigidbodyComponent;
  public Collider2D PlayerCollider;
  public Collider2D ShieldCollider;
  public Transform ShotPoint;
  public SpriteRenderer ShieldSprite;

  public List<GameObject> Bullets;

  int _currentWeapon = 0;

  float _rotation = 0.0f;
  float _acceleration = 0.0f;
  float _cos = 0.0f, _sin = 0.0f;

  [HideInInspector]
  public int Experience = 0;

  [HideInInspector]
  public int Level = 0;

  [HideInInspector]
  public int Hitpoints = 0;

  [HideInInspector]
  public int Shieldpoints = 0;

  int _maxPoints = 20;
  public int MaxPoints
  {
    get { return _maxPoints; }
  }

  void Awake()
  {
    Hitpoints = _maxPoints;
    Shieldpoints = _maxPoints;

    _shieldColor.a = 0.0f;

    Physics2D.IgnoreCollision(ShieldCollider, PlayerCollider);
  }

  Vector2 _direction = Vector2.zero;
  void Update()
  {
    if (Input.GetKey(KeyCode.A))
    {
      _rotation += GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    } 

    if (Input.GetKey(KeyCode.D))
    {
      _rotation -= GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    }

    #if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.LeftBracket))
    {
      _currentWeapon--;
    }
    else if (Input.GetKeyDown(KeyCode.RightBracket))
    {
      _currentWeapon++;
    }

    _currentWeapon = Mathf.Clamp(_currentWeapon, 0, GlobalConstants.BulletSpeedByType.Count - 1);

    AppReference.SetWeapon(_currentWeapon);
    #endif

    if (Input.GetKeyDown(KeyCode.Space))
    {      
      Quaternion q = Quaternion.Euler(0.0f, 0.0f, _rotation);

      GameObject go = Instantiate(Bullets[_currentWeapon], new Vector3(ShotPoint.position.x, ShotPoint.position.y, 0.0f), q);
      var bullet = go.GetComponent<BulletBase>();
      if (_currentWeapon == 2)
      {
        bullet.RigidbodyComponent.rotation = _rotation;
      }
      Physics2D.IgnoreCollision(PlayerCollider, bullet.Collider);
      Physics2D.IgnoreCollision(ShieldCollider, bullet.Collider);
      float volume = GlobalConstants.BulletSoundVolumesByType[(GlobalConstants.BulletType)_currentWeapon];
      string soundName = GlobalConstants.BulletSoundByType[(GlobalConstants.BulletType)_currentWeapon];
      bullet.Propel(_direction, GlobalConstants.BulletSpeedByType[(GlobalConstants.BulletType)_currentWeapon]);

      SoundManager.Instance.PlaySound(soundName, volume);
    }

    _cos = Mathf.Sin(_rotation * Mathf.Deg2Rad);
    _sin = Mathf.Cos(_rotation * Mathf.Deg2Rad);

    _direction.x = _sin;
    _direction.y = _cos;

    _acceleration = Input.GetAxis("Vertical") * GlobalConstants.PlayerMoveSpeed;

    ShieldCollider.gameObject.SetActive(Shieldpoints != 0);

    ProcessShield();
  }

  float _shieldRechargeTimer = 0.0f;
  Color _shieldColor = Color.white;
  void ProcessShield()
  {
    if (Shieldpoints < _maxPoints)
    {
      _shieldRechargeTimer += Time.smoothDeltaTime;
    
      if (_shieldRechargeTimer > GlobalConstants.ShieldRechargeTimeout)
      {
        _shieldRechargeTimer = 0.0f;
        ReceiveShieldDamage(-1);
      }
    }

    if (_shieldColor.a > 0.0f)
    {
      _shieldColor.a -= Time.smoothDeltaTime;
    }

    _shieldColor.a = Mathf.Clamp(_shieldColor.a, 0.0f, 1.0f);

    ShieldSprite.color = _shieldColor;
  }

  Vector2 _position = Vector2.zero;
  bool _isBeingPushed = false;
  void FixedUpdate()
  { 
    _position = RigidbodyComponent.position;

    _position.x = Mathf.Clamp(_position.x, AppReference.ScreenRect[0], AppReference.ScreenRect[2]);
    _position.y = Mathf.Clamp(_position.y, AppReference.ScreenRect[1], AppReference.ScreenRect[3]);

    RigidbodyComponent.position = _position;
    RigidbodyComponent.rotation = _rotation;

    if ((int)Mathf.Abs(RigidbodyComponent.velocity.x) < 3 && (int)Mathf.Abs(RigidbodyComponent.velocity.y) < 3)
    {
      RigidbodyComponent.velocity = Vector2.zero;
      _isBeingPushed = false;
    }

    if (!_isBeingPushed)
    {
      RigidbodyComponent.MovePosition(RigidbodyComponent.position + _direction * (_acceleration * Time.fixedDeltaTime));
    }
  }

  public void ReceiveShieldDamage(int damageReceived)
  {
    Shieldpoints -= damageReceived;

    Shieldpoints = Mathf.Clamp(Shieldpoints, 0, _maxPoints);
  }

  bool _isDestroying = false;
  public void ReceiveDamage(int damageReceived)
  {
    Hitpoints -= damageReceived;

    Hitpoints = Mathf.Clamp(Hitpoints, 0, _maxPoints);

    if (Hitpoints == 0 && !_isDestroying)
    { 
      _isDestroying = true;

      SoundManager.Instance.StopMusic();
      SoundManager.Instance.PlaySound("ship_explode", 0.5f);
      SoundManager.Instance.PlaySound("gameover");

      AppReference.SetGameOver();

      var go = Instantiate(AppReference.PlayerDeathEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
      Destroy(go, 2.0f);

      Destroy(gameObject);
    }
  }

  public void AddExperience(int experienceToAdd)
  {
    Experience += experienceToAdd;

    if (Experience >= GlobalConstants.ExperienceByLevel[Level])
    {
      AppReference.StarsList[Level].SetActive(true);

      Experience = 0;
      Level++;

      if (Level == GlobalConstants.ExperienceByLevel.Count)
      {
        AppReference.StatsDoubleIcon.gameObject.SetActive(true);

        _maxPoints *= 2;

        Hitpoints = _maxPoints;
        Shieldpoints = _maxPoints;
      }

      _currentWeapon++;

      SoundManager.Instance.PlaySound("weapon_upgrade", 0.25f);

      _currentWeapon = Mathf.Clamp(_currentWeapon, 0, GlobalConstants.BulletSpeedByType.Count - 1);

      AppReference.SetWeapon(_currentWeapon);
    }
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (AppReference.IsGameOver)
    {
      return;
    }

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int playerLayer = LayerMask.NameToLayer("Player");

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
    else if (other.gameObject.layer == playerLayer)
    {
      UFO saucer = other.gameObject.GetComponentInParent<UFO>();
      if (saucer != null)
      {
        ProcessDamage(1);
      }
    }
  }

  public void ProcessDamage(int damage)
  {
    if (Shieldpoints != 0)
    {                       
      SoundManager.Instance.PlaySound("shield_hit_energy", 0.1f, 1.0f, false);

      _shieldColor.a = 1.0f;
      ReceiveShieldDamage(1);
    }
    else
    {
      SoundManager.Instance.PlaySound("ship_hit", 0.25f, 1.0f, false);
      ReceiveDamage(damage);
    }
  }
}
