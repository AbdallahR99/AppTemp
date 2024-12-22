namespace AppTemp.Core.Options;

public static class OptionValidationExtenstion
{
    public static IEnumerable<ValidationResult> ValidateKeys<T>(this T option, params List<dynamic?> keysToCheck) where T : IOptionsRoot
    {
        var missingValues = keysToCheck.Where(x => string.IsNullOrEmpty(x)).ToList();
        if (missingValues.Count != 0)
        {
            foreach (var missingValue in missingValues)
            {
                yield return new ValidationResult($"No {missingValue} defined in {nameof(T)} config", [missingValue]);
            }
        }

    }
}
