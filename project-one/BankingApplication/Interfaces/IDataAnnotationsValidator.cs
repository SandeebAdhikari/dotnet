namespace BankingApp.Interfaces
{
    public interface IDataAnnotationsValidator
    {
        void Validate(object instance);

        void ValidatePropertyValue<T>(
            string propertyName,
            object? value);
    }
}
