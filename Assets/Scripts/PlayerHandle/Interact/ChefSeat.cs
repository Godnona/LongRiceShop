using UnityEngine;
using UnityEngine.Rendering;

public class ChefSeat : MonoBehaviour
{
    public Transform sitPoint; // vị trí player sẽ ngồi
    public float sitRotateSpeed = 8f;

    private bool isOccupied = false;
    private GameObject currentPlayer;

    public void Sit(GameObject player)
    {
        if (isOccupied) return;

        isOccupied = true;
        currentPlayer = player;

        player.transform.position = sitPoint.position;
        player.transform.rotation = sitPoint.rotation;

        PlayerState.Instance.isSittingAtChefSeat = true;

        // Tắt di chuyển player nếu có
        var move = player.GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        // SET COLLIDER IS TRIGGER
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
            box.isTrigger = true;


        Debug.Log("Player đã ngồi vào ghế đầu bếp");
    }

    public void Stand()
    {
        if (!isOccupied || currentPlayer == null) return;

        // Reset state
        isOccupied = false;
        PlayerState.Instance.isSittingAtChefSeat = false;

        // Bật lại di chuyển
        var move = currentPlayer.GetComponent<PlayerMovement>();
        if (move != null) move.enabled = true;

        // Reset collider ghế
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
            box.isTrigger = false;

        currentPlayer = null;

        Debug.Log("Player đã đứng dậy khỏi ghế đầu bếp");
    }
}
