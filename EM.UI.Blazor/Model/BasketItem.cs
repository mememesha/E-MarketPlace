using EM.Dto;

namespace EM.UI.Blazor.Model;

public class BasketItem
{
    public OfferShortResponseDto? Item { get; set; }
    public int Count { get; set; }
}