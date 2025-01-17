using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MySuperDuperPetProject.TransferDatabaseContext
{
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
}
