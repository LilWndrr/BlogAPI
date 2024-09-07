namespace BlogAPI.HelperServices;

public class ServiceResult<T>
{
    public T? Data { get; set; } // Nullable to handle cases where data might not be provided
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; } // Nullable for error message

    // Default constructor
    public ServiceResult() { }

    // Constructor for success result
    public ServiceResult(T data, bool success)
    {
        Data = data;
        Success = success;
        ErrorMessage = success ? null : string.Empty; // If success, no error message
    }

    // Constructor for failure result
    public ServiceResult(string errorMessage)
    {
        Data = default;
        Success = false;
        ErrorMessage = errorMessage;
    }

    // Method to create a successful service result
    public static ServiceResult<T> SuccessResult(T data) => new ServiceResult<T>(data, true);

    // Method to create a failed service result
    public static ServiceResult<T> FailureResult(string errorMessage) => new ServiceResult<T>(errorMessage);
}
