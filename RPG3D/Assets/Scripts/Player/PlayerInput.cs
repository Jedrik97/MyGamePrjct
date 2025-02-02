using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnJump;

    private void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movement.magnitude > 0.1f)
        {
            OnMove?.Invoke(movement);
        }

        if (Input.GetButtonDown("Jump"))
        {
            OnJump?.Invoke();
        }
    }
}