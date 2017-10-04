using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormYesNo : FormBase 
{
  Callback _closeYesCallback = null;
  Callback _closeNoCallback = null;

  public void SetActions(Callback affirmativeAction, Callback negativeAction)
  {
    _closeYesCallback = affirmativeAction;
    _closeNoCallback = negativeAction;
  }

  public void YesHandler()
  {
    Close();

    if (_closeYesCallback != null)
      _closeYesCallback();
  }

  public void NoHandler()
  {
    Close();

    if (_closeNoCallback != null)
      _closeNoCallback();
  }

  public override void Close()
  {
    if (_parentForm == null)
    {
      return;
    }

    gameObject.SetActive(false);
    _parentForm.gameObject.SetActive(true);

    _parentForm = null;
  }
}
