using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerSpawner : MonoBehaviour
{
    private enum CustomerState
    {
        MovingToOrder,
        AtOrder,
        MovingToTable,
        WaitingForFood
    }

    [Header("Customer Prefabs")]
    [SerializeField] private List<GameObject> customerPrefabs;
    [SerializeField] private CustomerOrderUI orderUIPrefab;

    [Header("Spawn & Order")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform orderSpot;

    [Header("Tables")]
    [SerializeField] private List<CustomerTable> tables;
    [SerializeField] private List<Transform> tableSpots;

    [Header("Exit Points")]
    [SerializeField] private Transform payPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float exitWaitTime = 5f;

    [Header("Settings")]
    [SerializeField] private float stoppingDistance = 0.05f;
    [SerializeField] private float orderWaitTime = 5f;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxCustomers = 5;

    // STATE
    private List<GameObject> currentCustomers = new();
    private Dictionary<GameObject, CustomerOrder> customerOrders = new();
    private Dictionary<GameObject, CustomerTable> customerTables = new();
    private Dictionary<GameObject, Transform> customerTableSpots = new();
    private Dictionary<GameObject, NavMeshAgent> customerAgents = new();
    private Dictionary<GameObject, CustomerState> customerStates = new();
    private Dictionary<GameObject, CustomerOrderUI> customerOrderUIs = new();


    private CookingManager cookingManager;

    void Start()
    {
        cookingManager = CookingManager.Instance;
        StartCoroutine(SpawnCustomersCoroutine());
    }

    void Update()
    {
        for (int i = currentCustomers.Count - 1; i >= 0; i--)
        {
            GameObject customerObj = currentCustomers[i];
            if (customerObj == null) continue;

            NavMeshAgent agent = customerAgents[customerObj];
            CustomerState state = customerStates[customerObj];

            if (agent.pathPending || !agent.hasPath) continue;

            if (agent.remainingDistance <= stoppingDistance)
            {
                switch (state)
                {
                    case CustomerState.MovingToOrder:
                        StartCoroutine(OrderCoroutine(customerObj));
                        break;
                    case CustomerState.MovingToTable:
                        StartCoroutine(StandAtTableSpot(customerObj));
                        break;
                }
            }
        }
    }

    // =========================
    // SPAWN KHÁCH TỰ ĐỘNG
    // =========================
    private IEnumerator SpawnCustomersCoroutine()
    {
        while (true)
        {
            if (currentCustomers.Count < maxCustomers)
            {
                SpawnCustomer();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCustomer()
    {
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        Vector3 spawnPos = spawnPoint.position;

        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            spawnPos = hit.position;

        GameObject customerObj = Instantiate(prefab, spawnPos, spawnPoint.rotation);
        currentCustomers.Add(customerObj);

        NavMeshAgent agent = customerObj.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(orderSpot.position);

        CustomerOrder order = customerObj.GetComponent<CustomerOrder>();
        order.Spawner = this; // Gán spawner cho khách

        customerOrders[customerObj] = order;
        customerAgents[customerObj] = agent;
        customerStates[customerObj] = CustomerState.MovingToOrder;

        // Spawn UI
        CustomerOrderUI ui = Instantiate(orderUIPrefab);
        ui.Hide();
        customerOrderUIs[customerObj] = ui;

        Debug.Log("👤 Khách spawn → đi order");
    }

    // =========================
    // KHÁCH ĐẾN ORDER → ĐỨNG 5S
    // =========================
    private IEnumerator OrderCoroutine(GameObject customerObj)
    {
        if (customerStates[customerObj] != CustomerState.MovingToOrder) yield break;

        NavMeshAgent agent = customerAgents[customerObj];
        CustomerOrder order = customerOrders[customerObj];

        customerStates[customerObj] = CustomerState.AtOrder;
        agent.isStopped = true;


        Debug.Log("🧾 Khách đến order, bắt đầu order 5s");
        yield return new WaitForSeconds(orderWaitTime);

        cookingManager.AssignOrder(order);

        // HIỆN UI MÓN ĂN TRÊN ĐẦU KHÁCH
        if (customerOrderUIs.TryGetValue(customerObj, out var ui))
        {
            string dishName = order.expectedState.ToString(); // hoặc map sang tên đẹp hơn
            ui.Setup(customerObj.transform, order.expectedState);
        }

        // Chọn bàn trống
        CustomerTable table = GetFreeTable();
        if (table != null)
        {
            table.isOccupied = true; // đặt bàn ngay lập tức

            int tableIndex = tables.IndexOf(table);
            Transform tableSpot = tableSpots[tableIndex];

            customerTables[customerObj] = table;
            customerTableSpots[customerObj] = tableSpot;

            agent.isStopped = false;
            agent.SetDestination(tableSpot.position);
            customerStates[customerObj] = CustomerState.MovingToTable;

            Debug.Log($"🪑 Khách đi tới bàn {table.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Không còn bàn trống, khách đứng chờ order");
            agent.isStopped = false;
        }
    }

    // =========================
    // KHÁCH ĐẾN BÀN → ĐỨNG CHÍNH XÁC
    // =========================
    private IEnumerator StandAtTableSpot(GameObject customerObj)
    {
        if (customerStates[customerObj] != CustomerState.MovingToTable) yield break;

        NavMeshAgent agent = customerAgents[customerObj];
        CustomerTable table = customerTables[customerObj];
        Transform tableSpot = customerTableSpots[customerObj];
        CustomerOrder order = customerOrders[customerObj];

        agent.isStopped = true;
        agent.Warp(tableSpot.position);

        table.AssignCustomer(order);
        customerStates[customerObj] = CustomerState.WaitingForFood;

        order.StartWaiting(); // Start countdown

        Debug.Log($"🍽️ Khách ngồi bàn {table.name}");
        yield break;
    }

    // =========================
    // LẤY BÀN CÒN TRỐNG
    // =========================
    private CustomerTable GetFreeTable()
    {
        foreach (var table in tables)
        {
            if (!table.isOccupied)
                return table;
        }
        return null;
    }

    // KHÁCH ĂN XONG → ĐI RA
    public void CustomerLeave(CustomerOrder order, bool leaveNormally = true)
    {
        GameObject leavingCustomer = null;
        foreach (var kv in customerOrders)
        {
            if (kv.Value == order)
            {
                leavingCustomer = kv.Key;
                break;
            }
        }

        if (leavingCustomer == null) return;

        StartCoroutine(CustomerExitCoroutine(leavingCustomer, leaveNormally));
    }

    private IEnumerator CustomerExitCoroutine(GameObject customerObj, bool leaveNormally)
    {
        if (customerObj == null) yield break;

        if (!customerAgents.TryGetValue(customerObj, out var agent))
            yield break;

        if (!agent.isOnNavMesh)
            yield break;

        agent.isStopped = false;

        // =====================
        // 1️⃣ PAY POINT
        // =====================
        if (leaveNormally && payPoint != null)
        {
            agent.SetDestination(payPoint.position);

            while (agent != null &&
                   agent.isOnNavMesh &&
                   (agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
            {
                yield return null;
            }

            // CỘNG TIỀN TẠI ĐÂY
            CustomerOrder order = customerOrders[customerObj];
            if (order.isCorrectDish)
            {
                int money = order.payAmount;

                // Update Progress
                GameplayProgressManager.Instance.AddCoin(money);
                GameplayProgressManager.Instance.AddServe();

                Debug.Log($"Khách trả {money}");
            }

            yield return new WaitForSeconds(exitWaitTime);
        }


        // =====================
        // 2️⃣ END POINT
        // =====================
        if (agent != null && agent.isOnNavMesh && endPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(endPoint.position);

            if (customerOrderUIs.TryGetValue(customerObj, out var ui))
            {
                ui.Hide();
            }

            while (agent != null &&
                   agent.isOnNavMesh &&
                   (agent.pathPending || agent.remainingDistance > agent.stoppingDistance))
            {
                yield return null;
            }
        }

        // =====================
        // 3️⃣ CLEAN DATA
        // =====================
        if (customerTables.TryGetValue(customerObj, out var table))
        {
            table.isOccupied = false;
        }

        if (customerOrderUIs.ContainsKey(customerObj))
        {
            Destroy(customerOrderUIs[customerObj].gameObject);
            customerOrderUIs.Remove(customerObj);
        }

        customerAgents.Remove(customerObj);
        customerOrders.Remove(customerObj);
        customerStates.Remove(customerObj);
        customerTables.Remove(customerObj);
        customerTableSpots.Remove(customerObj);
        currentCustomers.Remove(customerObj);

        // =====================
        // 4️⃣ DESTROY CUỐI CÙNG
        // =====================
        Destroy(customerObj);

        Debug.Log("🚶 Khách rời khỏi nhà hàng (DESTROY SAFE)");
    }
}
