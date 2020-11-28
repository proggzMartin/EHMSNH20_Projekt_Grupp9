using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DatabaseConnection;
using Microsoft.EntityFrameworkCore;

namespace Store
{

    
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private class CustomerDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        Context _context;

        List<CustomerDto> namesAndIds = new List<CustomerDto>();

        public LoginWindow()
        {
            InitializeComponent();
            _context = new Context();

            //namesAndIds = _context.Customers.Select(x => x.Name && x.Id)
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {

            //PROJ ORIGINAL CODE
            //State.User = API.GetCustomerByName(NameField.Text.Trim());
            //if (State.User != null)
            //{
            //    var next_window = new MainWindow();
            //    next_window.Show();
            //    this.Close();
            //}
            //else
            //{
            //    NameField.Text = "...";
            //}
        }
    }
}
