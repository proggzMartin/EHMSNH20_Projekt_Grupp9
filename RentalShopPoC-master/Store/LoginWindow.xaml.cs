using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        private Context _context = new Context();

        private const string LOGINERRORMESSAGE = "Angiven användare eller lösenord felaktigt";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            Customer inputUser;
            try
            {
                inputUser = GetEnteredCustomer();
            } catch(ArgumentException ex) //doesn't catch Exception; should be program error.
            {
                _DisplayError(ex.Message);
                return;
            }

            if (inputUser.Password.Equals(PasswordField.Password))
            {
                State.User = inputUser;

                var next_window = new MainWindow();
                next_window.Show();
                Close();
            }
            else
                _DisplayError();
    
        }


        private void _DisplayError(string message = LOGINERRORMESSAGE)
        {
            ErrorLabel.Content = LOGINERRORMESSAGE;
            UserNameField.Text = "";
            PasswordField.Password = "";
        }



        /// <summary>
        /// Throws exception.
        /// </summary>
        private Customer GetEnteredCustomer()
        {
            var foundUsers = _context.Customers
                                        .Where(x => x.UserEmail.Equals(UserNameField.Text));

            if (foundUsers == null)
                throw new ArgumentException(LOGINERRORMESSAGE);

            if (foundUsers.Count() == 0)
                throw new ArgumentException(LOGINERRORMESSAGE);

            if (foundUsers.Count() > 1)
                throw new Exception("Fel i databas; fler än 1 användare med samma key.");

            return foundUsers.First();
        }

    }
}
