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

namespace Sandbox
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


        public WindowViewModel()
        {

            // add the license file to the solution and add the license key to it
            apikey = System.IO.File.ReadAllText("license.txt");
            
        }

        

        string apikey;

        
    }
}
