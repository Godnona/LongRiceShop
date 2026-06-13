using UnityEngine;

public class CustomerTable : MonoBehaviour
{
    public bool isOccupied = false;
    private CustomerOrder currentCustomer;

    public void AssignCustomer(CustomerOrder customer)
    {
        currentCustomer = customer;
        isOccupied = true;
    }

    public bool PlaceDish(DishPickup dish)
    {
        if (currentCustomer == null)
        {
            Debug.LogWarning("Bàn không có khách");
            return false;
        }

        bool correct = currentCustomer.CheckDish(dish.DishState);

        if (correct)
        {
            currentCustomer.isCorrectDish = true;   // ⭐ QUAN TRỌNG
            currentCustomer.StopWaiting();
            Debug.Log("ĐÚNG MÓN – khách ăn");
            currentCustomer.Leave(true);
        }
        else
        {
            currentCustomer.isCorrectDish = false;
            Debug.Log("SAI MÓN – khách bỏ đi");
            currentCustomer.Leave(false);
        }

        Destroy(dish.gameObject);
        currentCustomer = null;
        isOccupied = false;

        return true;
    }

}
