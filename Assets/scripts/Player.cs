using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour
{
  public GameScript AppReference;
  public RosaryController RosaryControllerScript;
  public ShowerController ShowerControllerScript;

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

    _direction = Vector2.right;
  }

  int _rotationStatus = 0;
  public void SetRotation(int rotationStatus)
  {
    _rotationStatus = rotationStatus;
  }

  int _gasPedal = 0;
  public void SetGas(int gas)
  {
    _gasPedal = gas;
  }

  public void Fire()
  {
    // Edge case when user clicked on the same spot as previous one on joystick
    if (_direction.magnitude == 0.0f)
    {
      _direction = Vector2.up;
    }

    Quaternion q = Quaternion.Euler(0.0f, 0.0f, _rotation);

    GameObject go = Instantiate(Bullets[_currentWeapon], new Vector3(ShotPoint.position.x, ShotPoint.position.y, 0.0f), q);
    var bullet = go.GetComponent<BulletBase>();
    if (_currentWeapon == 2)
    {
      bullet.RigidbodyComponent.rotation = _rotation;
    }
    Physics2D.IgnoreCollision(PlayerCollider, bullet.Collider);
    Physics2D.IgnoreCollision(ShieldCollider, bullet.Collider);
    Physics2D.IgnoreCollision(RosaryControllerScript.AreaCollider, bullet.Collider);
    float volume = GlobalConstants.BulletSoundVolumesByType[(GlobalConstants.BulletType)_currentWeapon];
    string soundName = GlobalConstants.BulletSoundByType[(GlobalConstants.BulletType)_currentWeapon];
    bullet.Propel(_direction, GlobalConstants.BulletSpeedByType[(GlobalConstants.BulletType)_currentWeapon]);

    SoundManager.Instance.PlaySound(soundName, volume);
  }

  Vector2 _newCenterLocation = Vector2.zero;
  Vector2 _oldCenterLocation = Vector2.zero;
  Vector3 _mousePos = Vector3.zero;

  bool _empLocked = false;
  public bool IsEmpLocked
  {
    get { return _empLocked; }
  }

  public void SetEMPLockout()
  {
    if (!_empLocked)
    {
      AppReference.EMPLockoutObject.SetActive(true);
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
      AppReference.EMPLockoutObject.transform.position = pos;
      timer += Time.smoothDeltaTime;
      yield return null;
    }

    _empLocked = false;

    AppReference.EMPLockoutObject.SetActive(false);

    yield return null;
  }

  Vector2 _direction = Vector2.zero;
  void Update()
  {
    if (_empLocked)
    {
      AdjustShieldAlpha();
      return;
    }

    if (_rotationStatus == 1)
    {
      _rotation += GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    }

    if (_rotationStatus == 2)
    {
      _rotation -= GlobalConstants.PlayerRotationSpeed * Time.smoothDeltaTime;
    }

    HandleMobileInput();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

    HandleEditorInput();

  #if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.LeftBracket))
    {
      _currentWeapon--;
    }
    else if (Input.GetKeyDown(KeyCode.RightBracket))
    {
      _currentWeapon++;
    }
  #endif

    _currentWeapon = Mathf.Clamp(_currentWeapon, 0, GlobalConstants.BulletSpeedByType.Count - 1);

    AppReference.SetWeapon(_currentWeapon);
#endif

    CheckGas();

    _acceleration = _gasAmount * GlobalConstants.PlayerMoveSpeed;

    ShieldCollider.gameObject.SetActive(Shieldpoints != 0);

    RechargeShield();
    AdjustShieldAlpha();
  }

  Vector3 _touchPosition = Vector3.zero;
  void HandleMobileInput()
  {
    foreach (var t in Input.touches)
    {
      bool cond = (t.position.x > 0.0f && t.position.x < 200.0f && t.position.y > 0.0f && t.position.y < Screen.height);

      switch (t.phase)
      {
        case TouchPhase.Began:
          if (!cond)
          {
            _touchPosition.Set(t.position.x, t.position.y, Camera.main.nearClipPlane);

            Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(_touchPosition);

            _newCenterLocation.x = worldTouchPosition.x;
            _newCenterLocation.y = worldTouchPosition.y;

            _oldCenterLocation.x = worldTouchPosition.x;
            _oldCenterLocation.y = worldTouchPosition.y;
          }
          break;

        case TouchPhase.Stationary:
          break;

        case TouchPhase.Moved:
          if (!cond)
          {
            _touchPosition.Set(t.position.x, t.position.y, Camera.main.nearClipPlane);
            Vector3 worldTouchPosition = Camera.main.ScreenToWorldPoint(_touchPosition);

            _newCenterLocation.x = worldTouchPosition.x;
            _newCenterLocation.y = worldTouchPosition.y;

            if ((_newCenterLocation - _oldCenterLocation).magnitude > 0.1f)
            {
              _gasPedal = 1;

              _direction = _newCenterLocation - _oldCenterLocation;
              _direction.Normalize();

              float angle = Vector2.Angle(Vector2.right, _direction);
              Vector3 cross = Vector3.Cross(Vector2.right, _direction);

              if (cross.z < 0.0f)
              {
                angle = 360 - angle;
              }

              _rotation = angle;
            }
          }
          break;

        case TouchPhase.Ended:
          if (cond)
          {
            Fire();
          }
          else
          {
            _gasPedal = 0;
          }
          break;
      }
    }
  }

  bool _tapDetected = false;
  void HandleEditorInput()
  {
    _gasAmount = Input.GetAxis("Vertical");

    if (Input.GetKey(KeyCode.A))
    {
      _rotation += 2;
    }
    else if (Input.GetKey(KeyCode.D))
    {
      _rotation -= 2;
    }

    if (Input.GetKeyDown(KeyCode.Space))
    {
      Fire();
    }

    _cos = Mathf.Sin(_rotation * Mathf.Deg2Rad);
    _sin = Mathf.Cos(_rotation * Mathf.Deg2Rad);

    _direction.x = _sin;
    _direction.y = _cos;

    /*
    if (Input.GetMouseButtonDown(0))
    {
      _mousePos.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

      Vector2 wp = Camera.main.ScreenToWorldPoint(_mousePos);

      _newCenterLocation.x = wp.x;
      _newCenterLocation.y = wp.y;

      _oldCenterLocation.x = wp.x;
      _oldCenterLocation.y = wp.y;

      _tapDetected = true;

      //Debug.Log("mouse down");
    }
    else if (Input.GetMouseButtonUp(0))
    {
      if (_tapDetected)
      {
        Fire();
      }
      else
      {
        _mousePos.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

        Vector2 wp = Camera.main.ScreenToWorldPoint(_mousePos);

        _oldCenterLocation.x = _newCenterLocation.x;
        _oldCenterLocation.y = _newCenterLocation.y;

        _gasPedal = 0;
      }

      //Debug.Log("mouse up");
    }
    else if (Input.GetMouseButton(0))
    {
      _mousePos.Set(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
      Vector2 wp = Camera.main.ScreenToWorldPoint(_mousePos);

      _newCenterLocation.x = wp.x;
      _newCenterLocation.y = wp.y;

      if ((_newCenterLocation - _oldCenterLocation).magnitude > 0.5f)
      {
        _gasPedal = 1;

        _tapDetected = false;

        _direction = _newCenterLocation - _oldCenterLocation;
        _direction.Normalize();

        float angle = Vector2.Angle(Vector2.right, _direction);
        Vector3 cross = Vector3.Cross(Vector2.right, _direction);

        if (cross.z < 0.0f)
        {
          angle = 360 - angle;
        }

        _rotation = angle;
      }

      //Debug.Log("mouse hold");
    }
    */
  }

  float _gasAmount = 0.0f;
  void CheckGas()
  {
    if (_gasPedal == 1)
    {
      _gasAmount += Time.smoothDeltaTime * 2;
      _gasAmount = Mathf.Clamp(_gasAmount, 0.0f, 1.0f);
    }
    else
    {
      _gasAmount -= Time.smoothDeltaTime * 2;
      _gasAmount = Mathf.Clamp(_gasAmount, 0.0f, 1.0f);
    }
  }

  float _shieldRechargeTimer = 0.0f;
  Color _shieldColor = Color.white;
  void RechargeShield()
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

  Vector2 _position = Vector2.zero;
  bool _isBeingPushed = false;
  float _offset = 0.5f;
  void FixedUpdate()
  {
    _position = RigidbodyComponent.position;

    if (_position.x < AppReference.ScreenRect[0] - _offset)
    {
      _position.x = AppReference.ScreenRect[2] + _offset;
    }
    else if (_position.x > AppReference.ScreenRect[2] + _offset)
    {
      _position.x = AppReference.ScreenRect[0] - _offset;
    }
    else if (_position.y < AppReference.ScreenRect[1] - _offset)
    {
      _position.y = AppReference.ScreenRect[3] + _offset;
    }
    else if (_position.y > AppReference.ScreenRect[3] + _offset)
    {
      _position.y = AppReference.ScreenRect[1] - _offset;
    }

    //_position.x = Mathf.Clamp(_position.x, AppReference.ScreenRect[0], AppReference.ScreenRect[2]);
    //_position.y = Mathf.Clamp(_position.y, AppReference.ScreenRect[1], AppReference.ScreenRect[3]);

    RigidbodyComponent.position = _position;
    RigidbodyComponent.rotation = _rotation;

    if ((int)Mathf.Abs(RigidbodyComponent.velocity.x) < 3 && (int)Mathf.Abs(RigidbodyComponent.velocity.y) < 3)
    {
      RigidbodyComponent.velocity = Vector2.zero;
      _isBeingPushed = false;
    }

    if (!_isBeingPushed)
    {
      //RigidbodyComponent.velocity = new Vector2(3, 0);
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
      SoundManager.Instance.PlaySound("ship_explode", 0.5f, 1.0f, false);
      SoundManager.Instance.PlaySound("gameover", 1.0f, 1.0f, false);

      AppReference.SetGameOver();

      var go = Instantiate(AppReference.PlayerDeathEffect, new Vector3(RigidbodyComponent.position.x, RigidbodyComponent.position.y, 0.0f), Quaternion.identity);
      Destroy(go, 2.0f);

      Destroy(gameObject);
    }
  }

  public void AddExperience(int experienceToAdd)
  {
    if (Level == GlobalConstants.ExperienceByLevel.Count)
    {
      return;
    }

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

  public void InitiateRosaryPowerup()
  {
    RosaryControllerScript.Execute(RigidbodyComponent.position);
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (AppReference.IsGameOver)
    {
      return;
    }

    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int enemyLayer = LayerMask.NameToLayer("Enemy");

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

      asteroid.HandleBreakdown(newDir);
    }
    else if (other.gameObject.layer == enemyLayer)
    {
      ProcessDamage(1);
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
      if (damage > 0)
      {
        SoundManager.Instance.PlaySound("ship_hit", 0.7f, 1.0f, false);
      }

      ReceiveDamage(damage);
    }
  }
}
