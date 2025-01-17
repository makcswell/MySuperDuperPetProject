namespace MySuperDuperPetProject.Models
{
    public class TransferResponseModel(string from, string to)
    {
        public string PageFrom { get; set; } = from;
        public string PageTo { get; set; } = to;

    }
}
