using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;

  GameScript _game;

  void Awake()
  {
    _game = GameObject.Find("App").GetComponent<GameScript>();

    _direction.x = -0.5f;
    _direction.y = 0.25f;

    _direction.Normalize();
  }

  Vector2 _position = Vector2.zero;
  Vector2 _direction = Vector2.zero;
  float _offset = 0.5f;
  void FixedUpdate()
  {
    _position = RigidbodyComponent.position;

    if (RigidbodyComponent.position.x < _game.ScreenRect[0] - _offset)
    {
      _position.x = _game.ScreenRect[2] + _offset;
    }
    else if (RigidbodyComponent.position.x > _game.ScreenRect[2] + _offset)
    {
      _position.x = _game.ScreenRect[0] - _offset;
    }
    else if (RigidbodyComponent.position.y < _game.ScreenRect[1] - _offset)
    {
      _position.y = _game.ScreenRect[3] + _offset;
    }
    else if (RigidbodyComponent.position.y > _game.ScreenRect[3] + _offset)
    {
      _position.y = _game.ScreenRect[1] - _offset;
    }

    RigidbodyComponent.position = _position;
    RigidbodyComponent.MovePosition(RigidbodyComponent.position + _direction * (1.0f * Time.fixedDeltaTime));
  }
}
