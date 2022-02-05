using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertisseurMontant
{
    public class data
    {
        public string base_currency_id { get; set; }
        public string base_currency_name { get; set; }
        public string base_price_last_updated { get; set; }
        public string quote_currency_id { get; set; }
        public string quote_currency_name { get; set; }
        public string quote_price_last_updated { get; set; }
        public string amount { get; set; }
        public string price { get; set; }
    }
}
