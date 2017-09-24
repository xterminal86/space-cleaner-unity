using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour 
{
  void Awake()
  {
    SoundManager.Instance.Initialize();
  }
}
