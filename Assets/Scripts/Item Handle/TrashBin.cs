using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public void ThrowAway()
    {
        if (CookingManager.Instance == null) return;

        CookingManager.Instance.DiscardCurrentBowl();
    }
}
