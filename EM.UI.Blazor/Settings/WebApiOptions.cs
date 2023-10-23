namespace EM.UI.Blazor.Settings;

// TODO Переписать класс с нормальными именами переменных
public class WebApiOptions
{
    public string? Ip { get; set; }
    public string? SearchPath { get; set; }
    public string? GetNewOffers { get; set; }
    public string? GetNewSalesOffers { get; set; }
    public string? GetAllCategories { get; set; }
    public string? GetCityFromIp { get; set; }
    public string? GetAllCities { get; set; }
    public string? GetOfferById { get; set; }
    public string? GetUserById { get; set; }
    public string? UpdateOffer { get; set; }
    public string? GetOffersByOrganizationId { get; set; }
    public string? AddReserve { get; set; }
    public string? GetReservesByOfferId { get; set; }
}