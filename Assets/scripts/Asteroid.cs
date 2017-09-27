using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;
  public Vector3 OriginalScale;

  GameScript _game;

  [HideInInspector]
  public AsteroidController ControllerRef;

  bool _isColliding = false;

  [HideInInspector]
  public bool IsActive = false;
    
  void Awake()
  {
    _game = GameObject.Find("App").GetComponent<GameScript>();
  }

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

  public void Breakdown()
  {
    if (_breakdownLevel < GlobalConstants.AsteroidMaxBreakdownLevel)
    {
      _breakdownLevel++;

      int doubled = _breakdownLevel * 2;
      for (int i = 0; i < doubled; i++)
      {
        GameObject go = Instantiate(_game.AsteroidPrefab, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity, _game.AsteroidsHolder);
        Vector3 scale = new Vector3(transform.localScale.x / doubled, transform.localScale.y / doubled, transform.localScale.z / doubled);
        go.transform.localScale = scale;
        var asteroid = go.GetComponent<Asteroid>();
        asteroid.Init(RigidbodyComponent.position, _breakdownLevel);
        asteroid.RigidbodyComponent.AddForce(-_direction * _asteroidSpeed, ForceMode2D.Impulse);
      }
    }

    GameObject effect = Instantiate(_game.AsteroidBreakdownEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
    effect.transform.localScale = transform.localScale;
    Destroy(effect.gameObject, 2.0f);

    Destroy(gameObject);
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
    Vector2 vel = RigidbodyComponent.velocity + pusher.velocity;
    vel.Normalize();
    _direction.Set(vel.x, vel.y);
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

    if (RigidbodyComponent.position.x < _game.ScreenRect[0] - _offset)
    {
      _position.x = _game.ScreenRect[2] + _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.x > _game.ScreenRect[2] + _offset)
    {
      _position.x = _game.ScreenRect[0] - _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.y < _game.ScreenRect[1] - _offset)
    {
      _position.y = _game.ScreenRect[3] + _offset;
      RigidbodyComponent.position = _position;
    }
    else if (RigidbodyComponent.position.y > _game.ScreenRect[3] + _offset)
    {
      _position.y = _game.ScreenRect[1] - _offset;
      RigidbodyComponent.position = _position;
    }

    _rotation += _rotationDirection * (_rotationSpeed * Time.fixedDeltaTime);
    RigidbodyComponent.rotation = _rotation;
  }

  void OnCollisionEnter2D(Collision2D collision)
  {    
    if (_isColliding) return;

    _isColliding = true;

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");

    if (collision.gameObject.layer == asteroidsLayer)
    {
      collision.gameObject.GetComponent<Asteroid>().Push(RigidbodyComponent);
    }
  }
}
