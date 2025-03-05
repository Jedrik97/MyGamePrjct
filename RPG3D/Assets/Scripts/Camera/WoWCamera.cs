using UnityEngine;

public class WoWCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float minDistance = 4.0f;
    public float maxDistance = 10.0f;
    public float zoomSpeed = 2.0f;
    public float rotationSpeed = 150.0f;
    public float pitchMin = 10f;
    public float pitchMax = 80f;
    public float smoothing = 5f;
    public float autoRotateSpeed = 2f; 
    public bool autoRotate = true;
    public float terrainAvoidanceOffset = 0.5f; 
    public Vector3 startOffset = new Vector3(0, 2, -5); 

    private float yaw = 0.0f;
    private float pitch = 20.0f;
    private float targetDistance;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 lastTargetPosition;

    void Start()
    {
        targetDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        lastTargetPosition = target.position;
        
        
        transform.position = target.position + startOffset;
        transform.LookAt(target.position);
    }

    void LateUpdate()
    {
        if (!target) return;

        bool isMovingBackward = Input.GetKey(KeyCode.S); 
        bool isRightMouseDown = Input.GetMouseButton(1);

       
        if (isRightMouseDown)
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }
        else if (autoRotate && target.position != lastTargetPosition)
        {
            Vector3 direction = (target.position - lastTargetPosition).normalized;
            if (direction.magnitude > 0.01f)
            {
                float dot = Vector3.Dot(target.forward, direction);
                if (dot > 0 || isMovingBackward) 
                {
                    yaw = Mathf.LerpAngle(yaw, Quaternion.LookRotation(direction).eulerAngles.y, Time.deltaTime * autoRotateSpeed);
                }
            }
        }

        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        lastTargetPosition = target.position;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetDistance *= (1.0f - scroll * zoomSpeed);
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

       
        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * smoothing);

        
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0.0f, 0.0f, -distance);
        Vector3 desiredPosition = target.position + offset;

        
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            desiredPosition = target.position + rotation * new Vector3(0.0f, 0.0f, -targetDistance);
        }

        
        if (Physics.Raycast(desiredPosition, Vector3.down, out hit))
        {
            if (hit.collider.CompareTag("Terrain"))
            {
                desiredPosition.y = hit.point.y + terrainAvoidanceOffset;
            }
        }

       
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, Time.deltaTime * smoothing);
        transform.LookAt(target.position);
    }
}
