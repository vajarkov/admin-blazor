
namespace admin_blazor.Models;

public class DictionaryWord
{
    public int Id { get; set; }

    public string EnglishWord { get; set; } = string.Empty;

    public string RussianTranslation { get; set; } = string.Empty;
}
