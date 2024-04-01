using System.ComponentModel.DataAnnotations;

class FileExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        if (value is string path && File.Exists(path))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult($"The file '{value}' is not found.");
    }
}
