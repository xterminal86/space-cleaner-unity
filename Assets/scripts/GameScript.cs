using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour 
{
  Vector2 _dimensions = Vector2.zero;

  float[] _screenRect;
  public float[] ScreenRect
  {
    get { return _screenRect; }
  }

  float _aspect = 1.0f;
  void Awake()
  {    
    _dimensions = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.nearClipPlane));

    Debug.Log(_dimensions);

    _screenRect = new float[4];

    _screenRect[0] = _dimensions.x;
    _screenRect[1] = _dimensions.y;
    _screenRect[2] = -_dimensions.x;
    _screenRect[3] = -_dimensions.y;
  }
}
