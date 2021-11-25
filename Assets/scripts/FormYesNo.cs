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
    base.Close(true);

    if (_closeYesCallback != null)
      _closeYesCallback();
  }

  public void NoHandler()
  {
    base.Close();

    if (_closeNoCallback != null)
      _closeNoCallback();
  }

  public override void Process()
  {
    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Y))
    {
      YesHandler();
    }
    else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.N))
    {
      NoHandler();
    }
  }
}
