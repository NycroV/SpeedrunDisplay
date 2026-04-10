
namespace SpeedrunDisplay.DataStructures;

public record class Category(string LocalizationKey, Split CompletionSplit)
{
    public static implicit operator (string localizationKey, Split completionSplit)(Category category) => (category.LocalizationKey, category.CompletionSplit);
}