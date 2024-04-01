using System.ComponentModel.DataAnnotations;

class FolderExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        if (value is string path && Directory.Exists(path))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult($"The folder '{value}' does not exists.");
    }
}
