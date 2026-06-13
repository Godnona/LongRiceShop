using UnityEngine;

public class ButtonUnlockAllLevel : MonoBehaviour
{
    public void OnClickUnlock()
    {
        GameManager.Instance.UnlockAllLevels();
    }
}
