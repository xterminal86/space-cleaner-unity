using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoSingleton<FPSCounter> 
{
  public Text FPSCounterText;

  float _deltaTime = 0.0f;

  void Update()
  {
    _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

    float msec = _deltaTime * 1000.0f;
    float fps = 1.0f / _deltaTime;
    string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    FPSCounterText.text = text;
  }
}
