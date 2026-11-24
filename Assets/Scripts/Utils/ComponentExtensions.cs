using UnityEngine;

/// <summary>
/// Методы расширения для валидации и безопасного поиска ссылок на компоненты в инспекторе.
/// </summary>
public static class ComponentExtensions
{
    /// <summary>
    /// Проверяет, назначена ли ссылка на компонент в инспекторе. Если ссылка пустая,
    /// пытается однозначно найти компонент на этом же <see cref="GameObject"/> и выводит предупреждение в консоль.
    /// </summary>
    /// <typeparam name="TComponent">Тип искомого компонента.</typeparam>
    /// <param name="owner">Объект-владелец, на котором выполняется проверка.</param>
    /// <param name="component">Проверяемое поле ссылки на компонент.</param>
    /// <returns><see langword="true"/>, если компонент назначен или успешно найден в единственном экземпляре; иначе — <see langword="false"/>.</returns>
    public static bool CheckComponent<TComponent>(this MonoBehaviour owner, ref TComponent component)
        where TComponent : Component
    {
        if (component != null)
            return true;

        TComponent[] found = owner.GetComponents<TComponent>();

        return TryResolveFromFound(owner, found, out component);
    }

    /// <summary>
    /// Проверяет, назначена ли ссылка на дочерний компонент в инспекторе. Если ссылка пустая,
    /// пытается однозначно найти компонент в иерархии дочерних объектов и выводит предупреждение в консоль.
    /// </summary>
    /// <typeparam name="TComponent">Тип искомого компонента.</typeparam>
    /// <param name="owner">Корневой объект-владелец, внутри которого выполняется поиск.</param>
    /// <param name="component">Проверяемое поле ссылки на компонент.</param>
    /// <param name="includeInactive">Учитывать ли неактивные дочерние объекты при поиске.</param>
    /// <returns><see langword="true"/>, если компонент назначен или успешно найден в единственном экземпляре; иначе — <see langword="false"/>.</returns>
    public static bool CheckComponentInChildren<TComponent>(this MonoBehaviour owner, ref TComponent component, bool includeInactive = true)
        where TComponent : Component
    {
        if (component != null)
            return true;

        TComponent[] found = owner.GetComponentsInChildren<TComponent>(includeInactive);

        return TryResolveFromFound(owner, found, out component);
    }

    /// <summary>
    /// Анализирует массив найденных компонентов, присваивает ссылку при однозначном совпадении
    /// и логирует предупреждение или ошибку при некорректной настройке объекта.
    /// </summary>
    /// <typeparam name="TComponent">Тип компонента.</typeparam>
    /// <param name="owner">Объект-владелец для привязки контекста лога.</param>
    /// <param name="found">Массив найденных компонентов.</param>
    /// <param name="component">Результирующая ссылка на компонент (или <see langword="null"/> при ошибке).</param>
    /// <returns><see langword="true"/>, если найден ровно один компонент; иначе — <see langword="false"/>.</returns>
    private static bool TryResolveFromFound<TComponent>(MonoBehaviour owner, TComponent[] found, out TComponent component) where TComponent : Component
    {
        component = default;
        if (found.Length == 1)
        {
            component = found[0];
            Debug.LogWarning($"Система работает не в штатном режиме! Не указана ссылка на компонент {typeof(TComponent).Name} у {owner.name}.", owner);
            return true;
        }

        if (found.Length > 1)
        {
            Debug.LogError($"Внутри {owner.name} найдено несколько ({found.Length}) компонентов {typeof(TComponent).Name}. Укажите ссылку на нужный компонент!", owner);
            return false;
        }

        Debug.LogError($"Компонент {typeof(TComponent).Name} не найден внутри {owner.name}!", owner);
        return false;
    }
}
