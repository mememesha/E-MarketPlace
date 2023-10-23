using System.ComponentModel;
using Blazored.LocalStorage;
using EM.Dto;
using EM.UI.Blazor.Model;

namespace EM.UI.Blazor.Services;

public class BasketService
{
    private readonly ILocalStorageService _localStorage;
    public List<BasketItem> Items { get; set; } = new();
    public event Action? AddedOrRemove;

    public BasketService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        // InitializeAsync();
    }

    // public async Task InitializeAsync()
    // {
    //     Items = await _localStorage.GetItemAsync<List<BasketItem>>("EM.BasketService.BasketItems");
    //     // var userOffers = await _userService.GetUser();
    // }

    public decimal TotalCost => Items.Sum(i => i.Item!.CostOfUnit * i.Count);
    public List<OfferShortResponseDto?> ToOfferShortResponseDtoList => Items.Select(i => i.Item).ToList();
    public int Count
    {
        get
        {
            var result = 0;
            if (Items.Count > 0)
            {
                result += Items.Sum(item => item.Count);
            }
            return result;
        }
    }

    public async Task AddAsync(OfferShortResponseDto? offer)
    {
        if (offer == null)
            return;

        if (IsIncluded(offer))
            Items.First(bi => bi.Item!.Id == offer.Id).Count++;
        else
            Items.Add(new BasketItem { Item = offer, Count = 1 });

        await _localStorage.SetItemAsync("EM.BasketService.BasketItems", Items);
        AddedOrRemove!.Invoke();
    }

    public async Task RemoveAsync(OfferShortResponseDto? offer)
    {
        if (offer == null)
            return;

        if (IsIncluded(offer))
        {
            var item = Items.First(bi => bi.Item!.Id == offer.Id);

            if (item.Count > 1)
                item.Count--;
            else
                Items.Remove(item);
        }

        await _localStorage.SetItemAsync("EM.BasketService.BasketItems", Items);
        AddedOrRemove!.Invoke();

    }

    public async Task Clear()
    {
        Items.Clear();
        await _localStorage.RemoveItemAsync("EM.BasketService.BasketItems");
        AddedOrRemove!.Invoke();
    }

    private bool IsIncluded(OfferShortResponseDto offer)
        => Items.Any(bi => bi.Item!.Id == offer.Id);
}