namespace MySuperDuperPetProject.Models
{
    public class TransferStatisticResponseModel(int count, string from, string to)
    {
        public int Count { get; set; } = count;
        public string From { get; set; } = from;
        public string To { get; set; } = to;
    }
}
