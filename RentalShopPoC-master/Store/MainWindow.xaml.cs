using System;
using System.Collections.Generic;
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
using DatabaseConnection;

namespace Store
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int NUMOFCOLS = 4;
        private const int NUMOFROWS = 6;

        private const int LISTSPACING = 20;
        private const int LEFTMARGIN = 20;
        private const int MOVIELISTSTARTY = 120;


        //Knapp kan ha 3 lägen; uthyrd, hyra, vald.
        public enum MovieSelection
        {
            Hyr, Vald, Rented
        }
        private class MovieDto
        {
            public MovieSelection Status { get; set; } = MovieSelection.Hyr;
            public Movie TargetMovie { get; set; }
            public MovieDto(Movie movie, Customer activeCustomer)
            {
                TargetMovie = movie;

                if(_isRented(activeCustomer))
                {
                    Status = MovieSelection.Rented;
                }
            }
            private bool _isRented(Customer activeCustomer)
            {
                return TargetMovie.Sales.Any(x => x.Customer.UserEmail.Equals(activeCustomer.UserEmail));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            GreetText.Text = $"Välkommen {State.User.FirstName}";

            //State.Movies = API.GetMovieSlice(0, 40)

            List<MovieDto> movies = new List<MovieDto>();

            foreach(var m in API.GetMovieSlice(0,40))
            {
                movies.Add(new MovieDto(m, State.User));
            }

            for (int y = 1; y < NUMOFROWS; y++)
            {
                for (int x = 1; x <= NUMOFCOLS; x++)
                {
                    int i = y * MovieGrid.ColumnDefinitions.Count + x;
                    if (i < movies.Count)
                    {
                        var movie = movies[i];
                        try
                        {
                            var stackPanel = new StackPanel()
                            {
                                Orientation = Orientation.Vertical
                            };

                            //Define image properties
                            var image = new Image() {
                                Cursor = Cursors.Hand,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Source = new BitmapImage(new Uri(movie.TargetMovie.ImageURL)),
                                Margin = new Thickness(4, 4, 4, 4)
                            };
                            image.Height = 140;


                            //Define button properties

                            //See if movie is already rented
                            //if(_isRented(movie))
                            //{

                            //}
                            var button = new Button();
                            button.Content = movie.Status.ToString();
                            button.Width = 60;
                            button.PreviewMouseUp += ButtonClicked;


                            stackPanel.Children.Add(image);
                            stackPanel.Children.Add(button);

                            MovieGrid.Children.Add(stackPanel);
                            Grid.SetRow(stackPanel, y);
                            Grid.SetColumn(stackPanel, x);

                        }
                        catch (Exception e) when 
                            (e is ArgumentNullException || 
                             e is System.IO.FileNotFoundException || 
                             e is UriFormatException)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private bool _isRented(Movie movie)
        {
            var x = movie.Sales.Any(x => x.Customer.UserEmail.Equals(State.User.UserEmail));
            return movie.Sales.Any(x => x.Customer.UserEmail.Equals(State.User.UserEmail));
        }

        private void _setMovieStatus(Movie movie)
        {
            
        }

        private void _rentMovies(Movie[] movies)
        {
            throw new NotImplementedException();
        }

        private void ButtonClicked(object sender, MouseButtonEventArgs e) 
        {
            if(sender is Button)
            {

                var stackPanel = (sender as Button).Parent;

                var x = Grid.GetColumn(stackPanel as UIElement);
                var y = Grid.GetRow(stackPanel as UIElement);

                int i = y * MovieGrid.ColumnDefinitions.Count + x;

                State.Pick = State.Movies[i];

                (sender as Button).Content = "Vald";



                //if (API.RegisterSale(State.User, State.Pick))
                //    MessageBox.Show("All is well and you can download your movie now.", "Sale Succeeded!", MessageBoxButton.OK, MessageBoxImage.Information);
                //else
                //    MessageBox.Show("An error happened while buying the movie, please try again at a later time.", "Sale Failed!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        
    }
}
