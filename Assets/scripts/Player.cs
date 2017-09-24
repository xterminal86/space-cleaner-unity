using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
  public Rigidbody2D RigidbodyComponent;

  float _rotation = 0.0f;
  float _acceleration = 0.0f;
  float _cos = 0.0f, _sin = 0.0f;
  Vector2 _direction = Vector2.zero;
  void Update()
  {
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      _rotation += GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    } 
    else if (Input.GetKey(KeyCode.RightArrow))
    {
      _rotation -= GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    }

    _cos = Mathf.Sin(_rotation * Mathf.Deg2Rad);
    _sin = Mathf.Cos(_rotation * Mathf.Deg2Rad);

    _direction.x = _sin;
    _direction.y = _cos;

    _acceleration = Input.GetAxis("Vertical") * GlobalConstants.PlayerMoveSpeed;
  }

  bool _isBeingPushed = false;
  void FixedUpdate()
  {    
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

  void OnCollisionEnter2D(Collision2D collision)
  {    
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    //other.GetComponentInParent<Rigidbody2D>().AddForce(_direction * _acceleration, ForceMode2D.Impulse);
  }
}
