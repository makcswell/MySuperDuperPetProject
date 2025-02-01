using System.Security.Claims;

namespace MySuperDuperPetProject.Middle
{
    public class CustomClaimTypesStorage
    {
        public static CustomClaim ConstantClaim { get; private set; } = new("Constant", "SHS");
    }

    public struct CustomClaim(string type, string value)
    {
        public string Type { get; private set; } = type;
        public string Value { get; private set; } = value;
        public readonly Claim Claim { get => new(Type, Value); }
    }
}
