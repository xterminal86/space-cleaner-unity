using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour 
{
  [HideInInspector]
  public bool IsActive = false;

  GameScript _appReference;

  List<Asteroid> _totalAsteroidInstances = new List<Asteroid>();

  // We assume that every next breakdown level increases number of debris by one: e.g. one asteroid
  // breaks down in two, its debris are broken in three and so forth.
  // So, to calculate total amount of debris including starting asteroid, we use following recurrent statement:
  //
  // nextNumberOfDebris = currentNumberOfDebris + currentNumberOfDebris * breakdownLevel
  //
  // e.g. for 3 breakdowns
  //
  // currentNumberOfDebris = 1
  //
  // nextNumberOfDebris = 1 + 1 * 0 = 1; (0)
  // nextNumberOfDebris = 1 + 1 * 1 = 2; (1)
  // nextNumberOfDebris = 2 + 2 * 2 = 6; (2)
  // nextNumberOfDebris = 6 + 6 * 3 = 24; (3)

  int _totalAsteroids = 1;
  void Awake()
  {
    _appReference = GameObject.Find("App").GetComponent<GameScript>();

    _totalAsteroids = 1;

    for (int i = 0; i <= GlobalConstants.AsteroidMaxBreakdownLevel; i++)
    {
      _totalAsteroids += i * _totalAsteroids;
    }

    //Debug.Log(GlobalConstants.AsteroidMaxBreakdownLevel + " => " + _totalAsteroids);

    for (int i = 0; i < _totalAsteroids; i++)
    {
      GameObject go = Instantiate(_appReference.AsteroidPrefab);
      Asteroid a = go.GetComponent<Asteroid>();
      a.ControllerRef = this;
      _totalAsteroidInstances.Add(a);
      go.transform.parent = transform;
    }
  }

  void Update()
  {
    if (!IsActive)
    {
      return;
    }

    bool shouldDeactivate = true;
    foreach (var item in _totalAsteroidInstances)
    {
      if (item.IsActive)
      {
        shouldDeactivate = false;
      }
    }

    if (shouldDeactivate)
    {
      _appReference.SpawnedAsteroids--;

      if (_appReference.SpawnedAsteroids < 0)
      {
        _appReference.SpawnedAsteroids = 0;
      }

      IsActive = false;
    }
  }

  public void Init(Vector2 position)
  {
    IsActive = true;

    foreach (var item in _totalAsteroidInstances)
    {
      if (!item.IsActive)
      {
        item.Init(position, 1, GlobalConstants.GetRandomDir());
        break;
      }
    }
  }

  public void ProcessBreakdown(Vector2 position, int breakdownLevel, Vector2 pushDir)
  {
    for (int i = 0; i < breakdownLevel; i++)
    {
      foreach (var item in _totalAsteroidInstances)
      {
        if (!item.IsActive)
        {
          item.Init(position, breakdownLevel, pushDir);
          break;
        }
      }
    }
  }
}
