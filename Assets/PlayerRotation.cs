using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Joystick joystick;

    void Update()
    {
        // branie inputa z joysticka
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // gracz kr�ci si� tylko jak joystick jest ruszany
        if (horizontal != 0 || vertical != 0)
        {
            // obliczenie kr�cenia gracza
            float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg - 90f;

            // kr�� gracza
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
