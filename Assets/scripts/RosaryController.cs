using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosaryController : MonoBehaviour 
{
  public SpriteRenderer AreaSprite;

  float _maxRadius = 10.0f;

  Color _areaColor = Color.white;
  bool _isActive = false;
  public void Execute()
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
        a.Deactivate();
      }
    }
  }
}
