using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;
  public Collider2D UfoCollider;
  public Collider2D ShieldCollider;
  public SpriteRenderer ShieldSprite;

  public GameObject BulletPrefab;

  [HideInInspector]
  public int Shieldpoints = 20;

  [HideInInspector]
  public int Hitpoints = 20;

  int _maxPoints = 20;

  float _shieldRechargeTimer = 0.0f;
  Color _shieldColor = Color.white;

  Vector2 _direction = Vector2.zero;

  GameScript _app;
  float[] _screenRect;
  void Awake()
  {
    _app = GameObject.Find("App").GetComponent<GameScript>();
    _screenRect = _app.ScreenRect;

    _shieldColor.a = 0.0f;

    Physics2D.IgnoreCollision(ShieldCollider, UfoCollider);

    _timeout = Random.Range(_minTimeout, _maxTimeout);
    _saucerSpeed = Random.Range(_saucerMinSpeed, _saucerMaxSpeed);
    _direction = GlobalConstants.GetRandomDir();
  }

  float _minTimeout = 1.0f;
  float _maxTimeout = 2.0f;
  float _timeout = 0.0f;
  float _timer = 0.0f;
  void Update()
  {
    ShieldCollider.gameObject.SetActive(Shieldpoints != 0);

    ProcessShield();

    _timer += Time.smoothDeltaTime;

    if (_timer > _timeout)
    {
      _timer = 0.0f;
      _timeout = Random.Range(_minTimeout, _maxTimeout);
      _direction = GlobalConstants.GetRandomDir();
    }
  }

  Vector2 _position = Vector2.zero;
  float _saucerMinSpeed = 100.0f;
  float _saucerMaxSpeed = 200.0f;
  float _saucerSpeed = 0.0f;
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

  public void ReceiveShieldDamage(int damageReceived)
  {
    Shieldpoints -= damageReceived;

    Shieldpoints = Mathf.Clamp(Shieldpoints, 0, _maxPoints);
  }

  public void ReceiveDamage(int damageReceived)
  {
    Hitpoints -= damageReceived;

    Hitpoints = Mathf.Clamp(Hitpoints, 0, _maxPoints);

    if (Hitpoints == 0)
    { 
      SoundManager.Instance.PlaySound("ship_explode", 0.25f);

      var go = Instantiate(_app.PlayerDeathEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
      Destroy(go, 2.0f);

      Destroy(gameObject);
    }
  }

  void OnTriggerEnter2D(Collider2D other)
  {
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
      Player p = other.gameObject.GetComponentInParent<Player>();
      if (p != null)
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
      ReceiveDamage(damage);

      if (Hitpoints != 0)
      {
        SoundManager.Instance.PlaySound("ship_hit", 0.25f, 1.0f, false);
      }
    }
  }
}
