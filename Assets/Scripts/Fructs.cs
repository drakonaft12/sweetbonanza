using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Fructs : MonoBehaviour
{
    Image _image;
    Transform _transform;
    [SerializeField]GameObject _gameObjectImage;
    float speed = 30;
    bool isMove = false;
    bool isGoMove = true;
    WaitForSeconds wait = new WaitForSeconds(0.045f);

    public Image Image => _image;

    public bool isMoving => isMove;
    private void Awake()
    {
        _image = _gameObjectImage.GetComponent<Image>();
        
    }

    public void Create(Sprite sprite, Transform transform)
    {
        _image.sprite = sprite;
        _transform = transform;
    }

    public IEnumerator Destroy() 
    {
        yield return MoveTo(transform.position + Vector3.up * 10+ Vector3.right*10, 0.5f);
        yield return MoveTo(transform.position + Vector3.up * 10 - Vector3.right * 10, 0.5f);
        yield return MoveTo(transform.position + Vector3.up * 10, 0.5f);
        yield return MoveTo(transform.position, 0.5f);
        gameObject.SetActive(false);
    }

    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveTo(Vector2 point, float delay)
    {
        var p = (Vector3)Vector2.Lerp(_gameObjectImage.transform.position, point, 1 / delay /16) - _gameObjectImage.transform.position;
        for (int i = 0; i < delay*16; i++)
        {
            _gameObjectImage.transform.position += p;

            yield return wait;
        }
        _gameObjectImage.transform.position = point;
    }
    public void Update()
    {
        if (transform.position != _transform.position && isGoMove)
        {
            transform.position += (_transform.position - transform.position).normalized * speed;
            _gameObjectImage.transform.position = transform.position;
            isMove = true;
        }
        else { isMove = false; }
    }
}
