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

  void Awake()
  {    
    _dimensions = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

    _screenRect = new float[4];

    _screenRect[0] = -_dimensions.x;
    _screenRect[1] = -_dimensions.y;
    _screenRect[2] = _dimensions.x;
    _screenRect[3] = _dimensions.y;
  }
}
