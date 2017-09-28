using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour 
{
  public GameObject HitAnimationPrefab;
  public Collider2D Collider;

  protected Rigidbody2D _rigidbodyComponent;
  public Rigidbody2D RigidbodyComponent
  {
    get { return _rigidbodyComponent; }
  }

  protected GameScript _app;

  protected bool _isColliding = false;

  void Awake()
  {
    _rigidbodyComponent = GetComponentInChildren<Rigidbody2D>();
    _app = GameObject.Find("App").GetComponent<GameScript>();
  }

  protected Vector2 _direction = Vector2.zero;
  float _bulletSpeed = 0.0f;
  public virtual void Propel(Vector2 direction, float bulletSpeed)
  {
    _direction = direction;
    _bulletSpeed = bulletSpeed;
  }

  float _offset = 0.5f;
  void FixedUpdate()
  {
    if (_rigidbodyComponent.position.x > _app.ScreenRect[2] + _offset || _rigidbodyComponent.position.x < _app.ScreenRect[0] - _offset 
     || _rigidbodyComponent.position.y > _app.ScreenRect[3] + _offset || _rigidbodyComponent.position.y < _app.ScreenRect[1] - _offset) 
    {
      Destroy(gameObject);
      return;
    }

    _rigidbodyComponent.MovePosition(_rigidbodyComponent.position + _direction * (_bulletSpeed * Time.fixedDeltaTime));      
  }
}
