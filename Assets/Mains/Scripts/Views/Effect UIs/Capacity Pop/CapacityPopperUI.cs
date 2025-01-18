using DG.Tweening;
using Sirenix.Utilities;
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

        if (totalAmount <= 0) return;

        int index = 0;
        float delay = 0;

        var resourceImages = resources.Select(r => new { r.type, imageCount = Mathf.RoundToInt((float)r.amount / totalAmount * maxImages) }).ToList();

        var shuffledImages = new List<(ResourceType type, int order)>(); 
        foreach (var resource in resourceImages) 
        { 
            for (int i = 0; i < resource.imageCount; i++) 
            {
                shuffledImages.Add((resource.type, index)); index++; 
            } 
        }
        shuffledImages = shuffledImages.OrderBy(x => Random.value).ToList();

        index = 0;
        delay = 0;

        foreach (var shuffledImage in shuffledImages) 
        { 
            if (index >= _poppingUIs.Count) break; 
            
            AnimatePopping(_poppingUIs[index], shuffledImage.type); 

            index++; 
            delay += 0.05f; 

            if (index >= _poppingUIs.Count) break; 
        }

        void AnimatePopping((Image Image, RectTransform Rect) image, ResourceType type)
        {
            image.Image.sprite = Game.Data.Vault.ResourceIcons[type];

            image.Rect.anchoredPosition = new Vector2(Random.Range(-75f, 75f), Random.Range(-75f, 75f));

            image.Rect.DOScale(1, 0.3f).SetDelay(delay).SetEase(Ease.OutCubic);
            image.Rect.DOAnchorPos(_poppingDestinations[type].anchoredPosition.AddY(400), 1).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            image.Rect.DOScale(0, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutCubic);
        }
    }
}