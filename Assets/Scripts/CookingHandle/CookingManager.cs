using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance;

    [Header("Spawn")]
    public GameObject bowlPrefab;
    public Transform bowlSpawnPoint;

    [Header("Seat")]
    public ChefSeat chefSeat;

    [Header("Recipes")]
    public List<Recipe> recipes;

    public int LastRecipeIndex { get; private set; } = -1;

    private BowlController currentBowl;
    private readonly List<int> selectedIngredients = new();

    public bool IsDoneFood;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnBowl()
    {
        DiscardCurrentBowl();

        if (currentBowl)
            Destroy(currentBowl.gameObject);

        var bowlObj = Instantiate(
            bowlPrefab,
            bowlSpawnPoint.position,
            bowlSpawnPoint.rotation
        );

        currentBowl = bowlObj.GetComponent<BowlController>();
    }

    public void AssignOrder(CustomerOrder customer)
    {
        if (customer == null)
        {
            Debug.LogError("CustomerOrder null");
            return;
        }

        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogError("Chưa có recipe");
            return;
        }


        Recipe recipe = recipes.FirstOrDefault(r => r.resultState == customer.expectedState);
        if (recipe == null)
        {
            Debug.LogError($"Không tìm thấy recipe cho món: {customer.expectedState}");
            return;
        }

        customer.expectedState = recipe.resultState;
        Debug.Log($"Khách order: {recipe.recipeName}");
    }

    public void AddIngredient(int id)
    {
        if (!PlayerState.Instance.isSittingAtChefSeat) return;
        if (!currentBowl || IsDoneFood) return;

        selectedIngredients.Add(id);
        UpdatePreview();

        foreach (var recipe in recipes)
        {
            if (recipe.ingredients.Count != selectedIngredients.Count) continue;

            if (recipe.ingredients.OrderBy(x => x)
                .SequenceEqual(selectedIngredients.OrderBy(x => x)))
            {
                CookSuccess(recipe);
                return;
            }
        }
    }

    // Hủy bowl
    public void DiscardCurrentBowl()
    {
        // Đang cầm món → bỏ
        if (PlayerState.Instance.isHoldingDish)
        {
            Destroy(PlayerState.Instance.holdingDish.gameObject);
            PlayerState.Instance.holdingDish = null;
            PlayerState.Instance.isHoldingDish = false;
        }

        // Hủy bowl hiện tại
        if (currentBowl != null)
        {
            Destroy(currentBowl.gameObject);
            currentBowl = null;
        }

        ResetPlate();
    }

    void UpdatePreview()
    {
        // Tắt toàn bộ trước
        currentBowl.ResetBowl();

        bool com = selectedIngredients.Contains(0);
        bool trung = selectedIngredients.Contains(1);
        bool suon = selectedIngredients.Contains(2);
        bool ca = selectedIngredients.Contains(3);
        bool cachua = selectedIngredients.Contains(4);
        bool bongcai = selectedIngredients.Contains(5);
        bool thitkho = selectedIngredients.Contains(6);
        bool ga = selectedIngredients.Contains(7);
        bool so = selectedIngredients.Contains(8);
        bool tomhum = selectedIngredients.Contains(9);

        // =====================
        // PREVIEW TỪNG NGUYÊN LIỆU
        // =====================
        if (com) currentBowl.ShowCom();
        if (trung) currentBowl.ShowTrung();
        if (suon) currentBowl.ShowSuon();
        if (ca) currentBowl.ShowCa();
        if (cachua) currentBowl.ShowCaChua();
        if (bongcai) currentBowl.ShowBongCai();
        if (thitkho) currentBowl.ShowThitKho();
        if (ga) currentBowl.ShowGa();
        if (so) currentBowl.ShowSo();
        if (tomhum) currentBowl.ShowTomHum();

        // =====================
        // COMBO (CHỈ ĐỔI KHI ĐÚNG)
        // =====================
        if (com && so && tomhum)
            currentBowl.ChangeState(BowlState.ComDacBiet);
        else if (com && ca && bongcai)
            currentBowl.ChangeState(BowlState.ComCa);
        else if (com && thitkho && cachua)
            currentBowl.ChangeState(BowlState.ComThitKho);
        else if (com && ga)
            currentBowl.ChangeState(BowlState.ComGa);
        else if (com && trung)
            currentBowl.ChangeState(BowlState.ComTrung);
        else if (com && suon)
            currentBowl.ChangeState(BowlState.ComSuon);
    }



    void CookSuccess(Recipe recipe)
    {
        IsDoneFood = true;
        LastRecipeIndex = recipes.IndexOf(recipe);

        currentBowl.EnablePickup(recipe.resultState);
        chefSeat.Stand();
    }

    public void ResetPlate()
    {
        selectedIngredients.Clear();
        IsDoneFood = false;
        LastRecipeIndex = -1;

        if (currentBowl != null)
            currentBowl.ResetBowl();
    }
}
