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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    /// 

    
    public partial class AdminWindow : Window
    {
       

        public AdminWindow()
        {
            InitializeComponent();

            PopulateCustomerData();

        }

        
      private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var _name = CustomerName.Text;
            var _last_name = LastName.Text;
            var _password = Password.Text;

            if (_name.Length < 2 || _last_name.Length <2 || _password.Length < 2)
            {

                MessageBox.Show("Too Short Name,LastName or Password try again!", "The Database says no!", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            else if (_name.Length > 2 || _last_name.Length > 2 || _password.Length > 2)
            {
                using (Context db = new Context())
                {
                    Customer customer = new Customer()
                    {
                        Name = _name,
                        LastName = _last_name,
                        Password = _password
                    };


                    {
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        PopulateCustomerData();
                    }

                }
            }
        }
           
        

        private void PopulateCustomerData()
        {

            using (Context db = new Context())
            {
                var CustomerData = db.Customers.ToList();
                foreach (var item in CustomerData)
                {
                    CustomerList.Items.Add(item);
                }


            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
