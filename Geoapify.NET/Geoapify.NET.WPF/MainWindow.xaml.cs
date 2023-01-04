using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Geoapify.NET.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new WindowViewModel();
            InitializeComponent();

            
        }
    }


    public class WindowViewModel : BindableBase
    {

        private bool needsOpen = false;
        public bool NeedsOpen
        {
            get { return needsOpen; }
            set { SetProperty(ref needsOpen, value); }
        }

        private ObservableCollection<Address> addresses = new ObservableCollection<Address>();
        public ObservableCollection<Address> Addresses
        {
            get { return addresses; }
            set { SetProperty(ref addresses, value); }
        }

        private Address selectedAddress;
        public Address SelectedAddress
        {
            get { return selectedAddress; }
            set { SetProperty(ref selectedAddress, value); }
        }

        private string query;
        public string Query
        {
            get { return query; }
            set { SetProperty(ref query, value); }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public WindowViewModel()
        {

            // add the license file to the solution and add the license key to it
            apikey = System.IO.File.ReadAllText("license.txt");
            this.PropertyChanged += WindowViewModel_PropertyChanged;
           
        }

        

        string apikey;

        private async void WindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Query))
            {
                if (Query.EndsWith("United States of America")) 
                    return;

                IsBusy = true; 

                if (string.IsNullOrWhiteSpace(Query))
                    Addresses.Clear();

                var addresses = await AutocompleteHelper.GetAddressAsync(query, apikey);

                if (addresses != null)
                {
                    var toBeRemovedAddresses = Addresses.Where(x => addresses.Select(j => j.formatted).Contains(x.formatted) == false).ToArray();
                    var toBeAddedAddresses = addresses.ToList().Where(x => Addresses.Select(j => j.formatted).Contains(x.formatted) == false).ToArray();


                    foreach (var toBeRemovedAddresse in toBeRemovedAddresses)
                        Addresses.Remove(toBeRemovedAddresse);

                    toBeAddedAddresses.ToList().ForEach(x => Addresses.Add(x));


                    IsBusy = false;
                }


              
            }

            if (this.IsBusy == false)
                NeedsOpen = this.Addresses.Count == 0 ? false : true;
        }
    }
}
