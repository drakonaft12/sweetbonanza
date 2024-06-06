using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PageBase : MonoBehaviour
{
    private Image[] allTargetGraphic;
    private TextMeshProUGUI[] allTargetText;
    [SerializeField] private float animationDuration = 2;
    [SerializeField] private GraphicRaycaster[] raycaster;



    public async virtual Task StartScreen()
    {
        gameObject.SetActive(true);
        allTargetGraphic = GetComponentsInChildren<Image>();
        allTargetText = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var item in raycaster) item.enabled = false;
        Color color;
        foreach (var item in allTargetGraphic)
        {
            color = item.color;
            color.a = 0;
            item.color = color;
        }
        foreach (var item in allTargetText)
        {
            color = item.color;
            color.a = 0;
            item.color = color;
        }
        foreach (var item in allTargetGraphic)
        {
            DOFade(item, 1, animationDuration);
        }
        foreach (var item in allTargetText)
        {
            DOFade(item, 1, animationDuration);
        }
        await Task.Delay((int)(animationDuration * 1000));
        foreach (var item in raycaster) item.enabled = true;

    }

    public async Task CloseScreen()
    {
        foreach (var item in raycaster) item.enabled = false;
        foreach (var item in allTargetGraphic)
        {
            DOFade(item, 0, animationDuration);
        }

        foreach (var item in allTargetText)
        {
            DOFade(item, 0, animationDuration);
        }

        await Task.Delay((int)(animationDuration * 1000));
        gameObject.SetActive(false);
        foreach (var item in raycaster) item.enabled = true;
    }

    private async void DOFade(Image image, float alfa, float animationDuration)
    {
        float foot;
        if (image.color.a < alfa)
            foot = Mathf.Lerp(image.color.a, alfa, 1 / (animationDuration * 16));
        else foot = -Mathf.Lerp(alfa, image.color.a, 1 / (animationDuration * 16));
        Color color = image.color;
        for (int i = 0; i < animationDuration * 16; i++)
        {
            color.a += foot;
            image.color = color;

            await Task.Delay(45);
        }
        color.a = alfa;
        image.color = color;
    }
    private async void DOFade(TextMeshProUGUI image, float alfa, float animationDuration)
    {
        float foot;
        if (image.color.a < alfa)
            foot = Mathf.Lerp(image.color.a, alfa, 1 / (animationDuration * 16));
        else foot = -Mathf.Lerp(alfa, image.color.a, 1 / (animationDuration * 16));
        Color color = image.color;
        for (int i = 0; i < animationDuration * 16; i++)
        {
            color.a += foot;
            image.color = color;

            await Task.Delay(45);
        }
        color.a = alfa;
        image.color = color;
    }
}
