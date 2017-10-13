using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosaryController : MonoBehaviour 
{
  public SpriteRenderer AreaSprite;
  public Collider2D AreaCollider;

  float _maxRadius = 10.0f;

  Color _areaColor = Color.white;
  bool _isActive = false;

  [HideInInspector]
  public bool WasSpawned = false;

  public void Execute(Vector2 pos)
  {
    if (_isActive)
    {
      return;
    }

    _areaColor = Color.white;
    _scale.Set(0.0f, 0.0f, 0.0f);
    AreaSprite.transform.localScale = _scale;
    AreaSprite.color = _areaColor;
    _isActive = true;
    AreaSprite.transform.localPosition = pos;
    AreaSprite.gameObject.SetActive(true);
  }

  float _areaSpeed = 10.0f;
  Vector3 _scale = Vector3.zero;
  public void Update()
  {
    if (!_isActive)
    {
      return;
    }

    _scale.x += Time.smoothDeltaTime * _areaSpeed;
    _scale.y += Time.smoothDeltaTime * _areaSpeed;
    _scale.z += Time.smoothDeltaTime * _areaSpeed;

    _areaColor.a = 1.0f - _scale.x / _maxRadius;
    _areaColor.a = Mathf.Clamp(_areaColor.a, 0.0f, 1.0f);

    AreaSprite.transform.localScale = _scale;
    AreaSprite.color = _areaColor;

    if (AreaSprite.transform.localScale.x > _maxRadius)
    {
      AreaSprite.gameObject.SetActive(false);
      _isActive = false;
      WasSpawned = false;
    }
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    int asteroidsLayer = LayerMask.NameToLayer("Asteroids");
    int enemyLayer = LayerMask.NameToLayer("Enemy");
    int bulletsLayer = LayerMask.NameToLayer("Bullets");

    if (other.gameObject.layer == asteroidsLayer)
    {
      var a = other.gameObject.GetComponentInParent<Asteroid>();
      if (a != null)
      {
        a.ForceDestroy();
      }
    }
    else if (other.gameObject.layer == enemyLayer)
    {
      var u = other.gameObject.GetComponentInParent<UFO>();
      if (u != null)
      {
        u.ForceDestroy();
      }
    }
    else if (other.gameObject.layer == bulletsLayer)
    {
      var b = other.gameObject.GetComponentInParent<BulletBase>();
      if (b != null)
      {
        b.ForceDestroy();
      }
    }
  }
}
