using System.Collections.Generic; // Подключаем пространство имён для работы со списками.
using NUnit.Framework; // Подключаем фреймворк для тестирования.
using UnityEngine; // Подключаем Unity API.

public class WayPoint : MonoBehaviour // Определяем класс WayPoint, который наследуется от MonoBehaviour.
{
    public WayPoint PreviousWayPoint; // Ссылка на предыдущую точку маршрута.
    public WayPoint NextWayPoint; // Ссылка на следующую точку маршрута.
    [UnityEngine.Range(0f, 5f)] public float Width = 3f; // Поле для ширины слайдера в диапазоне от 0 до 5, по умолчанию 3.
    
    public List<WayPoint> WayPoints = new List<WayPoint>(); // Список всех точек маршрута.

    public Vector3 GetPositionWayPoint() // Метод для получения случайной позиции внутри ширины точки маршрута.
    {
        Vector3 minBounds = transform.position + transform.right * Width / 2f; // Рассчитываем минимальную границу сдвига вправо.
        Vector3 maxBounds = transform.position - transform.right * Width / 2f; // Рассчитываем максимальную границу сдвига влево.
        return Vector3.Lerp(minBounds, maxBounds, Random.Range(0f, 1f)); // Возвращаем случайную позицию между этими границами.
    }
}