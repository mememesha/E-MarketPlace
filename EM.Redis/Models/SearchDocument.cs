namespace EM.Redis.Models;

public class SearchDocument
{
    // TODO: Вынести в общий класс с моделями
    public Guid DocumentId { get; set; }
    
    /// <summary>
    /// Документ в виде Json, который будем парсить в завсимости от категории (предложение, продавец)
    /// </summary>
    public string Document { get; set; }
    public string Category { get; set; }
    public string Location { get; set; }
    public bool IsDeleted { get; set; }
}