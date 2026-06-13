using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float rayDistance = 3f;
    public GameObject bowlPoint;

    [SerializeField] private NotificationManager notification;
    private string lastItemName = "";

    public ChefSeat chefSeat;

    public LayerMask seatLayer;
    public LayerMask bowlLayer;
    public LayerMask ingredientLayer;
    public LayerMask dishLayer;
    public LayerMask trashLayer;

    public LayerMask tableLayer;
    public LayerMask dogLayer;
    public LayerMask dogPointLayer;
    private Dog carriedDog;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleLookAtItem();

        if (Input.GetKeyDown(KeyCode.E))
            InteractItem();
    }

    public void InteractItem()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        // ===============================
        // 1️⃣ TRASH – ƯU TIÊN CAO NHẤT
        // ===============================
        if (Physics.Raycast(ray, out RaycastHit trashHit, rayDistance, trashLayer))
        {
            trashHit.collider.GetComponentInParent<TrashBin>()?.ThrowAway();
            return;
        }

        // ===============================
        // 2️⃣ BẮT CHÓ
        // ===============================
        if (Physics.Raycast(ray, out RaycastHit dogHit, rayDistance, dogLayer))
        {
            Dog dog = dogHit.collider.GetComponentInParent<Dog>();
            if (dog != null && carriedDog == null)
            {
                dog.CatchDog(bowlPoint.transform);
                carriedDog = dog;
                return;
            }
        }

        // ===============================
        // 3️⃣ ĐẶT CHÓ
        // ===============================
        if (carriedDog != null &&
            Physics.Raycast(ray, out RaycastHit pointHit, rayDistance, dogPointLayer))
        {
            carriedDog.PlaceDog();
            carriedDog = null;
            return;
        }

        // ===============================
        // 4️⃣ GIAO MÓN
        // ===============================
        if (PlayerState.Instance.isHoldingDish &&
            Physics.Raycast(ray, out RaycastHit tableHit, rayDistance, tableLayer))
        {
            CustomerTable table = tableHit.collider.GetComponentInParent<CustomerTable>();
            if (table != null)
            {
                bool delivered = table.PlaceDish(PlayerState.Instance.holdingDish);
                if (delivered)
                {
                    PlayerState.Instance.isHoldingDish = false;
                    PlayerState.Instance.holdingDish = null;
                }
                return;
            }
        }

        // ===============================
        // 5️⃣ PICKUP MÓN (SAU TRASH)
        // ===============================
        if (!PlayerState.Instance.isHoldingDish &&
            Physics.Raycast(ray, out RaycastHit dishHit, rayDistance, dishLayer))
        {
            DishPickup dish = dishHit.collider.GetComponent<DishPickup>();
            if (dish != null)
            {
                PlayerState.Instance.isHoldingDish = true;
                PlayerState.Instance.holdingDish = dish;

                Transform t = dish.transform;
                t.SetParent(bowlPoint.transform);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;

                return;
            }
        }

        // ===============================
        // 6️⃣ NGỒI GHẾ
        // ===============================
        if (!PlayerState.Instance.isSittingAtChefSeat &&
            Physics.Raycast(ray, out RaycastHit seatHit, rayDistance, seatLayer))
        {
            seatHit.collider.GetComponentInParent<ChefSeat>()?.Sit(gameObject);
            return;
        }

        // ===============================
        // 7️⃣ SPAWN BOWL
        // ===============================
        if (PlayerState.Instance.isSittingAtChefSeat &&
            Physics.Raycast(ray, out _, rayDistance, bowlLayer))
        {
            CookingManager.Instance.SpawnBowl();
            return;
        }

        // ===============================
        // 8️⃣ CHỌN NGUYÊN LIỆU
        // ===============================
        if (PlayerState.Instance.isSittingAtChefSeat &&
            Physics.Raycast(ray, out RaycastHit ingHit, rayDistance, ingredientLayer))
        {
            ingHit.collider.GetComponent<Ingredient>()?.OnSelected();
        }
    }


    void HandleLookAtItem()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        // ===== TRASH ƯU TIÊN =====
        if (Physics.Raycast(ray, out _, rayDistance, trashLayer))
        {
            ShowItemName("Throw Bowl");
            return;
        }

        // ===== ĐẶT CHÓ =====
        if (carriedDog != null &&
            Physics.Raycast(ray, out _, rayDistance, dogPointLayer))
        {
            ShowItemName("Place Dog");
            return;
        }

        // ===== BẮT CHÓ =====
        if (carriedDog == null &&
            Physics.Raycast(ray, out _, rayDistance, dogLayer))
        {
            ShowItemName("Catch Dog");
            return;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (PlayerState.Instance.isSittingAtChefSeat)
            {
                if (hit.collider.TryGetComponent(out Ingredient ing))
                {
                    ShowItemName(ing.ingredientName);
                    return;
                }

                if (hit.collider.TryGetComponent(out BowlNoti bowl))
                {
                    ShowItemName("Bowl");
                    return;
                }
            }
            else
            {
                if (hit.collider.TryGetComponent(out DishPickup dish))
                {
                    ShowItemName(dish.dishName);
                    return;
                }

                if (hit.collider.TryGetComponent(out ChairNoti chair))
                {
                    ShowItemName("Chair");
                    return;
                }

                if (hit.collider.TryGetComponent(out CustomerTable _))
                {
                    ShowItemName("Serve Dish");
                    return;
                }

                
            }
        }

        notification.HideInstant();
        lastItemName = "";
    }



    public void Stand()
    {
        chefSeat.Stand();
    }    

    void ShowItemName(string itemName)
    {
        if (lastItemName == itemName) return;

        lastItemName = itemName;
        notification.ShowNotification(itemName);
    }
}
