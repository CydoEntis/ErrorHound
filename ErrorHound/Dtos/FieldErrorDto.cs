namespace ErrorHound.Dtos;

/// <summary>
/// Represents a single field error for API responses.
/// </summary>
public class FieldErrorDto
{
    /// <summary>
    /// The name of the field that has errors.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// The list of error messages for this field.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();
}