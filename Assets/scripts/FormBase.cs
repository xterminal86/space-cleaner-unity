using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormBase : MonoBehaviour 
{
  public List<FormBase> ChildForms;

  protected FormBase _parentForm;

  void Awake()
  {
    Init();
  }

  public virtual void Init()
  {
  }

  public virtual void SelectMenuItem(int itemIndex)
  {
  }

  public virtual void Select(FormBase parentForm)
  {   
    SoundManager.Instance.PlaySound(GlobalConstants.MenuSelectSound);

    _parentForm = parentForm;

    _parentForm.gameObject.SetActive(false);
    gameObject.SetActive(true);
  }

  public virtual void Close()
  {
    if (_parentForm == null)
    {
      return;
    }

    SoundManager.Instance.PlaySound(GlobalConstants.MenuBackSound);

    gameObject.SetActive(false);
    _parentForm.gameObject.SetActive(true);

    _parentForm = null;
  }

  public virtual void Process()
  {
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Close();
      return;
    }

    Process();
  }
}
