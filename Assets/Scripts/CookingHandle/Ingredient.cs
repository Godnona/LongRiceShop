using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public int ingredientId; // ID nguyên liệu (0,1,2...)
    public string ingredientName;

    public void OnSelected()
    {
        CookingManager.Instance.AddIngredient(ingredientId);
        Debug.Log("Chọn nguyên liệu ID: " + ingredientId);
    }
}
