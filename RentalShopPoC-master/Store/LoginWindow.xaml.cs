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
using Store.DTOs;

namespace Store
{

    
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Context _context;

        private List<CustomerNameIdDto> _customers;
        public LoginWindow()
        {
            InitializeComponent();

          var _context = new Context();

          var allCustomers = _context.Customers.AsNoTracking().ToList();

          _customers = StoreMapper.projectMapper.Map<List<CustomerNameIdDto>>(allCustomers);

          foreach (var c in _customers)
           peopleListBox.Items.Add(c.Name);

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
