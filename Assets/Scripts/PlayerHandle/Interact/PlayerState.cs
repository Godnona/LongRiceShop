using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    public bool isSittingAtChefSeat = false;

    public bool isHoldingDish;
    public DishPickup holdingDish;

    void Awake()
    {
        Instance = this;
    }
}
