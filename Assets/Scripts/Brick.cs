using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[System.Serializable]*/
public class Brick
{
    public float health;                 
    public bool hasBonus = false;
    private GameObject imageInstanse;           // ссылка на изображение этого кирпича (чтобы разрушать)
    public float x;                             // координаты этого кирпича (центра)
    public float y;

    public Brick (float _health, bool _hasBonus, GameObject _image, float _x, float _y)
    {
        imageInstanse = _image;
        x = _x;
        y = _y;
        hasBonus = _hasBonus;
        health = _health;
    }

    public bool TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            GameObject.Destroy(imageInstanse);
            return true;
        }
        else return false;
    }
}
