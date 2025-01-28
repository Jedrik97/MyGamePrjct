using UnityEditor;
using UnityEngine;

// Инициализация класса при загрузке Unity Editor
[InitializeOnLoad]

// Класс для отображения визуализации точек пути (Waypoint) в редакторе
public class WayPointVision
{
    // Атрибут для рисования гизмо (вспомогательных графических элементов в редакторе)
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawGizmo(WayPoint wayPoint, GizmoType gizmoType)
    {
        // Определяем цвет гизмо в зависимости от того, выбрана ли точка в редакторе
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.green; // Яркий зелёный для выбранной точки
        }
        else
        {
            Gizmos.color = Color.green * 0.5f; // Полупрозрачный зелёный для невыбранной
        }
        
        // Рисуем сферу в позиции точки пути
        Gizmos.DrawSphere(wayPoint.transform.position, 0.2f);

        // Рисуем линию, обозначающую ширину пути
        Gizmos.color = Color.black; // Цвет линии - чёрный
        Gizmos.DrawLine(wayPoint.transform.position + (wayPoint.transform.right * wayPoint.Width/2), // Левая граница ширины
                        wayPoint.transform.position - (wayPoint.transform.right * wayPoint.Width/2)); // Правая граница ширины

        // Если есть предыдущая точка пути, рисуем линию до неё
        if (wayPoint.PreviousWayPoint != null)
        {
            Gizmos.color = Color.yellow; // Цвет линии - жёлтый
            Vector3 offset = wayPoint.transform.right * wayPoint.Width / 2; // Смещение для текущей точки
            Vector3 offsetTo = wayPoint.PreviousWayPoint.transform.right * wayPoint.PreviousWayPoint.Width / 2; // Смещение для предыдущей точки
            Gizmos.DrawLine(wayPoint.transform.position + offset, wayPoint.PreviousWayPoint.transform.position + offsetTo); // Линия между точками
        }

        // Если есть следующая точка пути, рисуем линию до неё
        if (wayPoint.NextWayPoint != null)
        {
            Gizmos.color = Color.blue; // Цвет линии - синий
            Vector3 offset = wayPoint.transform.right * -wayPoint.Width / 2; // Смещение для текущей точки
            Vector3 offsetTo = wayPoint.NextWayPoint.transform.right * -wayPoint.NextWayPoint.Width / 2; // Смещение для следующей точки
            Gizmos.DrawLine(wayPoint.transform.position + offset, wayPoint.NextWayPoint.transform.position + offsetTo); // Линия между точками
        }
    }
}
