using UnityEngine;

public class PlayerHandleTouch : MonoBehaviour
{
    public FixedTouchField fixedTouchField;
    public PlayerLookAround cameraLook;
    void Start()
    {

    }


    void Update()
    {
        cameraLook.LockAxis = fixedTouchField.TouchDist;
        //Debug.Log(fixedTouchField.TouchDist);

        fixedTouchField.TouchDist = Vector2.zero;
    }
}
