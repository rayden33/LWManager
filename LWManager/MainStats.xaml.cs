using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LWManager
{
    /// <summary>
    /// Логика взаимодействия для MainStats.xaml
    /// </summary>
    public partial class MainStats : Window
    {
        ApplicationContext dataBaseAC;

        public MainStats(ApplicationContext dbap)
        {
            InitializeComponent();
            dataBaseAC = dbap;

            GenerateOrdersPlot();
            GeneratePaymentsPlot();
            CalculateLabelContent();

        }

        private void GenerateOrdersPlot()
        {
            List<double> dates = new List<double>();
            List<double> ordersCount = new List<double>();
            SortedDictionary<double, double> keyValuePairs = new SortedDictionary<double, double>();
            DateTime tmpDateTime = new DateTime();
            foreach (ArchiveLeaseContract contract in dataBaseAC.ArchiveLeaseContracts)
            {
                //dates.Add();
                tmpDateTime = UnixTimeStampToDateTime(contract.Create_datetime);
                tmpDateTime = new DateTime(tmpDateTime.Year, tmpDateTime.Month, 1);

                //keyValuePairs[tmpDateTime.ToOADate()] = (keyValuePairs[tmpDateTime.ToOADate()] == null ? 0 : keyValuePairs[tmpDateTime.ToOADate()]) + 1;
                if(!keyValuePairs.ContainsKey(tmpDateTime.ToOADate()))
                        keyValuePairs[tmpDateTime.ToOADate()] = 1;
                else
                    keyValuePairs[tmpDateTime.ToOADate()] = keyValuePairs[tmpDateTime.ToOADate()] + 1;

                //MainOrderPlot.Plot.AddPoint(tmpDateTime.ToOADate(), keyValuePairs[tmpDateTime.ToOADate()]);
            }
            

            double[] dts = keyValuePairs.Keys.ToArray();
            double[] values = keyValuePairs.Values.ToArray();
            var bar = OrderPlot.Plot.AddBar(values, dts);
            OrderPlot.Plot.XAxis.DateTimeFormat(true);

            // define tick spacing as 1 day (every day will be shown)

            bar.BarWidth = (30) * .8;
            //OrderPlot.Plot.XAxis.ManualTickSpacing(1, ScottPlot.Ticks.DateTimeUnit.Month);
            //OrderPlot.Plot.XAxis.TickLabelStyle(rotation: 45);

            OrderPlot.Plot.SetAxisLimits(yMin: 0);
            OrderPlot.Plot.Layout(right: 20);
            // add some extra space for rotated ticks
            //OrderPlot.Plot.XAxis.SetSizeLimit(min: 50);
            OrderPlot.Plot.Title("Аренды");

            OrderPlot.Refresh();

        }

        private void GeneratePaymentsPlot()
        {
            List<double> dates = new List<double>();
            List<double> ordersCount = new List<double>();
            SortedDictionary<double, double> keyValuePairs = new SortedDictionary<double, double>();
            DateTime tmpDateTime = new DateTime();
            foreach (Payment payment in dataBaseAC.Payments)
            {
                //dates.Add();
                tmpDateTime = UnixTimeStampToDateTime(payment.Datetime);
                tmpDateTime = new DateTime(tmpDateTime.Year, tmpDateTime.Month, 1);

                //keyValuePairs[tmpDateTime.ToOADate()] = (keyValuePairs[tmpDateTime.ToOADate()] == null ? 0 : keyValuePairs[tmpDateTime.ToOADate()]) + 1;
                if (!keyValuePairs.ContainsKey(tmpDateTime.ToOADate()))
                    keyValuePairs[tmpDateTime.ToOADate()] = payment.Amount;
                else
                    keyValuePairs[tmpDateTime.ToOADate()] = keyValuePairs[tmpDateTime.ToOADate()] + payment.Amount;

                //MainOrderPlot.Plot.AddPoint(tmpDateTime.ToOADate(), keyValuePairs[tmpDateTime.ToOADate()]);
            }


            double[] dts = keyValuePairs.Keys.ToArray();
            double[] values = keyValuePairs.Values.ToArray();
            var bar = PaymentPlot.Plot.AddBar(values, dts);
            PaymentPlot.Plot.XAxis.DateTimeFormat(true);

            bar.BarWidth = (30) * .8;

            // define tick spacing as 1 day (every day will be shown)
            /*PaymentPlot.Plot.XAxis.ManualTickSpacing(1, ScottPlot.Ticks.DateTimeUnit.Month);
            PaymentPlot.Plot.XAxis.TickLabelStyle(rotation: 45);*/

            // add some extra space for rotated ticks
            PaymentPlot.Plot.XAxis.SetSizeLimit(min: 50);
            PaymentPlot.Plot.Title("Оплаты");

            PaymentPlot.Refresh();

        }

        private void CalculateLabelContent()
        {
            int inPaymentAmount = 0;
            int outPaymentAmount = 0;
            foreach(Payment payment in dataBaseAC.Payments)
            {
                if(payment.Amount > 0)
                    inPaymentAmount += payment.Amount;
                else
                    outPaymentAmount += payment.Amount;

            }
            outPaymentAmount *= -1;
            inPaymentAmountLbl.Content = inPaymentAmount;
            outPaymentAmountLbl.Content = outPaymentAmount;
            inPaymentAmountLbl.ContentStringFormat = "N0";
            outPaymentAmountLbl.ContentStringFormat = "N0";
        }

        private void LoadFromSQLiteDB()
        {
            dataBaseAC = new ApplicationContext();
            dataBaseAC.LeaseContracts.Load();
            dataBaseAC.Clients.Load();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}