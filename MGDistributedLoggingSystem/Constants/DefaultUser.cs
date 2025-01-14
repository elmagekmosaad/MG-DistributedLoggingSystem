using MGDistributedLoggingSystem.Data.Entities.Enums;

namespace MGDistributedLoggingSystem.Constants
{
    public record DefaultUser
    {
        public const string Name = $"{nameof(DefaultUser)}";
        public const string UserName = $"{Name}3535";
        public const string Email = $"{UserName}@mg-control.com";
        public const string Password = $"{UserName}.Asd@123";
        public const string PhoneNumber = Constants.PhoneNumber;
    }
}

