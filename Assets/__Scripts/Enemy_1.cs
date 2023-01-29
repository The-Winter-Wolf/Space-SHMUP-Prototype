using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy_1 расширяет класс Enemy
public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    // Число секунд полного цикла синусоиды
    public float            waveFrequency = 2f;
    // Ширина синусоиды в метрах
    public float            waveWidth = 4f;
    public float            waveRotY = 45f;

    private float           x0; // Начальное значение координаты Х
    private float           birthTime;

    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    // Переопределить функцию Move суперкласса Enemy
    public override void Move() 
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // Повернуть немного относительно оси Х
        Vector3 rot = new Vector3(0, sin*waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move(); // Обрабатывает движение вниз, вдоль оси Y

        // print (bndCheck.isOnScreen);
    }
}
