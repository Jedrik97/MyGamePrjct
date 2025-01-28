using UnityEditor; // Подключаем пространство имён для работы с редактором Unity.
using UnityEngine; // Подключаем пространство имён Unity API.

public class WayPointEditor : EditorWindow // Определяем класс WayPointEditor, наследуемый от EditorWindow.
{
   [MenuItem("Waypoint Tools/WayPoint Editor")] // Добавляем пункт в меню Unity для открытия окна редактора.
   public static void Open() // Метод для открытия окна редактора.
   {
      GetWindow<WayPointEditor>(); // Получаем и отображаем окно WayPointEditor.
   }

   public Transform WayPointsRoot; // Переменная для хранения корневого объекта, содержащего все WayPoint.

   private void OnGUI() // Метод для отрисовки пользовательского интерфейса окна редактора.
   {
      SerializedObject obj = new SerializedObject(this); // Создаём объект для работы с сериализованными данными.
      EditorGUILayout.PropertyField(obj.FindProperty("WayPointsRoot")); // Создаём поле для назначения корня точек маршрута.

      if (!WayPointsRoot) // Проверка на наличие корневого объекта.
      {
         EditorGUILayout.HelpBox("No WayPoints Root is assigned.", MessageType.Warning); // Выводим предупреждение, если корневой объект не задан.
      }
      else
      {
         EditorGUILayout.BeginVertical("box"); // Начинаем вертикальный блок для кнопок.
         DrawButton(); // Вызываем метод для отрисовки кнопок управления.
         EditorGUILayout.EndVertical(); // Завершаем вертикальный блок.
      }
      
      obj.ApplyModifiedProperties(); // Применяем изменения в данных.
   }

   private void DrawButton() // Метод для создания кнопок управления.
   {
      if (GUILayout.Button("Add WayPoint")) // Кнопка для добавления нового WayPoint.
      {
         CreateWayPoint(); // Создаём новый WayPoint.
      }

      if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<WayPoint>()) // Проверяем, выбран ли объект с компонентом WayPoint.
      {
         if (GUILayout.Button("Add NextWayPoint")) // Кнопка для добавления следующего WayPoint.
         {
            CreateWayPointAfter(); // Создаём WayPoint после выбранного.
            if (GUILayout.Button("Previous WayPoint")) // Кнопка для добавления предыдущего WayPoint.
            {
               CreateWayPointBefore(); // Создаём WayPoint перед выбранным.
            }
         }
      }
   }

   private void CreateWayPointBefore() // Метод для создания WayPoint перед текущим.
   {
      GameObject wayPointObj = new GameObject("WayPoint" + WayPointsRoot.childCount, typeof(WayPoint)); // Создаём новый объект с компонентом WayPoint.
      wayPointObj.transform.SetParent(WayPointsRoot, false); // Назначаем родительским объектом корневой объект.
      WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>(); // Получаем компонент WayPoint у нового объекта.
      
      WayPoint selectedWayPoint = Selection.activeGameObject.GetComponent<WayPoint>(); // Получаем выбранный WayPoint.
      wayPointObj.transform.position = selectedWayPoint.transform.position; // Копируем позицию выбранного WayPoint.
      wayPointObj.transform.forward = selectedWayPoint.transform.forward; // Копируем направление выбранного WayPoint.

      if (selectedWayPoint.PreviousWayPoint) // Если у выбранного есть предыдущий WayPoint.
      {
         wayPoint.PreviousWayPoint = selectedWayPoint.PreviousWayPoint; // Присваиваем предыдущий WayPoint новому объекту.
         selectedWayPoint.PreviousWayPoint.NextWayPoint = wayPoint; // Связываем предыдущий WayPoint с новым.
      }
      
      wayPoint.NextWayPoint = selectedWayPoint; // Связываем новый WayPoint с текущим как следующий.
      selectedWayPoint.PreviousWayPoint = wayPoint; // Связываем текущий WayPoint с новым как предыдущий.
      
      wayPoint.transform.SetSiblingIndex(selectedWayPoint.transform.GetSiblingIndex()); // Устанавливаем порядок нового объекта.
      Selection.activeGameObject = wayPoint.gameObject; // Делаем новый WayPoint активным.
   }

   private void CreateWayPointAfter() // Метод для создания WayPoint после текущего.
   {
      GameObject wayPointObj = new GameObject("WayPoint" + WayPointsRoot.childCount, typeof(WayPoint)); // Создаём новый объект с компонентом WayPoint.
      wayPointObj.transform.SetParent(WayPointsRoot, false); // Устанавливаем родителя для нового объекта.
      WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>(); // Получаем компонент WayPoint у нового объекта.
      
      WayPoint selectedWayPoint = Selection.activeGameObject.GetComponent<WayPoint>(); // Получаем выбранный WayPoint.
      wayPointObj.transform.position = selectedWayPoint.transform.position; // Копируем позицию выбранного WayPoint.
      wayPointObj.transform.forward = selectedWayPoint.transform.forward; // Копируем направление выбранного WayPoint.
      wayPoint.PreviousWayPoint = selectedWayPoint; // Присваиваем текущий WayPoint как предыдущий.
      if (selectedWayPoint.NextWayPoint) // Если есть следующий WayPoint.
      {
         selectedWayPoint.NextWayPoint.PreviousWayPoint = wayPoint; // Связываем следующий WayPoint с новым.
         wayPoint.NextWayPoint = wayPoint; // Связываем новый WayPoint с текущим как следующий.
      }

      selectedWayPoint.NextWayPoint = wayPoint; // Обновляем связь текущего WayPoint.
      wayPoint.transform.SetSiblingIndex(selectedWayPoint.transform.GetSiblingIndex()); // Устанавливаем порядок в иерархии.
      Selection.activeGameObject = wayPoint.gameObject; // Делаем новый WayPoint активным.
   }

   private void CreateWayPoint() // Метод для создания нового WayPoint.
   {
      GameObject wayPointObj = new GameObject("WayPoint" + WayPointsRoot.childCount, typeof(WayPoint)); // Создаём новый объект с компонентом WayPoint.
      wayPointObj.transform.SetParent(WayPointsRoot, false); // Назначаем родителя для нового объекта.
      WayPoint wayPoint = wayPointObj.GetComponent<WayPoint>(); // Получаем компонент WayPoint у нового объекта.
      if (WayPointsRoot.childCount > 1) // Если в корне есть другие WayPoint.
      {
         wayPoint.PreviousWayPoint = WayPointsRoot.GetChild(WayPointsRoot.childCount - 2).GetComponent<WayPoint>(); // Присваиваем предыдущий WayPoint.
         wayPoint.PreviousWayPoint.NextWayPoint = wayPoint; // Связываем предыдущий WayPoint с новым.
         wayPoint.transform.position = wayPoint.PreviousWayPoint.transform.position; // Копируем позицию предыдущего.
         wayPoint.transform.forward = wayPoint.PreviousWayPoint.transform.forward; // Копируем направление предыдущего.
      }
      Selection.activeObject = wayPoint.gameObject; // Делаем новый WayPoint активным.
   }
}
