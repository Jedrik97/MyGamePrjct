using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2, bool> OnMove; // bool для бега
    public event Action OnJump;

    private void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (movement.magnitude > 0.1f)
        {
            OnMove?.Invoke(movement, isRunning);
        }

        if (Input.GetButtonDown("Jump"))
        {
            OnJump?.Invoke();
        }
    }
}