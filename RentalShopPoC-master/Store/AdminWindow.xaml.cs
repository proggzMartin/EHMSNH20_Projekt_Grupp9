﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static DataGrid datagrid;
        Context db = new Context();
       

        public AdminWindow()
        {
            InitializeComponent();
            PopulateCustomerData();

        }

        
      private void Button_Add(object sender, RoutedEventArgs e)
        {
            
            var _name = CustomerName.Text;
            var _last_name = LastName.Text;
            var _password = Password.Text;



            if (_name.Length <= 2 || _last_name.Length <=2 || _password.Length <= 2)
            {
                MessageBox.Show("Too Short Name,LastName or Password try again!", "The Database says no!", MessageBoxButton.OK, MessageBoxImage.Information);    
            }
            else 
            {
                using (Context db = new Context())
                {
                    Customer customer = new Customer()
                    {
                        FirstName = _name,
                        LastName = _last_name,
                        Password = _password
                    };
                    {
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        CustomerGrid.ItemsSource = db.Customers.ToList();
                       

                    }

                }
            }
        }
           
        

        private void PopulateCustomerData()
        {

           
                CustomerGrid.ItemsSource = db.Customers.ToList();
                datagrid = CustomerGrid;



        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            // har du hyrt mera filmer går det inte att ta bort dig, men har du en film är det kört!
            try
            {

                int Id = (CustomerGrid.SelectedItem as Customer).Id;
                var deleteCustomer = db.Customers.Where(c => c.Id == Id).Single();
                db.Customers.Remove(deleteCustomer);
                db.SaveChanges();
                CustomerGrid.ItemsSource = db.Customers.ToList();
            }
            catch
            {
               
                    int Id2 = (CustomerGrid.SelectedItem as Customer).Id;
                    var deleteCustomer2 = db.Sales.Where(s => s.Customer.Id == Id2).Single();           
                    db.Sales.Remove(deleteCustomer2);
                    db.SaveChanges();
                    CustomerGrid.ItemsSource = db.Customers.ToList();

            }
            

        }

        private void Button_Loginwindow(object sender, RoutedEventArgs e)
        {
            LoginWindow loginw = new LoginWindow();
            loginw.Show();
            this.Close();
            
        }
    }
}
