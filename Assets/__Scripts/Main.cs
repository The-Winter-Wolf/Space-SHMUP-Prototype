using System.Collections;               // Для доступа к массивам и другим коллекциям
using System.Collections.Generic;       // Для доступа к спискам и словарям
using UnityEngine;                      // Для доступа к Unity
using UnityEngine.SceneManagement;      // Для загрузки и перезагрузки сцен

public class Main : MonoBehaviour
{
    static public Main S;   // Объект-одиночка Main
    static Dictionary<WeaponType, WeaponDefinition>     WEAP_DICT;  // Словарь

    [Header("Set in Inspector")]
    public GameObject[]         prefabEnemies;              // Массив шаблонов Enemy
    public float                enemySpawnPerSecond = 0.5f; // Вражеских кораблей в секунду
    public float                enemyDefaultPadding = 1.5f; // Отступ для позиционирования
    public WeaponDefinition[]   weaponDefinitions;          // Массив элементов типа WeaponDefinition
    public GameObject           prefabPowerUp;
    public WeaponType[]         powerUpFrequency = new WeaponType[] 
    {WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield};

    private BoundsCheck     bndCheck;   // Переменная для ссылки на класс BoundsCheck

    public void ShipDestroyed(Enemy e) 
    {
        // Сгенерировать бонус с заданной вероятностью
        if (Random.value <= e.powerUpDropChance) {
            // Выбрать тип бонуса
            // Выюрать один из элементов в powerUpFrequency
            int ndx = Random.Range(0,powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            // Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            pu.SetType(puType);

            // Поместить в место, где находился разрушенный корабль
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;
        // Записать в bndCheck ссылку на компонент BoundsCheck этого игрового объекта
        bndCheck = GetComponent<BoundsCheck>();
        // Вызывать SpawnEnemy() один раз (в 2 секунды по умолчанию)
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

        // Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions) {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy() 
    {
        // Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // Разместить вражеский корабль над экраном в случайной позиции х
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyPadding = Mathf.Abs( go.GetComponent<BoundsCheck>().radius);
        }

        // Установить начальные координаты созданного вражеского корабля
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range( xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        // Снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
    }

    public void DelayedRestart (float delay) {
        // Вызвать метод Restart() через delay секунд
        Invoke("Restart", delay);
    }

    public void Restart() {
        // Перезапустить _Scene_0, чтобы перезапустить игру
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Статическая функция, возвращающая WeaponDefinition из статического
    /// защищенного поля WEAP_DICT класса Main/
    /// </summary>
    /// <returns>
    /// Экземпляр WeaponDefinition или, если нет такого определения для указанного
    /// WeaponType, возвращает новый экземпляр WeaponDefinition с типом none.
    /// </returns>
    /// <param name = "wt">
    /// Тип оружия WeaponType, для которого требуется получить WeaponDefinition
    /// </param>

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        // Проверить наличие указанного ключа в словаре
        // Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
        // поэтому следующая функция играет важную роль
        if (WEAP_DICT.ContainsKey(wt)) {
            return(WEAP_DICT[wt]);
        }
        // Следующая инструкция возвращает новый экземпляр WeaponDefinition
        // c типом оружия WeaponType.none, что означает неудачную попытку
        // найти требуемое определение WeaponDefinition
        return(new WeaponDefinition());
    }
}
