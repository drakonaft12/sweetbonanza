using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fructs : MonoBehaviour
{
    Image _image;
    Transform _transform;
    float speed = 30;
    bool isMove = false;

    public bool isMoving => isMove;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Create(Sprite sprite, Transform transform)
    {
        _image.sprite = sprite;
        _transform = transform;
    }

    public void Update()
    {
        if(transform.position != _transform.position)
        {
        transform.position += (_transform.position-transform.position).normalized*speed;
            isMove = true;
        }
        else { isMove = false; }
    }
}
