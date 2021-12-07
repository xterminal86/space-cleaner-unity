using System.Collections.Generic;
using UnityEngine;

public class UfoController : MonoBehaviour
{
  public enum UfoVariant
  {
    LAME = 0,
    EMP,
    ELITE,
    LAST_ELEMENT
  }

  /*
  public Dictionary<UfoVariant, int> HitpointsByVariant = new Dictionary<UfoVariant, int>()
  {
    { UfoVariant.LAME,  20 },
    { UfoVariant.EMP,   10 },
    { UfoVariant.ELITE, 40 }
  };

  public Dictionary<UfoVariant, int> ShieldPointsByVariant = new Dictionary<UfoVariant, int>()
  {
    { UfoVariant.LAME,  20 },
    { UfoVariant.EMP,   40 },
    { UfoVariant.ELITE, 60 }
  };

  public Dictionary<UfoVariant, KeyValuePair<float, float>> SpeedByVariant = new Dictionary<UfoVariant, KeyValuePair<float, float>>()
  {
    { UfoVariant.LAME,  new KeyValuePair<float, float>(50.0f, 100.0f) },
    { UfoVariant.EMP,   new KeyValuePair<float, float>(125.0f, 150.0f) },
    { UfoVariant.ELITE, new KeyValuePair<float, float>(25.0f, 50.0f) }
  };

  public Dictionary<UfoVariant, KeyValuePair<float, float>> MoveDirChangeTimeoutByVariant = new Dictionary<UfoVariant, KeyValuePair<float, float>>()
  {
    { UfoVariant.LAME,  new KeyValuePair<float, float>(1.0f, 2.0f)   },
    { UfoVariant.EMP,   new KeyValuePair<float, float>(0.75f, 1.25f) },
    { UfoVariant.ELITE, new KeyValuePair<float, float>(2.0f, 4.0f)   }
  };

  public Dictionary<UfoVariant, float> ShootingTimeoutByVariant = new Dictionary<UfoVariant, float>()
  {
    { UfoVariant.LAME,  3.0f },
    { UfoVariant.EMP,   2.0f },
    { UfoVariant.ELITE, 1.0f },
  };
  */

  Dictionary<UfoVariant, int> _spawnedUfosByVariant = new Dictionary<UfoVariant, int>()
  {
    { UfoVariant.LAME,  0 },
    { UfoVariant.EMP,   0 },
    { UfoVariant.ELITE, 0 }
  };

  public Dictionary<UfoVariant, int> SpawnedUfosByVariant
  {
    get { return _spawnedUfosByVariant; }
  }

  public List<GameObject> UfoPrefabs;

  public GameObject SpawnUfo(Vector2 pos, UfoVariant variant)
  {
    GameObject go = Instantiate(UfoPrefabs[(int)variant], new Vector3(pos.x, pos.y, 0.0f), Quaternion.identity);
    UfoBase bc = go.GetComponent<UfoBase>();
    bc.Setup(this, variant);
    return go;
  }
}
