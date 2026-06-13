using TMPro;
using UnityEngine;

public class CustomerOrderUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dishText;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private Transform target;

    public void Setup(Transform followTarget, BowlState expectedState)
    {
        target = followTarget;
        dishText.text = DishNameHelper.GetDishName(expectedState);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.forward = Camera.main.transform.forward; // luôn quay về camera
    }
}
