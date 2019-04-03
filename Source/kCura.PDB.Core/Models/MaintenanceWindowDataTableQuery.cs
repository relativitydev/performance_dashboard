namespace kCura.PDB.Core.Models
{
    public class MaintenanceWindowDataTableQuery
    {
        public string StartTimeFilter { get; set; }

        public string EndTimeFilter { get; set; }

        public string ReasonFilter { get; set; }

        public string CommentFilter { get; set; }

        public FilterOperand StartTimeOperator { get; set; }

        public FilterOperand EndTimeOperator { get; set; }

        public FilterOperand ReasonOperator { get; set; }

        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public string sEcho { get; set; }

        public int SortIndex { get; set; }

        public string SortColumn { get; set; }

        public bool SortDirectionDesc { get; set; }
    }
}
