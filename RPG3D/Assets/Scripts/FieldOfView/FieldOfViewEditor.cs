using UnityEngine; // Подключаем пространство имён Unity.
using UnityEditor; // Подключаем пространство имён UnityEditor для работы с редактором.

[CustomEditor(typeof(FieldOfView))] // Указываем, что этот редактор предназначен для работы с компонентом FieldOfView.
public class FieldOfViewEditor : Editor // Определяем класс редактора, наследуемый от Editor.
{
    private void OnSceneGUI() // Метод для рисования элементов на сцене.
    {
        FieldOfView fov = (FieldOfView)target; // Получаем ссылку на целевой компонент FieldOfView.
        Handles.color = Color.white; // Устанавливаем белый цвет для элементов интерфейса.
        float thickness = 1.5f; // Задаём толщину линий.
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, thickness); // Рисуем круг радиусом толщины для области обзора.

        Vector3 viewAngleLeft = fov.DirectionFromAngle(-fov._viewAngle / 2, false); // Рассчитываем левую границу угла обзора.
        Vector3 viewAngleRight = fov.DirectionFromAngle(fov._viewAngle / 2, false); // Рассчитываем правую границу угла обзора.
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov._viewRadius); // Рисуем линию до правой границы обзора.
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov._viewRadius); // Рисуем линию до левой границы обзора.
      
        Handles.color = Color.yellow; // Устанавливаем жёлтый цвет для линий к целям.
        foreach (var visibleTarget in fov._targets) // Перебираем все цели, видимые в области обзора.
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.transform.position, thickness); // Рисуем линию от объекта до цели.
        }
    }
}