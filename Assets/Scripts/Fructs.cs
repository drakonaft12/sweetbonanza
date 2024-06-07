using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fructs : MonoBehaviour
{
    Image _image;
    Transform _transform;
    [SerializeField]GameObject _gameObjectImage;
    float speed = 40;
    bool isMove = false;
    bool isGoMove = true;
    WaitForSeconds wait = new WaitForSeconds(0.045f);

    public Image Image => _image;

    public bool isMoving => isMove;
    private void Awake()
    {
        _image = _gameObjectImage.GetComponent<Image>();
        
    }

    public void Create(Sprite sprite, Transform transform, Vector2 size)
    {
        (_image.transform as RectTransform).sizeDelta = size*100;
        _image.sprite = sprite;
        _transform = transform;
    }

    public IEnumerator Destroy() 
    {
        yield return MoveTo(transform.position + Vector3.up * 40+ Vector3.right*50, 0.5f);
        yield return MoveTo(transform.position + Vector3.up * 40 - Vector3.right * 50, 0.5f);
        yield return MoveTo(transform.position - Vector3.up * 40, 0.3f);
        yield return MoveTo(transform.position, 0.3f);
        yield return RotateAndScaleTo(720, Vector2.zero, 0.4f);

        _gameObjectImage.transform.position = transform.position;
        _gameObjectImage.transform.rotation = Quaternion.identity;
        _gameObjectImage.transform.localScale = transform.localScale;
        gameObject.SetActive(false);
    }

    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator RotateAndScaleTo(float angle,Vector2 scale,  float delay)
    {
        for (int i = 0; i < delay * 16; i++)
        {
            var thisAngles = _gameObjectImage.transform.rotation.eulerAngles.z;
            var r = Mathf.Lerp(thisAngles, angle, 1 / delay / 16);
            _gameObjectImage.transform.Rotate(Vector3.forward, r - thisAngles);
            _gameObjectImage.transform.localScale = (Vector3)Vector2.Lerp(_gameObjectImage.transform.localScale, scale,
                                                                        1 / delay / 16);
            yield return wait;
        }
    }
    
    private IEnumerator MoveTo(Vector2 point, float delay)
    {
        
        for (int i = 0; i < delay*16; i++)
        {
            _gameObjectImage.transform.position = (Vector3)Vector2.Lerp(_gameObjectImage.transform.position, point, 
                                                                        1 / delay / 16);

            yield return wait;
        }
        
    }
    public void Update()
    {
        if (Vector3.Distance(transform.position , _transform.position)>20 && isGoMove)
        {
            transform.position += (_transform.position - transform.position).normalized * speed;
            _gameObjectImage.transform.position = transform.position;
            isMove = true;
        }
        else { isMove = false; }
    }
}
