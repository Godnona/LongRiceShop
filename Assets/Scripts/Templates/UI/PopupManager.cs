using UnityEngine;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    private const int SORT_ORDER_POPUP = 1;
    private Dictionary<PopupName, BasePopup> popupInstances = new Dictionary<PopupName, BasePopup>();
    private Transform popupParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        popupParent = transform;
    }

    public BasePopup ShowPopup(PopupName popupName)
    {
        // nếu popup chưa được tạo → load từ Resources
        if (!popupInstances.ContainsKey(popupName))
        {
            var prefab = Resources.Load<BasePopup>($"UI/Popups/{popupName}");
            if (prefab == null)
            {
                Debug.LogError($"Không tìm thấy popup: {popupName}");
                return null;
            }

            var instance = Instantiate(prefab, popupParent);
            popupInstances.Add(popupName, instance);
        }

        var popup = popupInstances[popupName];

        // Đưa popup này lên trên cùng (hiển thị trên các popup khác)
        popup.GetComponent<Canvas>().sortingOrder = SORT_ORDER_POPUP;
        popup.transform.SetAsLastSibling();

        popup.Show();
        return popup;
    }

    public void HidePopup(PopupName popupName)
    {
        if (popupInstances.ContainsKey(popupName))
        {
            popupInstances[popupName].Hide();
        }
    }

    public void HideAll()
    {
        foreach (var p in popupInstances.Values)
        {
            p.Hide();
        }
    }
}
