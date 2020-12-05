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



        private List<Movie> _chosenMovies = new List<Movie>();
        public MainWindow()
        {
            InitializeComponent();

            //StackPanel s = new StackPanel();

            //var x = s.Children.Add(new Label();

            GreetText.Text = $"Välkommen {State.User.FirstName}";

            State.Movies = API.GetMovieSlice(0, 40);

            for (int y = 1; y < NUMOFROWS; y++)
            {
                for (int x = 1; x <= NUMOFCOLS; x++)
                {
                    int i = y * MovieGrid.ColumnDefinitions.Count + x;
                    if (i < State.Movies.Count)
                    {
                        var movie = State.Movies[i];

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
                                Source = new BitmapImage(new Uri(movie.ImageURL)),
                                Margin = new Thickness(4, 4, 4, 4)
                            };
                            image.Height = 140;


                            //Define button properties
                            var button = new Button() { Content = "Hej" };
                            button.Width = 60;
                            button.PreviewMouseUp += UIElementClicked<Button>;


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


        private void UIElementClicked<T>(object sender, MouseButtonEventArgs e) where T : FrameworkElement
        {
            if(sender is T)
            {
                var stackPanel = (sender as T).Parent;

                var x = Grid.GetColumn(stackPanel as UIElement);
                var y = Grid.GetRow(stackPanel as UIElement);

                int i = y * MovieGrid.ColumnDefinitions.Count + x;
                State.Pick = State.Movies[i];

                if (API.RegisterSale(State.User, State.Pick))
                    MessageBox.Show("All is well and you can download your movie now.", "Sale Succeeded!", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("An error happened while buying the movie, please try again at a later time.", "Sale Failed!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
