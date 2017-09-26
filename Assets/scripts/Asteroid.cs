using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;

  GameScript _game;

  bool _isColliding = false;

  void Awake()
  {
    _game = GameObject.Find("App").GetComponent<GameScript>();
  }

  float _asteroidSpeed = 1.0f;
  public void Init(Vector2 position)
  {
    _position = position;
    _rotationSpeed = Random.Range(GlobalConstants.AsteroidMinRotationSpeed, GlobalConstants.AsteroidMaxRotationSpeed);
    _rotationDirection = (Random.Range(0, 2) == 0) ? 1 : -1;
    _asteroidSpeed = Random.Range(GlobalConstants.AsteroidMinSpeed, GlobalConstants.AsteroidMaxSpeed);
    float dirX = Random.Range(-1.0f, 1.0f);
    float dirY = Random.Range(-1.0f, 1.0f);
    _direction.Set(dirX, dirY);
    _direction.Normalize();
    RigidbodyComponent.AddForce(_direction, ForceMode2D.Impulse);
    gameObject.SetActive(true);
  }

  public void Push(Rigidbody2D pusher)
  {
    Vector2 vel = RigidbodyComponent.velocity - pusher.velocity;
    vel.Normalize();
    _direction.Set(vel.x, vel.y);
    RigidbodyComponent.AddForce(_direction, ForceMode2D.Impulse);
  }

  Vector2 _position = Vector2.zero;
  Vector2 _direction = Vector2.zero;
  float _rotation = 0.0f;
  float _rotationSpeed = 1.0f;
  int _rotationDirection = 1;
  float _offset = 0.5f;
  void FixedUpdate()
  {
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

    _rotation += (_rotationSpeed * _rotationDirection) * Time.fixedDeltaTime;
    RigidbodyComponent.MoveRotation(_rotation);
  }

  void OnCollisionEnter2D(Collision2D collision)
  {    
    if (_isColliding) return;

    _isColliding = true;

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int playerLayer = LayerMask.NameToLayer("Player");

    if (collision.gameObject.layer == asteroidsLayer)
    {
      collision.gameObject.GetComponent<Asteroid>().Push(RigidbodyComponent);
    }
  }
}
