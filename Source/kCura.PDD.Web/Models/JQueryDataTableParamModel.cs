using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models
{
	public class JQueryDataTableParamModel
	{
		public string iDisplayStart; //Display start point in the current data set
		public string iDisplayLength; //Number of records that the table can display in the current draw
		public int iColumns; //Number of columns being displayed
		public bool[] bSearchable; //Indicates if columns are flagged as searchable on the client-side
		public string[] sSearch; //Individual column filters
		public bool[] bRegex; //True if the column filter should be treated as a regular expression
		public bool[] bSortable; //Indicates whether column is flagged as sortable on the client-side
		public int iSortingCols; //Number of columns to sort on
		public int[] iSortCol; //Column being sorted on (decode)
		public string[] sSortDir; //Direction to be sorted
		public string[] mDataProp; //Value specified by mDataProp for each column
		public string sEcho;
	}
}