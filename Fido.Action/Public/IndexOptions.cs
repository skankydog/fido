using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fido.Action
{
    public class IndexOptions
    {
        public string Echo { get; set; } // Sequence number
        public string Filter { get; set; } // Typed into the search textbox
        public int Take { get; set; } // Number of items to display on the page
        public int Skip { get; set; } // First record number to display
        public int NumberOfColumns { get { return Columns.Count; } }
        public IList<string> Columns { get; set; } // Column names

        public IList<string> SortColumns; // When sorted on more than one column
        public IList<string> SortOrders; // When sorted on more than one column

        public int SortColumn { get { return Convert.ToInt32(SortColumns[0]); } }
        public char SortOrder { get { return SortOrders[0].ToLower()[0]; } }
    }
}
