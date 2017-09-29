using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoSingleton<ScreenshotTaker> 
{
  DateTime _dt = new DateTime();
  string _filename = string.Empty;
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F9))
    {
      SoundManager.Instance.PlaySound("screenshot", 1.0f, 1.0f, false);

      _dt = DateTime.Now;
      _filename = string.Format("{0}-{1}-{2}-{3}{4}{5}.png", _dt.Day, _dt.Month, _dt.Year, _dt.Hour, _dt.Minute, _dt.Second);
      Application.CaptureScreenshot(_filename);
    }
  }
}
