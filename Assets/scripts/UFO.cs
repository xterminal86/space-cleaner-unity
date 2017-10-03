using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour 
{
  GameScript _app;
  void Awake()
  {
    _app = GameObject.Find("App").GetComponent<GameScript>();
  }
}
