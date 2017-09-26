using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour 
{ 
  public Transform AsteroidsHolder;

  public List<Sprite> BulletsIcons;

  public Image WeaponIcon;
  public Text ExperienceText;
  public Text HitpointsBar;
  public Text ShieldpointsBar;
  public Text SpawnRate;
  public Text AsteroidsCount;
  public Text PhaseCount;
  public Text SpawnProgressBar;

  public GameObject AsteroidPrefab;
  public GameObject AsteroidSpawnEffect;
  public GameObject PowerupSpawnEffect;

  public Player PlayerScript;

  Vector2 _dimensions = Vector2.zero;

  StringBuilder _progressBarSb = new StringBuilder();

  float[] _screenRect;
  public float[] ScreenRect
  {
    get { return _screenRect; }
  }

  int _currentPhase = 0;
  int _spawnedAsteroids = 0;

  float _currentSpawnRate = 1.0f;
  float _spawnScale = 0.1f;

  List<Vector2> _spawnPoints = new List<Vector2>();

  float _progressBarTimeCounter = 0.0f;
  float _progressBarCurrentTimeMax = 0.0f;
  void Awake()
  {    
    _dimensions = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.nearClipPlane));

    //Debug.Log(_dimensions);

    _screenRect = new float[4];

    _screenRect[0] = _dimensions.x;
    _screenRect[1] = _dimensions.y;
    _screenRect[2] = -_dimensions.x;
    _screenRect[3] = -_dimensions.y - 1.0f;

    _currentSpawnRate = GlobalConstants.StartingSpawnRate;

    // 20 is the maximum number of '=' characters in the bar (hardcoded)
    _progressBarCurrentTimeMax = _currentSpawnRate / 20.0f;

    _spawnPoints.Add(new Vector2(_dimensions.x + 0.5f, _dimensions.y + 0.5f));
    _spawnPoints.Add(new Vector2(-_dimensions.x - 0.5f, _dimensions.y + 0.5f));
    _spawnPoints.Add(new Vector2(_dimensions.x + 0.5f, -_dimensions.y - 1.5f));
    _spawnPoints.Add(new Vector2(-_dimensions.x - 0.5f, -_dimensions.y - 1.5f));

    /*
    foreach (var item in _spawnPoints)
    {
      Debug.Log(item);
    }
    */
  }

  bool _isGameOver = false;

  float _spawnTimer = 0.0f;
  void Update()
  {
    if (_isGameOver) return;

    ExperienceText.text = string.Format("{0} / {1}", PlayerScript.Experience, GlobalConstants.ExperienceByLevel[PlayerScript.Level]);
    AsteroidsCount.text = string.Format("S: {0} / {1}", _spawnedAsteroids, GlobalConstants.AsteroidsMaxInstances);
    PhaseCount.text = string.Format("PHASE {0}", _currentPhase);
    SpawnRate.text = string.Format("R: {0:N2}", GlobalConstants.StartingSpawnRate / _currentSpawnRate);

    if (_spawnedAsteroids <= GlobalConstants.AsteroidsMaxInstances)
    {
      _spawnTimer += Time.smoothDeltaTime;
      _progressBarTimeCounter += Time.smoothDeltaTime;

      if (_progressBarTimeCounter > _progressBarCurrentTimeMax)
      {
        _progressBarTimeCounter = 0.0f;
        _progressBarSb.Append('=');
      }
    }
    else
    {
      _spawnTimer = 0.0f;
    }

    if (_spawnTimer > _currentSpawnRate)
    {
      _spawnTimer = 0.0f;

      _spawnedAsteroids++;
      _spawnScale += 0.1f;
      _currentPhase++;

      _spawnScale = Mathf.Clamp(_spawnScale, 0.1f, 1.0f);

      _currentSpawnRate -= GlobalConstants.SpawnRateDelta * _spawnScale;

      _currentSpawnRate = Mathf.Clamp(_currentSpawnRate, GlobalConstants.MaxSpawnRate, GlobalConstants.StartingSpawnRate);

      _progressBarCurrentTimeMax = _currentSpawnRate / 20.0f;
      _progressBarTimeCounter = 0.0f;

      _progressBarSb.Length = 0;

      SpawnAsteroid();
    }

    SpawnProgressBar.text = _progressBarSb.ToString();
  }

  void SpawnAsteroid()
  {
    int index = Random.Range(0, _spawnPoints.Count);

    GameObject ase = Instantiate(AsteroidSpawnEffect, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    Destroy(ase.gameObject, 1.0f);

    GameObject go = Instantiate(AsteroidPrefab, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    go.transform.parent = AsteroidsHolder;

    go.GetComponent<Asteroid>().Init(_spawnPoints[index]);
  }

  public void SetWeapon(int weaponIndex)
  {
    WeaponIcon.sprite = BulletsIcons[weaponIndex];
  }
}
