using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YNL.Bases;
using YNL.Extensions.Methods;
using YNL.Utilities.Addons;

public class CapacityPopperUI : MonoBehaviour
{
    [SerializeField] private Image _popping;
    [SerializeField] private int _poppingAmount = 30;

    private List<(Image Image, RectTransform Rect)> _poppingUIs = new();

    [SerializeField] private SerializableDictionary<ResourceType, RectTransform> _poppingDestinations = new();

    private void Awake()
    {
        Player.OnReturnCapacity += OnReturnCapacity;
    }

    private void OnDestroy()
    {
        Player.OnReturnCapacity -= OnReturnCapacity;
    }

    private void Start()
    {
        CreatePoppings();
    }

    private void CreatePoppings()
    {
        this.transform.DestroyAllChildren();
        _poppingUIs.Clear();
        for (int i = 0; i < _poppingAmount; i++)
        {
            Image popping = Instantiate(_popping, this.transform);
            _poppingUIs.Add(new(popping, popping.GetComponent<RectTransform>()));
            popping.transform.localScale = Vector3.zero;
        }
    }

    private void OnReturnCapacity((ResourceType type, int amount)[] resources)
    {
        if (resources.Length <= 0) return;

        int totalAmount = resources.Sum(i => i.amount);
        int maxImages = Mathf.Min(totalAmount, _poppingAmount);

        int index = 0;
        float delay = 0;

        int imagesPerType = 0;

        if (totalAmount >= maxImages)
        {
            index = 0;
            delay = 0;

            foreach (var resource in resources)
            {
                imagesPerType = Mathf.Min(maxImages, resource.amount);

                for (int i = 0; i < imagesPerType; i++)
                {
                    if (index >= maxImages) break;

                    AnimatePopping(_poppingUIs[index], resource.type);

                    index++;
                    delay += 0.1f;

                    if (index >= maxImages) break;
                }
            }
        }
        else
        {
            index = 0;
            delay = 0;

            foreach (var resource in resources)
            {
                for (int i = 0; i < resource.amount; i++)
                {
                    if (index >= _poppingAmount) break;

                    AnimatePopping(_poppingUIs[index], resource.type);

                    index++;
                    delay += 0.1f;

                    if (index >= _poppingAmount) break;
                }

                if (index >= _poppingAmount) break;
            }
        }

        void AnimatePopping((Image Image, RectTransform Rect) image, ResourceType type)
        {
            image.Image.sprite = Game.Data.Vault.ResourceIcons[type];

            image.Rect.anchoredPosition = new Vector2(Random.Range(-75f, 75f), Random.Range(-75f, 75f));

            image.Rect.DOScale(1, 0.3f).SetDelay(delay).SetEase(Ease.OutCubic);
            image.Rect.DOAnchorPos(_poppingDestinations[type].anchoredPosition.AddY(400), 1).SetDelay(delay + 0.5f).SetEase(Ease.OutCubic);
            image.Rect.DOScale(0, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutCubic);
        }
    }
}