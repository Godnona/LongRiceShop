using UnityEngine;

public class ButtonResetLevel : MonoBehaviour
{
    public void OnClickReset()
    {
        GameManager.Instance.ResetProgress();
    }
}
