using UnityEngine;

public class WoWCamera : MonoBehaviour
{
    public Transform target;         // Ссылка на трансформ игрока (персонажа)
    public float distance = 10.0f;     // Расстояние от камеры до персонажа
    public float xSpeed = 250.0f;      // Скорость вращения по горизонтали
    public float ySpeed = 120.0f;      // Скорость вращения по вертикали

    public float yMinLimit = -20f;     // Минимальный угол по вертикали
    public float yMaxLimit = 80f;      // Максимальный угол по вертикали
    public float distanceMin = 5f;     // Минимальное расстояние
    public float distanceMax = 20f;    // Максимальное расстояние

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        // Инициализируем углы согласно текущему повороту камеры
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Если у камеры есть Rigidbody, запрещаем его вращение
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            // Вращение камеры происходит только при зажатой правой кнопке мыши
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // Обновление зума (колёсико мыши) – всегда активно
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMin, distanceMax);

            // Вычисляем поворот камеры
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            // Применяем поворот и позицию к камере
            transform.rotation = rotation;
            transform.position = position;

            // Если зажата ПКМ, обновляем поворот игрока так, чтобы он соответствовал направлению камеры
            if (Input.GetMouseButton(1))
            {
                // Берем только горизонтальную составляющую поворота камеры
                Vector3 targetEuler = target.rotation.eulerAngles;
                targetEuler.y = rotation.eulerAngles.y;
                target.rotation = Quaternion.Euler(targetEuler);
            }
        }
    }

    // Метод для ограничения угла по вертикали
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
