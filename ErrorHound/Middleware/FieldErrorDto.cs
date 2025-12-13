namespace ErrorHound.Middleware
{
    /// <summary>
    /// Represents a single field error for API responses.
    /// </summary>
    public class FieldErrorDto
    {
        public string Field { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}