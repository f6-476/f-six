using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Spectator : NetworkBehaviour
{
    public static System.Action<Spectator> OnLocal;
    public System.Action<Ship> OnSelect;
    public System.Action OnDeselect;
    private int selectedIndex = 0;
    private Transform target;

    private Vector3 input;
    [SerializeField] private float movementSpeed = 50.0f;
    [SerializeField] private float rotationSpeed = 50.0f;

    private void Start()
    {
        if (!IsOwner) return;

        if (OnLocal != null) OnLocal(this);
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (target == null)
        {
            if (Mathf.Abs(input.x) > 0.01f)
            {
                transform.rotation *= Quaternion.AngleAxis(input.x * rotationSpeed * Time.deltaTime, transform.up);
            }

            Vector3 moveDirection = input.y * Vector3.up + input.z * transform.forward;
            transform.position += moveDirection * movementSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }

    public void GetNext(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        if (!ctx.performed) return;

        selectedIndex = (selectedIndex + 1) % (RaceManager.Singleton.Ships.Count + 1);

        if (selectedIndex == 0)
        {
            target = null;
            if (OnDeselect != null) OnDeselect();
        }
        else
        {
            Ship ship = RaceManager.Singleton.Ships[selectedIndex - 1];
            if (ship != null)
            {
                target = ship.transform;
                if (OnSelect != null) OnSelect(ship);
            }
        }
    }

    public void GetForwards(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.z = ctx.ReadValue<float>();
    }

    public void GetBackwards(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.z = -ctx.ReadValue<float>();
    }

    public void GetLeft(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.x = -ctx.ReadValue<float>();
    }

    public void GetRight(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.x = ctx.ReadValue<float>();
    }

    public void GetUpwards(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.y = ctx.ReadValue<float>();
    }

    public void GetDownwards(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;
        input.y = -ctx.ReadValue<float>();
    }
}
