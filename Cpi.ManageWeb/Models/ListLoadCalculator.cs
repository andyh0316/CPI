namespace Cpi.ManageWeb.Models
{
    public class ListLoadCalculator
    {
        public int LoadedRecords { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public ListLoadCalculator(int loads, int total, int? take = 50)
        {
            Total = total;
            Take = take.Value;
            Skip = loads * take.Value;
            LoadedRecords = Skip + take.Value;
        }
    }
}