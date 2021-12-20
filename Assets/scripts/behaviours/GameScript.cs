using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
  public Transform BackgroundStarsHolder;
  public GameObject BackgroundStarPrefab;
  public GameObject HealthPowerupPrefab;
  public GameObject ShieldPowerupPrefab;
  public List<GameObject> SpecialPowerups;

  public Transform AsteroidsHolder;
  public GameObject GameOverScreen;
  public GameObject ReturnToMenuScreen;

  public List<Sprite> BulletsIcons;
  public List<GameObject> StarsList;

  public Image WeaponIcon;
  public Text StatsDoubleIcon;
  public Text ExperienceText;
  public Text HitpointsBar;
  public Text ShieldpointsBar;
  public Text SpawnRate;
  public Text AsteroidsCount;
  public Text PhaseCount;
  public Text ScoreCount;
  public Text SpawnProgressBar;
  public Text MusicPlayingTrackText;

  public GameObject AsteroidPrefab;
  public GameObject AsteroidSpawnEffect;
  public GameObject AsteroidBreakdownEffect;
  public GameObject PlayerDeathEffect;
  public GameObject PowerupSpawnEffect;

  public GameObject AsteroidControllerPrefab;

  public GameObject EMPLockoutObject;

  public Player PlayerScript;
  public UfoController UfoControllerScript;

  Vector2 _dimensions = Vector2.zero;

  StringBuilder _progressBarSb = new StringBuilder();

  float[] _screenRect;
  public float[] ScreenRect
  {
    get { return _screenRect; }
  }

  int _currentPhase = 0;

  [HideInInspector]
  public int Score = 0;

  [HideInInspector]
  public int SpawnedAsteroids = 0;

  float _currentSpawnRate = 1.0f;

  [HideInInspector]
  public int ActiveEmpBullets = 0;

  List<AsteroidController> _asteroidControllers = new List<AsteroidController>();

  List<Vector2> _spawnPoints = new List<Vector2>();

  StringBuilder _hpBar = new StringBuilder();
  StringBuilder _shieldBar = new StringBuilder();

  int _asteroidsOnScreen = 0;
  public int AsteroidsOnScreen
  {
    get
    {
      return _asteroidsOnScreen;
    }

    set
    {
      _asteroidsOnScreen = value;
      _asteroidsOnScreen = Mathf.Clamp(_asteroidsOnScreen, 0, int.MaxValue);
    }
  }

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

    for (int i = 0; i < GlobalConstants.AsteroidsMaxInstances; i++)
    {
      var go = Instantiate(AsteroidControllerPrefab);
      go.transform.parent = AsteroidsHolder;
      AsteroidController ac = go.GetComponent<AsteroidController>();
      _asteroidControllers.Add(ac);
    }

    int starsNumberHorizontal = Screen.width / 180;
    int starsNumberVertical = Screen.height / 180;

    int totalStars = starsNumberHorizontal * starsNumberVertical;

    for (int i = 0; i < totalStars; i++)
    {
      GameObject go = Instantiate(BackgroundStarPrefab, Vector3.zero, Quaternion.identity, BackgroundStarsHolder.transform);
      BackgroundStar bs = go.GetComponentInChildren<BackgroundStar>();
      bs.Init(_screenRect);
    }

    /*
    foreach (var item in _spawnPoints)
    {
      Debug.Log(item);
    }
    */
  }

  public void ReduceSpawnRate()
  {
    _spawnAcceleration -= 0.4f;
    _spawnAcceleration = Mathf.Clamp(_spawnAcceleration, 1.0f, 2.0f);

    _currentSpawnRate += 4.0f;
    _currentSpawnRate = Mathf.Clamp(_currentSpawnRate, GlobalConstants.MaxSpawnRate, GlobalConstants.StartingSpawnRate);

    _progressBarSb.Length = 0;
    _progressBarTimer = Time.realtimeSinceStartup;

    _progressBarUpdateInterval = _currentSpawnRate / 20.0f;

    _barCounter = 0;
  }

  Dictionary<UfoController.UfoVariant, int> _ufosKilled = new Dictionary<UfoController.UfoVariant, int>()
  {
    { UfoController.UfoVariant.LAME,  0 },
    { UfoController.UfoVariant.EMP,   0 },
    { UfoController.UfoVariant.ELITE, 0 }
  };

  public Dictionary<UfoController.UfoVariant, int> UfosKilled
  {
    get { return _ufosKilled; }
  }

  public void SetGameOver()
  {
    IsGameOver = true;

    EMPLockoutObject.SetActive(false);
    GameOverScreen.SetActive(true);
    GameStats.Instance.WriteHighScore(Score, _currentPhase, _ufosKilled);

    HitpointsBar.text = "";
    ShieldpointsBar.text = "";
  }

  public void RestartYesHandler()
  {
    SoundManager.Instance.AudioSourcesByName["gameover"].Stop();
    SoundManager.Instance.AudioSourcesByName["ship_explode"].Stop();

    LoadingScreen.Instance.Show();
    SceneManager.LoadScene("main");
  }

  public void RestartNoHandler()
  {
    SoundManager.Instance.AudioSourcesByName["gameover"].Stop();
    SoundManager.Instance.AudioSourcesByName["ship_explode"].Stop();

    LoadingScreen.Instance.Show();
    SceneManager.LoadScene("title");
  }

  public void ReturnToMenuYesHandler()
  {
    LoadingScreen.Instance.Show();
    SceneManager.LoadScene("title");
  }

  public void ReturnToMenuNoHandler()
  {
    _returnToMainOpen = false;

    ReturnToMenuScreen.gameObject.SetActive(false);
  }

  [HideInInspector]
  public bool IsGameOver = false;

  bool _returnToMainOpen = false;

  float _progressBarTimer = 0.0f;
  float _spawnAcceleration = 1.0f;
  int _barCounter = 0;
  float _powerupLifetimeTimer = 0.0f;

  int _specialPowerupRespawnTimeoutCounter = 0;
  const int _specialPowerupTimeoutPhases = 12;
  bool _specialPowerupWasPicked = false;
  public void SpecialPowerupPicked()
  {
    _specialPowerupWasPicked = true;
    _specialPowerupRespawnTimeoutCounter = 0;
  }

  public void ResetPowerupLock()
  {
    _powerupLifetimeTimer = 0.0f;
    _powerupSpawned = false;
  }

  bool CheckReturnToMenu()
  {
    if (_returnToMainOpen && (Input.GetKeyDown(KeyCode.Return)
                           || Input.GetKeyDown(KeyCode.Y)))
    {
      ReturnToMenuYesHandler();
      return true;
    }
    else if (_returnToMainOpen && (Input.GetKeyDown(KeyCode.Escape)
                                || Input.GetKeyDown(KeyCode.N)))
    {
      ReturnToMenuNoHandler();
      return true;
    }

    return false;
  }

  void CheckRestart()
  {
    if (IsGameOver && (Input.GetKeyDown(KeyCode.Return)
                    || Input.GetKeyDown(KeyCode.Y)))
    {
      RestartYesHandler();
    }
    else if (IsGameOver && (Input.GetKeyDown(KeyCode.Escape)
                         || Input.GetKeyDown(KeyCode.N)))
    {
      RestartNoHandler();
    }
  }

  Color _playingTrackColor = Color.white;

  const float _alphaControlDelay = 3.0f;
  float _playingTrackAlphaControl = _alphaControlDelay;

  bool _musicKeyPressed = false;
  void MusicTrackControl()
  {
    _musicKeyPressed = false;

    if (Input.GetKeyDown(KeyCode.LeftBracket))
    {
      _musicTrackIndex++;
      _musicKeyPressed = true;
    }
    else if (Input.GetKeyDown(KeyCode.RightBracket))
    {
      _musicTrackIndex--;
      _musicKeyPressed = true;
    }

    if (_musicKeyPressed)
    {
      if (_musicTrackIndex < 0)
      {
        _musicTrackIndex = GlobalConstants.MusicTracks.Count - 1;
      }

      _musicTrackIndex %= GlobalConstants.MusicTracks.Count;

      _playingTrackColor.a = 1.0f;

      _playingTrackAlphaControl = _alphaControlDelay;

      SetTrackName();

      SoundManager.Instance.PlayMusicTrack(GlobalConstants.MusicTracks[_musicTrackIndex].Key);
    }

    _playingTrackAlphaControl -= Time.smoothDeltaTime;
    _playingTrackAlphaControl = Mathf.Clamp(_playingTrackAlphaControl, 0.0f, _alphaControlDelay);

    if (_playingTrackAlphaControl < 1.0f)
    {
      _playingTrackColor.a = _playingTrackAlphaControl;
    }

    _playingTrackColor.a = Mathf.Clamp01(_playingTrackColor.a);

    MusicPlayingTrackText.color = _playingTrackColor;
  }

  void Update()
  {
    MusicTrackControl();

    CheckRestart();

    if (IsGameOver)
    {
      return;
    }

    if (CheckReturnToMenu())
    {
      return;
    }

    if (_powerupSpawned)
    {
      _powerupLifetimeTimer += Time.smoothDeltaTime;

      if (_powerupLifetimeTimer > GlobalConstants.PowerupLifetime)
      {
        ResetPowerupLock();
      }
    }

#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.P))
    {
      float x = Random.Range(-5.0f, 5.0f);
      float y = Random.Range(-5.0f, 5.0f);
      TryToSpawnPowerup(new Vector2(x, y));
      //TryToSpawnSpecialPowerup(new Vector2(x, y), true);
    }

    if (Input.GetKeyDown(KeyCode.U))
    {
      SpawnUfo(UfoController.UfoVariant.EMP);
    }

    if (Input.GetKeyDown(KeyCode.L))
    {
      SpawnUfo(UfoController.UfoVariant.LAME);
    }

    if (Input.GetKeyDown(KeyCode.E))
    {
      SpawnUfo(UfoController.UfoVariant.ELITE);
    }

    if (Input.GetKeyDown(KeyCode.Y))
    {
      PlayerScript.AddExperience(1000);
      Debug.Log(PlayerScript.Level);
    }

    if (Input.GetKeyDown(KeyCode.C))
    {
      TryToSpawnSpecialPowerup(Vector2.zero, 1, true);
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      TryToSpawnSpecialPowerup(Vector2.zero, 0, true);
    }

    if (Input.GetKeyDown(KeyCode.T))
    {
      ExhaustAllUfoVariants();
    }
#endif

    if (Input.GetKeyDown(KeyCode.Escape) && !_returnToMainOpen)
    {
      _returnToMainOpen = true;

      ReturnToMenuScreen.gameObject.SetActive(true);
    }

    _hpBar.Length = 0;

    int barLength = (PlayerScript.Hitpoints * GlobalConstants.GuiHitpointsShieldBarLength) / PlayerScript.MaxPoints;

    _hpBar.Append('>', barLength);

    _shieldBar.Length = 0;

    barLength = (PlayerScript.Shieldpoints * GlobalConstants.GuiHitpointsShieldBarLength) / PlayerScript.MaxPoints;

    _shieldBar.Append('>', barLength);

    HitpointsBar.text = _hpBar.ToString();
    ShieldpointsBar.text = _shieldBar.ToString();

    if (PlayerScript.Level < GlobalConstants.ExperienceByLevel.Count)
    {
      ExperienceText.text = string.Format("{0} / {1}", PlayerScript.Experience, GlobalConstants.ExperienceByLevel[PlayerScript.Level]);
    }
    else
    {
      ExperienceText.text = "MAX";
    }

    AsteroidsCount.text = string.Format("S: {0} / {1}", SpawnedAsteroids, GlobalConstants.AsteroidsMaxInstances);
    //AsteroidsCount.text = string.Format("S: {0} / {1}  E: {2} / {3}", SpawnedAsteroids, GlobalConstants.AsteroidsMaxInstances, SpawnedUfos, GlobalConstants.MaxUfosPerPlayerLevel[PlayerScript.Level]);
    PhaseCount.text = string.Format("PHASE {0}", _currentPhase);
    SpawnRate.text = string.Format("R: {0:0.00}", _currentSpawnRate / GlobalConstants.MaxSpawnRate);
    ScoreCount.text = Score.ToString();

    if (SpawnedAsteroids < GlobalConstants.AsteroidsMaxInstances)
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

        _currentPhase++;

        _currentSpawnRate -= GlobalConstants.SpawnRateDelta * _spawnAcceleration;

        _spawnAcceleration += 0.2f;

        _spawnAcceleration = Mathf.Clamp(_spawnAcceleration, 1.0f, 2.0f);

        _currentSpawnRate = Mathf.Clamp(_currentSpawnRate, GlobalConstants.MaxSpawnRate, GlobalConstants.StartingSpawnRate);

        _progressBarUpdateInterval = _currentSpawnRate / 20.0f;

        _progressBarSb.Length = 0;

        DecideSpawn();

        _specialPowerupRespawnTimeoutCounter++;

        if (_specialPowerupWasPicked
         && _specialPowerupRespawnTimeoutCounter > _specialPowerupTimeoutPhases)
        {
          _specialPowerupWasPicked = false;
        }
      }

      SpawnProgressBar.text = _progressBarSb.ToString();
    }
  }

  void DecideSpawn()
  {
#if UNITY_EDITOR
    string output = string.Empty;
    foreach (var kvp in UfoControllerScript.SpawnedUfosByVariant)
    {
      output += string.Format("{0} {1} ", kvp.Key, kvp.Value);
    }
    Debug.Log(output);
#endif

    UfoController.UfoVariant ufoVariant = ExhaustAllUfoVariants();

    if (ufoVariant == UfoController.UfoVariant.LAST_ELEMENT)
    {
      SpawnAsteroid();
    }
    else
    {
      SpawnUfo(ufoVariant);
    }
  }

  int _nextUfoSpawnNudge = 0;
  UfoController.UfoVariant ExhaustAllUfoVariants()
  {
    UfoController.UfoVariant variant = UfoController.UfoVariant.LAST_ELEMENT;

    // Check total number of UFOs on screen
    int activeUfos = 0;
    foreach (var kvp in UfoControllerScript.SpawnedUfosByVariant)
    {
      activeUfos += kvp.Value;
    }

    // If we can spawn some more because player level allows...
    if (activeUfos < GlobalConstants.MaxUfosPerPlayerLevel[PlayerScript.Level])
    {
      int chance = Random.Range(0, 101) - _nextUfoSpawnNudge;

#if UNITY_EDITOR
      Debug.Log("UFO spawn: chance " + chance);
#endif

      //
      // If we hit the maximum category, but can't spawn it due to
      // maximum allowed spawned objects of that category, move to the next
      // one in order until we can either spawn something or can't.
      //
      var candidates = GetSpawnCandidates(chance);
      for (int i = 0; i < candidates.Count; i++)
      {
        UfoController.UfoVariant v = candidates[i];
        bool isAllowed = GlobalConstants.AllowedUfoVariantsByPlayerLevel[PlayerScript.Level].Contains(v);
        if (isAllowed)
        {
          bool canBeSpawned = (UfoControllerScript.SpawnedUfosByVariant[v] < GlobalConstants.MaximumAllowedUfosByVariant[v]);
          if (canBeSpawned)
          {
            _nextUfoSpawnNudge = 0;
            variant = v;
            break;
          }
        }
      }
    }

    if (variant == UfoController.UfoVariant.LAST_ELEMENT)
    {
      _nextUfoSpawnNudge++;
      _nextUfoSpawnNudge = Mathf.Clamp(_nextUfoSpawnNudge, 1, 50);
    }

#if UNITY_EDITOR
    Debug.Log("UFO spawn result: " + variant);
#endif

    return variant;
  }

  List<UfoController.UfoVariant> GetSpawnCandidates(int chance)
  {
    List<UfoController.UfoVariant> res = new List<UfoController.UfoVariant>();

    if (chance <= GlobalConstants.UfoSpawnChanceByVariant[UfoController.UfoVariant.ELITE])
    {
      res.Add(UfoController.UfoVariant.ELITE);
    }

    if (chance <= GlobalConstants.UfoSpawnChanceByVariant[UfoController.UfoVariant.EMP])
    {
      res.Add(UfoController.UfoVariant.EMP);
    }

    if (chance <= GlobalConstants.UfoSpawnChanceByVariant[UfoController.UfoVariant.LAME])
    {
      res.Add(UfoController.UfoVariant.LAME);
    }

    return res;
  }

  void SpawnUfo(UfoController.UfoVariant variant)
  {
    SoundManager.Instance.PlaySound("ufo-spawn", 0.4f);

    int index = Random.Range(0, _spawnPoints.Count);

    GameObject ase = Instantiate(AsteroidSpawnEffect, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    Destroy(ase.gameObject, 1.0f);

    UfoControllerScript.SpawnUfo(_spawnPoints[index], variant);
  }

  void SpawnAsteroid()
  {
    int index = Random.Range(0, _spawnPoints.Count);

    GameObject ase = Instantiate(AsteroidSpawnEffect, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    Destroy(ase.gameObject, 1.0f);

    foreach (var item in _asteroidControllers)
    {
      if (!item.IsActive)
      {
        SoundManager.Instance.PlaySound("asteroid_spawn", 0.5f);
        item.Init(_spawnPoints[index]);
        SpawnedAsteroids++;
        break;
      }
    }

    /*
    GameObject go = Instantiate(AsteroidPrefab, new Vector3(_spawnPoints[index].x, _spawnPoints[index].y, 0.0f), Quaternion.identity);
    go.transform.parent = AsteroidsHolder;

    var asteroid = go.GetComponent<Asteroid>();
    asteroid.Init(_spawnPoints[index], 0);
    asteroid.PushRandom();
    */
  }

  public void SpawnPowerup(Vector2 position)
  {
    if (PlayerScript == null)
    {
      return;
    }

    _powerupPosition.Set(position.x, position.y);

    if (_powerupPosition.x < _screenRect[0]) _powerupPosition.x = _screenRect[0] + 1.0f;
    if (_powerupPosition.x > _screenRect[2]) _powerupPosition.x = _screenRect[2] - 1.0f;
    if (_powerupPosition.y < _screenRect[1]) _powerupPosition.y = _screenRect[1] + 1.0f;
    if (_powerupPosition.y > _screenRect[3]) _powerupPosition.y = _screenRect[3] - 1.0f;

    int whichOne = (PlayerScript.Hitpoints < PlayerScript.Shieldpoints
                 || PlayerScript.Hitpoints < (PlayerScript.MaxPoints / 4)) ? 0 : 1;

    SoundManager.Instance.PlaySound("powerup_spawn", _powerupSpawnVolume);
    var effect = Instantiate(PowerupSpawnEffect, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);
    Destroy(effect, 1.0f);

    if (whichOne == 0)
    {
      Instantiate(HealthPowerupPrefab, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);
    }
    else
    {
      Instantiate(ShieldPowerupPrefab, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);
    }
  }

  const float _powerupSpawnVolume = 0.4f;

  bool _powerupSpawned = false;
  Vector2 _powerupPosition = Vector2.zero;
  public void TryToSpawnPowerup(Vector2 position)
  {
    if (PlayerScript == null || _powerupSpawned)
    {
      return;
    }

    bool success = false;

    _powerupPosition.Set(position.x, position.y);

    if (_powerupPosition.x < _screenRect[0]) _powerupPosition.x = _screenRect[0] + 1.0f;
    if (_powerupPosition.x > _screenRect[2]) _powerupPosition.x = _screenRect[2] - 1.0f;
    if (_powerupPosition.y < _screenRect[1]) _powerupPosition.y = _screenRect[1] + 1.0f;
    if (_powerupPosition.y > _screenRect[3]) _powerupPosition.y = _screenRect[3] - 1.0f;

    float modifierH = 1.0f - (float)PlayerScript.Hitpoints / (float)PlayerScript.MaxPoints;
    float chanceH = modifierH * GlobalConstants.PowerupSpawnPercent;
    float modifierS = 1.0f - (float)PlayerScript.Shieldpoints / (float)PlayerScript.MaxPoints;
    float chanceS = modifierS * GlobalConstants.PowerupSpawnPercent;

    int whichOne = ((PlayerScript.Hitpoints < PlayerScript.Shieldpoints)
                 || (PlayerScript.Hitpoints < (PlayerScript.MaxPoints / 4))) ? 0 : 1;

    float chance = Random.Range(0.0f, 100.0f);

    if (whichOne == 0)
    {
      #if UNITY_EDITOR
      Debug.Log(string.Format("Health powerup: rolled {0} chance {1}", chance, chanceH));
      #endif

      if (chance < chanceH)
      {
        SoundManager.Instance.PlaySound("powerup_spawn", _powerupSpawnVolume);

        var effect = Instantiate(PowerupSpawnEffect, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

        Destroy(effect, 1.0f);

        Instantiate(HealthPowerupPrefab, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

        //Debug.Log("health : " + chanceH + " " + modifierH + " chance rolled: " + chance);

        success = true;
      }
    }
    else
    {
      #if UNITY_EDITOR
      Debug.Log(string.Format("Shield powerup: rolled {0} chance {1}", chance, chanceS));
      #endif

      if (chance < chanceS)
      {
        SoundManager.Instance.PlaySound("powerup_spawn", _powerupSpawnVolume);

        var effect = Instantiate(PowerupSpawnEffect, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

        Destroy(effect, 1.0f);

        Instantiate(ShieldPowerupPrefab, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

        //Debug.Log("shield : " + chanceS + " " + modifierS + " chance rolled: " + chance);

        success = true;
      }
    }

    if (!success)
    {
      success = TryToSpawnSpecialPowerup(position);
    }

    _powerupSpawned = success;
  }

  public bool TryToSpawnSpecialPowerup(Vector2 position, int whichOneOverride = -1, bool debugMode = false)
  {
    if (_specialPowerupWasPicked && !debugMode)
    {
      return false;
    }

    bool res = false;

    _powerupPosition.Set(position.x, position.y);

    if (_powerupPosition.x < _screenRect[0]) _powerupPosition.x = _screenRect[0] + 1.0f;
    if (_powerupPosition.x > _screenRect[2]) _powerupPosition.x = _screenRect[2] - 1.0f;
    if (_powerupPosition.y < _screenRect[1]) _powerupPosition.y = _screenRect[1] + 1.0f;
    if (_powerupPosition.y > _screenRect[3]) _powerupPosition.y = _screenRect[3] - 1.0f;

    //float modifier = (float)SpawnedAsteroids / (float)GlobalConstants.AsteroidsMaxInstances;
    //float modifier = (float)AsteroidsOnScreen / (float)GlobalConstants.AsteroidsMaxInstances;
    //float chance = GlobalConstants.SpecialPowerupSpawnPercent * modifier;
    float rosaryChance = (float)AsteroidsOnScreen / ((float)GlobalConstants.AsteroidsMaxInstances / 24.0f);
    float clockChance = (0.8f - (_currentSpawnRate / GlobalConstants.StartingSpawnRate)) * 50.0f;

    int whichOne = Random.Range(0, SpecialPowerups.Count);
    float roll = Random.Range(0, 101);

    if (debugMode)
    {
      roll = 0;
      whichOne = whichOneOverride;
      rosaryChance = 100;
      clockChance = 100;
    }

#if UNITY_EDITOR
    Debug.Log(string.Format("Special powerup: roll = {0}, rosaryChance = {1}, clockChance = {2}", roll, rosaryChance, clockChance));
#endif

    // FIXME: duplicate code
    if (whichOne == 0 && roll < rosaryChance)
    {
      SoundManager.Instance.PlaySound("powerup_spawn", _powerupSpawnVolume);
      var effect = Instantiate(PowerupSpawnEffect, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);
      Destroy(effect, 1.0f);

      Instantiate(SpecialPowerups[whichOne], new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

      res = true;
    }
    else if (whichOne == 1 && roll < clockChance)
    {
      SoundManager.Instance.PlaySound("powerup_spawn", _powerupSpawnVolume);
      var effect = Instantiate(PowerupSpawnEffect, new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);
      Destroy(effect, 1.0f);

      Instantiate(SpecialPowerups[whichOne], new Vector3(_powerupPosition.x, _powerupPosition.y, 0.0f), Quaternion.identity);

      res = true;
    }

    return res;
  }

  public void SetWeapon(int weaponIndex)
  {
    WeaponIcon.sprite = BulletsIcons[weaponIndex];
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += SceneLoadedHandler;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= SceneLoadedHandler;
  }

  string _musicPlayingTrackFormat = "Playing Track:\n[ {0} ]";

  void SetTrackName()
  {
    string trackName = GlobalConstants.MusicTracks[_musicTrackIndex].Value;
    MusicPlayingTrackText.text = string.Format(_musicPlayingTrackFormat, trackName);
  }

  int _musicTrackIndex = 0;
  void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
  {
    // May introduce unnecessary lags
    // (see comments in TitleScreen:;SceneLoadedHandler())
    //
    System.GC.Collect();

    LoadingScreen.Instance.Hide();

    _musicTrackIndex = Random.Range(1, GlobalConstants.MusicTracks.Count);

    SetTrackName();

    SoundManager.Instance.PlayMusicTrack(GlobalConstants.MusicTracks[_musicTrackIndex].Key);
  }
}
