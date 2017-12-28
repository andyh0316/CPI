using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cpi.Application.DataTransferObjects
{
    public class FinanceDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        //public decimal Profit { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expense { get; set; }
        public int CallsReceived { get; set; }
        public int ProductsSold { get; set; }
        public int ProductsCancelled { get; set; }
        public int ProductsPending { get; set; }
        public int ProductsTotal { get; set; }
        public int InvoicesSold { get; set; }
        public int InvoicesCancelled { get; set; }
        public int InvoicesPending { get; set; }
        public int InvoicesTotal { get; set; }
    }
}
