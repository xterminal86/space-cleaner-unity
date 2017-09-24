using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour 
{
  public GameObject BackgroundStarsHolder;
  public GameObject BackgroundStarPrefab;

  public int BackgroundStars = 100;

  List<BackgroundStar> _stars = new List<BackgroundStar>();
  void Awake()
  {
    SoundManager.Instance.Initialize();

    _stars.Clear();

    for (int i = 0; i < BackgroundStars; i++)
    {      
      GameObject go = Instantiate(BackgroundStarPrefab, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.identity, BackgroundStarsHolder.transform);
      BackgroundStar bs = go.GetComponent<BackgroundStar>();
      bs.Init();
      _stars.Add(bs);
    }
  }
}
