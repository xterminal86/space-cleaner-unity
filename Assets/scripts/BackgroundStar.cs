using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundStar : MonoBehaviour 
{
  public SpriteRenderer StarSprite;

  bool _isActive = false;
  public bool IsActive
  {
    get { return _isActive; }
  }

  Color _starColor = Color.white;

  float[] _screenDimensions;

  float _alpha = 0.0f, _alphaSpeed = 1.0f;
  public void Init(float[] screenDimensions)
  {
    _screenDimensions = screenDimensions;

    _isActive = true;

    Reset();
  }

  public void Reset()
  {
    _isGrowing = true;

    float scale = Random.Range(0.05f, 0.25f);
    float alphaSpeed = Random.Range(0.1f, 1.0f);

    _alphaSpeed = alphaSpeed;

    _scale.Set(scale, scale, scale);
    transform.localScale = _scale;

    _alpha = 0.0f;
    _starColor.a = _alpha;

    SetRandomPosition();
  }

  Vector3 _position = Vector3.zero;
  public void SetRandomPosition()
  {
    float x = Random.Range(_screenDimensions[0], _screenDimensions[2]);
    float y = Random.Range(_screenDimensions[1], _screenDimensions[3]);

    _position.Set(x, y, 0.0f);

    transform.localPosition = _position;
  }

  Vector3 _scale = Vector3.zero;
  bool _isGrowing = true;
  void Update()
  {
    if (!_isActive)
    {
      return;
    }

    //Debug.Log(transform.localPosition + " " + StarSprite.color + " " + _starColor);

    if (_isGrowing)
    {      
      _alpha += _alphaSpeed * Time.smoothDeltaTime;
    }
    else
    {      
      _alpha -= _alphaSpeed * Time.smoothDeltaTime;
    }

    if (_alpha > 1.0f)
    {
      _isGrowing = false;
    }
    else if (_alpha < 0.0f)
    {
      Reset();
    }

    _starColor.a = _alpha;

    StarSprite.color = _starColor;

    transform.localScale = _scale;
  }
}
