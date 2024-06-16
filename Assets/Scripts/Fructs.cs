using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class Fructs : MonoBehaviour
{
    Image _image;
    Transform _transform;
    [SerializeField] Image _gameObjectImage;
    float speed = 1400;
    float deltaSpeed;
    bool isMove = false;
    bool isGoMove = true;
    WaitForSeconds wait = new WaitForSeconds(0.045f);

    public Image Image => _image;

    public bool isMoving => isMove;

    private bool canAnimate = true;

    private void Awake()
    {
        _image = _gameObjectImage.GetComponent<Image>();
        deltaSpeed = Screen.width / 1920f;
    }

    public void Create(Sprite sprite, Transform transform, Vector2 size)
    {
        (_image.transform as RectTransform).sizeDelta = size * 100;
        _image.sprite = sprite;
        _transform = transform;
    }

    /// <summary>
    /// �������� ��� ������� ���������� (����� �������� ��, ��� y����� ������������������ �� 2 ���)
    /// </summary>
    public IEnumerator CombinationAnimationAndDisable(bool isBomb = false)
    {
        // yield return MoveTo(transform.position + Vector3.up * 40+ Vector3.right*50, 0.5f);
        // yield return MoveTo(transform.position + Vector3.up * 40 - Vector3.right * 50, 0.5f);
        // yield return MoveTo(transform.position - Vector3.up * 40, 0.3f);
        // yield return MoveTo(transform.position, 0.3f);
        // yield return RotateAndScaleTo(720, Vector2.zero, 0.4f);

        //  _gameObjectImage.transform.DOShakePosition(3f, 1, 10, 90);

        if (isBomb)
        {
            yield return _gameObjectImage.rectTransform.DOScale(3, 0.1f).SetEase(Ease.OutExpo).WaitForCompletion();
            yield return _gameObjectImage.rectTransform.DOScale(0, 0.1f).SetEase(Ease.Linear).WaitForCompletion();
        }
        else
        {
            yield return _gameObjectImage.rectTransform.DORotate(new Vector3(0, 0, 60), 0.25f).SetEase(Ease.Linear).WaitForCompletion();
            yield return _gameObjectImage.rectTransform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Linear).WaitForCompletion();
            yield return _gameObjectImage.rectTransform.DOScale(0, 1f).WaitForCompletion();
        }



        _gameObjectImage.transform.position = transform.position;
        _gameObjectImage.transform.rotation = Quaternion.identity;
        _gameObjectImage.transform.localScale = transform.localScale;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �������� ��� ������� fructs (������ ��������)
    /// </summary>
    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        canAnimate = true;
    }

    private void OnEnable()
    {
        _gameObjectImage.transform.localScale = Vector3.one + Vector3.up * 0.2f;
    }

    private IEnumerator RotateAndScaleTo(float angle, Vector2 scale, float delay)
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

        for (int i = 0; i < delay * 16; i++)
        {
            _gameObjectImage.transform.position = (Vector3)Vector2.Lerp(_gameObjectImage.transform.position, point,
                                                                        1 / delay / 16);

            yield return wait;
        }

    }
    /// <summary>
    /// ����� Fructs - ��� ������ ����������� �����, ������� ����y�� �� �������. isMov� - ������������, ��� ������ �� ��������y
    /// </summary>
    public void Update()
    {
        if (Vector3.Distance(transform.position, _transform.position) > 20 * deltaSpeed && isGoMove)
        {
            transform.position += (_transform.position - transform.position).normalized * speed * deltaSpeed * Time.deltaTime;
            _gameObjectImage.transform.position = transform.position;
            isMove = true;
        }
        else
        {
            isMove = false;
            EndGraviAnimation();
            transform.position = _transform.position;
        }
    }

    async void EndGraviAnimation()
    {
        if (!canAnimate)
            return;

        canAnimate = false;
        await Task.Delay(10);

        await _gameObjectImage.transform.DOScaleY(0.7f, 0.2f).SetEase(Ease.Flash).AsyncWaitForCompletion();
        _gameObjectImage.transform.DOScaleY(1f, 0.2f);
    }
}
