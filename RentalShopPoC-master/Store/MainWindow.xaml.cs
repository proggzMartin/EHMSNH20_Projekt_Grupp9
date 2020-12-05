using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DatabaseConnection;

namespace Store
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Knapp kan ha 3 lägen; uthyrd, hyra, vald.
        public enum MovieSelection
        {
            Hyr, Vald, Uthyrd
        }
        private class MovieDto
        {
            public MovieSelection Status { get; private set; } = MovieSelection.Hyr;
            public Movie TargetMovie { get; set; }
            public MovieDto(Movie movie, Customer activeCustomer)
            {
                TargetMovie = movie;

                if(IsRented(activeCustomer))
                {
                    Status = MovieSelection.Uthyrd;
                }
            }
            public bool IsRented(Customer activeCustomer)
            {
                var x = TargetMovie.Sales.Any(x => x.Customer.UserEmail.Equals(activeCustomer.UserEmail));
                return x;
            }

            public void SetStatus()
            {
                if (!IsRented(State.User))
                    if (Status == MovieSelection.Hyr)
                        Status = MovieSelection.Vald;
                    else
                        Status = MovieSelection.Hyr;
            }
        }


        private const int NUMOFCOLS = 4;
        private const int NUMOFROWS = 6;

        

        private List<MovieDto> movies = new List<MovieDto>();
        private List<MovieDto> selectedMovies = new List<MovieDto>();

        public MainWindow()
        {
            InitializeComponent();

            GreetText.Text = $"Välkommen {State.User.FirstName}";


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
                            if (movie.IsRented(State.User))
                                button.IsEnabled = false;

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

        private void _rentMovies(Movie[] movies)
        {
            throw new NotImplementedException();
        }


        private const int LISTSPACING = 20;
        private const int LEFTMARGIN = 20;
        private const int SELECTEDMOVIELISTSTARTY = 120;
        private const string MOVIEIDPREFIX = "ID";

        private void ButtonClicked(object sender, MouseButtonEventArgs e) 
        {
            if(sender is Button)
            {
                var buttonParentStackPanel = (sender as Button).Parent;
                var x = Grid.GetColumn(buttonParentStackPanel as UIElement);
                var y = Grid.GetRow(buttonParentStackPanel as UIElement);

                //int i = y * MovieGrid.ColumnDefinitions.Count + x;
                var selectedMovie = movies[y * MovieGrid.ColumnDefinitions.Count + x];


                //Behöver kolla om redan vald.
                //Behöver kunna plocka bort.
                //Behöver se till så redan hyrda inte går att klicka.
                selectedMovie.SetStatus();

                (sender as Button).Content = selectedMovie.Status.ToString();
                selectedMovies.Add(selectedMovie);

                ChosenMoviesStack.Children.Add(new TextBox() { 
                    Name = MOVIEIDPREFIX+selectedMovie.TargetMovie.Id.ToString(), //Will be used for removing movie.
                    Text = selectedMovie.TargetMovie.Title
                });
            }
        }
    }
}
