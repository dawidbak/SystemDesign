namespace Notification.Application.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public int CountryCode { get; set; }
    public int PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Device> Devices { get; set; }
    public ICollection<Setting> Settings { get; set; }

    internal User(Guid id, string email, int countryCode, int phoneNumber)
    {
        Id = id;
        Email = email;
        CountryCode = countryCode;
        PhoneNumber = phoneNumber;
        CreatedAt = TimeProvider.System.GetUtcNow().UtcDateTime;
    }
}