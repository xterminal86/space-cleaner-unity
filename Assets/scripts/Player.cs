using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour 
{
  public GameScript AppReference;

  public Rigidbody2D RigidbodyComponent;
  public Collider2D PlayerCollider;
  public Transform ShotPoint;

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

  StringBuilder _hpBar = new StringBuilder();
  StringBuilder _shieldBar = new StringBuilder();
  void Awake()
  {
    Hitpoints = _maxPoints;
    Shieldpoints = _maxPoints;

    _hpBar.Length = 0;
    _shieldBar.Length = 0;

    _hpBar.Append('>', Hitpoints);
    _shieldBar.Append('>', Shieldpoints);
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

    if (Input.GetKeyDown(KeyCode.Space))
    {
      GameObject go = Instantiate(Bullets[_currentWeapon], new Vector3(ShotPoint.position.x, ShotPoint.position.y, 0.0f), Quaternion.identity);
      var bullet = go.GetComponent<BulletBase>();
      //Physics2D.IgnoreCollision(PlayerCollider, bullet.Collider);
      bullet.Propel(_direction, GlobalConstants.BulletSpeedByType[(GlobalConstants.BulletType)_currentWeapon]);
    }

    _hpBar.Length = 0;
    _hpBar.Append('>', Hitpoints);

    _shieldBar.Length = 0;
    _shieldBar.Append('>', Shieldpoints);

    AppReference.HitpointsBar.text = _hpBar.ToString();
    AppReference.ShieldpointsBar.text = _shieldBar.ToString();

    _cos = Mathf.Sin(_rotation * Mathf.Deg2Rad);
    _sin = Mathf.Cos(_rotation * Mathf.Deg2Rad);

    _direction.x = _sin;
    _direction.y = _cos;

    _acceleration = Input.GetAxis("Vertical") * GlobalConstants.PlayerMoveSpeed;
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

  public void ReceiveShieldDamage()
  {
    Shieldpoints--;

    Shieldpoints = Mathf.Clamp(Shieldpoints, 0, _maxPoints);
  }

  public void ReceiveDamage(int damageReceived)
  {
    Hitpoints -= damageReceived;

    Hitpoints = Mathf.Clamp(Hitpoints, 0, _maxPoints);

    if (Hitpoints == 0)
    {
      // Game Over
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
