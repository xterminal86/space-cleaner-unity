using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;
  public Vector3 OriginalScale;

  GameScript _game;
  TitleScreen _title;

  [HideInInspector]
  public AsteroidController ControllerRef;

  bool _isColliding = false;

  [HideInInspector]
  public bool IsActive = false;
    
  float[] _screenRect;

  int _hitpoints = 0;

  void Awake()
  {
    var go = GameObject.Find("App");

    if (go != null)
    {
      _game = go.GetComponent<GameScript>();
      _screenRect = _game.ScreenRect;
    }
    else
    {
      go = GameObject.Find("Title");
      _title = go.GetComponent<TitleScreen>();
      _screenRect = _title.ScreenRect;
    }
  }

  // Starts from 1
  int _breakdownLevel = 0;
  public int BreakdownLevel
  {
    get { return _breakdownLevel; }
  }

  float _asteroidSpeed = 1.0f;
  public void Init(Vector2 position, int breakdownLevel)
  {
    IsActive = true;
    _breakdownLevel = breakdownLevel;
    _hitpoints = GlobalConstants.AsteroidHitpointsByBreakdownLevel[_breakdownLevel];
    _position = position;
    _rotationSpeed = Random.Range(GlobalConstants.AsteroidMinRotationSpeed, GlobalConstants.AsteroidMaxRotationSpeed);
    _rotationDirection = (Random.Range(0, 2) == 0) ? 1 : -1;
    _asteroidSpeed = Random.Range(GlobalConstants.AsteroidMinSpeed, GlobalConstants.AsteroidMaxSpeed);
    float divider = Mathf.Pow(2, _breakdownLevel);
    Vector3 scale = new Vector3(OriginalScale.x / divider, OriginalScale.y / divider, OriginalScale.z / divider);
    gameObject.transform.localScale = scale;
    gameObject.transform.localPosition = new Vector3(position.x, position.y, 0.0f);
    gameObject.SetActive(true);
    PushRandom();
  }

  void PushRandom()
  {
    float dirX = Random.Range(-1.0f, 1.0f);
    float dirY = Random.Range(-1.0f, 1.0f);
    _direction.Set(dirX, dirY);
    _direction.Normalize();
    RigidbodyComponent.AddForce(_direction * _asteroidSpeed, ForceMode2D.Impulse);
  }

  public void ReceiveDamage(int damage)
  {
    _hitpoints -= damage;

    if (_hitpoints <= 0)
    {
      float pitch = 0.5f * _breakdownLevel;
      float volume = 0.75f / _breakdownLevel;

      SoundManager.Instance.PlaySound("asteroid_hit_big", volume, pitch);

      if (_game.PlayerScript.Level != GlobalConstants.ExperienceByLevel.Count)
      {
        _game.PlayerScript.AddExperience(_breakdownLevel);
      }

      HandleCollision();
    }
  }

  public void HandleCollision()
  {
    _breakdownLevel++;

    GameObject effect = Instantiate(_game.AsteroidBreakdownEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
    effect.transform.localScale = transform.localScale;
    Destroy(effect.gameObject, 2.0f);

    gameObject.SetActive(false);

    IsActive = false;

    if (_breakdownLevel <= GlobalConstants.AsteroidMaxBreakdownLevel)
    {
      ControllerRef.ProcessBreakdown(RigidbodyComponent.position, _breakdownLevel);
    }
  }

  public void Push(Rigidbody2D pusher)
  {
    Vector2 v = RigidbodyComponent.position - pusher.position;
    v.Normalize();
    _direction.Set(v.x, v.y);
    RigidbodyComponent.AddForce(_direction, ForceMode2D.Impulse);
  }

  Vector2 _position = Vector2.zero;
  Vector2 _direction = Vector2.zero;
  Vector2 _velocity = Vector2.zero;
  float _rotation = 0.0f;
  float _rotationSpeed = 1.0f;
  int _rotationDirection = 1;
  float _offset = 0.5f;
  void FixedUpdate()
  {
    if (!IsActive) return;

    /*
    _velocity.Set(RigidbodyComponent.velocity.x, RigidbodyComponent.velocity.y);

    float vx = Mathf.Abs(_velocity.x);
    float vy = Mathf.Abs(_velocity.y);

    int sx = (int)Mathf.Sign(_velocity.x);
    int sy = (int)Mathf.Sign(_velocity.y);

    _velocity.x = sx * Mathf.Clamp(vx, GlobalConstants.AsteroidMinSpeed, GlobalConstants.AsteroidMaxSpeed);
    _velocity.y = sy * Mathf.Clamp(vy, GlobalConstants.AsteroidMinSpeed, GlobalConstants.AsteroidMaxSpeed);

    RigidbodyComponent.velocity = _velocity;
    */

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

    _rotation += _rotationDirection * (_rotationSpeed * Time.fixedDeltaTime);
    RigidbodyComponent.rotation = _rotation;
  }

  void OnTriggerEnter2D(Collider2D collider)
  {    
    //if (_isColliding) return;

    //_isColliding = true;

    //Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.yellow, 10.0f);

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");

    if (collider.gameObject.layer == asteroidsLayer)
    { 
      var c = collider.gameObject.GetComponentInParent<Asteroid>();

      // GetComponent returns null if object is inactive
      if (c != null)
      {
        c.Push(RigidbodyComponent);
      }
    }
  }
}
