using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaser : BulletBase 
{
  void OnTriggerEnter2D(Collider2D collider)
  {
    if (_isColliding) return;

    _isColliding = true;
  }
}
