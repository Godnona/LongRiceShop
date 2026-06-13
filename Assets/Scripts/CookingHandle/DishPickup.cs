using UnityEngine;

public class DishPickup : MonoBehaviour
{
    public BowlState DishState { get; private set; }
    public string dishName;

    public void Init(BowlState state)
    {
        DishState = state;
    }
}