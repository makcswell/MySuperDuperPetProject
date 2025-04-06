using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MyDatabase;

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffsetConverter()
        : base(
            d => d.ToUniversalTime(),
            d => d.ToUniversalTime())
    {
    }
}