using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace ConvertisseurMontant
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MontantBinding();
        }

        public void clear()
        {
            resultLBL.Content = "";
            montantTB.Text = "";
            montantTB.Focus();
        }
        public static HttpClient Apiclient { get; set; }

        public static async Task<string> loadData(string base_id, string quote_id, string montant)
        {
            string url = $"https://api.coinpaprika.com/v1/price-converter?base_currency_id={base_id}&quote_currency_id={quote_id}&amount={montant}";
            Apiclient = new HttpClient();
            //Apiclient.BaseAddress = new Uri("https://api.coinpaprika.com/v1/");
            Apiclient.DefaultRequestHeaders.Accept.Clear();
            Apiclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Apiclient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");

            using ( HttpResponseMessage response = await Apiclient.GetAsync(url) )
            {
                if ( response.IsSuccessStatusCode )
                {
                    string res = await response.Content.ReadAsStringAsync();
                    return res;
                }
                else if ( response.StatusCode == HttpStatusCode.NotFound )
                {
                    MessageBox.Show("Page Not Found", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if ( response.StatusCode == HttpStatusCode.BadRequest )
                {
                    MessageBox.Show("invalid parameters", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                /*else if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                {
                    MessageBox.Show("too many requests", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                }*/
                throw new Exception(response.ReasonPhrase);
            }
        }
        private void MontantBinding()
        {
            //combobox1
            DataTable dt = new DataTable();
            dt.Columns.Add("base_currency_id");
            dt.Columns.Add("base_currency_name");

            //add rows
            dt.Rows.Add("btc-bitcoin", "Bitcoin");
            dt.Rows.Add("eur-euro-token", "Euro");
            dt.Rows.Add("ncc-neurochain", "Neurochain");
            
            fromCB.ItemsSource = dt.DefaultView;
            fromCB.DisplayMemberPath = "base_currency_name";
            fromCB.SelectedValuePath = "base_currency_id";
            fromCB.SelectedIndex = 0;

            //combobox2
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("quote_currency_id");
            dt1.Columns.Add("quote_currency_name");

            //add rows
            dt1.Rows.Add("usd-us-dollars", "USD");
            dt1.Rows.Add("eth-ethereum", "Ethereum");
            dt1.Rows.Add("xrp-xrp", "XRP");

            toCB.ItemsSource = dt1.DefaultView;
            toCB.DisplayMemberPath = "quote_currency_name";
            toCB.SelectedValuePath = "quote_currency_id";
            toCB.SelectedIndex = 0;

        }

        private void clearBTN_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void montantTB_TextChanged(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9.]+");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private async void convertBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(montantTB.Text))
                {
                    MessageBox.Show("veuillez entrer le montant", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                    montantTB.Focus();
                    return;
                }
                string jsonData = await loadData(fromCB.SelectedValue.ToString(), toCB.SelectedValue.ToString(), montantTB.Text);
                data jsonString = JsonConvert.DeserializeObject<data>(jsonData);
                resultLBL.Content = jsonString.price.ToString();
            }
            catch(Exception x)
            {
                MessageBox.Show(x.ToString(), "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
