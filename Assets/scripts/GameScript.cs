using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour 
{ 
  public Transform AsteroidsHolder;
  public GameObject GameOverScreen;

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
  public GameObject AsteroidBreakdownEffect;
  public GameObject PlayerDeathEffect;
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

  List<Vector2> _spawnPoints = new List<Vector2>();

  StringBuilder _hpBar = new StringBuilder();
  StringBuilder _shieldBar = new StringBuilder();

  float _progressBarUpdateInterval = 0.0f;
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
    _progressBarUpdateInterval = _currentSpawnRate / 20.0f;

    _spawnPoints.Add(new Vector2(_dimensions.x + 0.5f, _dimensions.y + 0.5f));
    _spawnPoints.Add(new Vector2(-_dimensions.x - 0.5f, _dimensions.y + 0.5f));
    _spawnPoints.Add(new Vector2(_dimensions.x + 0.5f, -_dimensions.y - 1.5f));
    _spawnPoints.Add(new Vector2(-_dimensions.x - 0.5f, -_dimensions.y - 1.5f));

    _progressBarTimer = Time.realtimeSinceStartup;

    _hpBar.Length = 0;
    _shieldBar.Length = 0;

    _hpBar.Append('>', PlayerScript.Hitpoints);
    _shieldBar.Append('>', PlayerScript.Shieldpoints);

    /*
    foreach (var item in _spawnPoints)
    {
      Debug.Log(item);
    }
    */
  }

  public void SetGameOver()
  {
    IsGameOver = true;

    GameOverScreen.SetActive(true);

    HitpointsBar.text = "";
    ShieldpointsBar.text = "";
  }

  [HideInInspector]
  public bool IsGameOver = false;

  float _progressBarTimer = 0.0f;
  float _spawnAcceleration = 1.0f;
  int _barCounter = 0;
  void Update()
  {
    if (IsGameOver) return;

    _hpBar.Length = 0;
    _hpBar.Append('>', PlayerScript.Hitpoints);

    _shieldBar.Length = 0;
    _shieldBar.Append('>', PlayerScript.Shieldpoints);

    HitpointsBar.text = _hpBar.ToString();
    ShieldpointsBar.text = _shieldBar.ToString();

    ExperienceText.text = string.Format("{0} / {1}", PlayerScript.Experience, GlobalConstants.ExperienceByLevel[PlayerScript.Level]);
    AsteroidsCount.text = string.Format("S: {0} / {1}", _spawnedAsteroids, GlobalConstants.AsteroidsMaxInstances);
    PhaseCount.text = string.Format("PHASE {0}", _currentPhase);
    SpawnRate.text = string.Format("R: {0:N2}", _currentSpawnRate / GlobalConstants.MaxSpawnRate);

    if (_spawnedAsteroids < GlobalConstants.AsteroidsMaxInstances)
    {      
      float timeNow = Time.realtimeSinceStartup;
      if (timeNow > _progressBarTimer + _progressBarUpdateInterval)
      {        
        _progressBarTimer = timeNow;
        _progressBarSb.Append('=');
        _barCounter++;
      }

      if (_barCounter > 20)
      {
        _barCounter = 0;

        _spawnedAsteroids++;
        _currentPhase++;

        _currentSpawnRate -= GlobalConstants.SpawnRateDelta * _spawnAcceleration;

        _spawnAcceleration += 0.2f;

        _spawnAcceleration = Mathf.Clamp(_spawnAcceleration, 1.0f, 2.0f);

        _currentSpawnRate = Mathf.Clamp(_currentSpawnRate, GlobalConstants.MaxSpawnRate, GlobalConstants.StartingSpawnRate);

        _progressBarUpdateInterval = _currentSpawnRate / 20.0f;

        _progressBarSb.Length = 0;

        SpawnAsteroid();
      }

      SpawnProgressBar.text = _progressBarSb.ToString();
    }
  }

  void SpawnAsteroid()
  {
    int index = Random.Range(0, _spawnPoints.Count);

    GameObject ase = Instantiate(AsteroidSpawnEffect, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    Destroy(ase.gameObject, 1.0f);

    GameObject go = Instantiate(AsteroidPrefab, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    go.transform.parent = AsteroidsHolder;

    var asteroid = go.GetComponent<Asteroid>();
    asteroid.Init(_spawnPoints[index], 0);
    asteroid.PushRandom();
  }

  public void SetWeapon(int weaponIndex)
  {
    WeaponIcon.sprite = BulletsIcons[weaponIndex];
  }
}
