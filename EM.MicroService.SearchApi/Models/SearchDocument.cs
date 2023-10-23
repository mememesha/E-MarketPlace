using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EM.MicroService.SearchApi.Models;

public class SearchDocument
{
    // TODO: Вынести в общий класс с моделями
    /// <summary>
    /// Документ в виде Json, который будем парсить в завсимости от категории (предложение, продавец)
    /// </summary>
    public string Document { get; set; }
    public string Category { get; set; }
    public string Location { get; set; }
}