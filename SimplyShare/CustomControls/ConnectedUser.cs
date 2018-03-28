using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace SimplyShare.CustomControls
{
    public class ConnectedUser : Button
    {
        //private Button mainButton;
        private StackPanel sp;
        private Ellipse ellipse;
        private ImageBrush ellipseFiller;
        private TextBlock generalità;
        private string nome, cognome;
        IPEndPoint remoteEP;

        public string Nome { get { return nome; } set { nome = value; } }
        public string Cognome { get { return cognome; } set { cognome = value; } }
        public IPEndPoint RemoteEP { get { return remoteEP; } set { remoteEP = value; } }

        public bool isSelected { get; set; }

        public ConnectedUser(string nome, string cognome,IPEndPoint ip,BitmapImage profilePic)
        {
            Nome = nome;            
            Cognome = cognome;
            remoteEP = ip;
            sp = new StackPanel();
            sp.VerticalAlignment = VerticalAlignment.Top;
            sp.Width = 120;
            sp.Height = 130;
            this.Height = sp.Height;
            this.Width = sp.Width;            
            ellipse = new Ellipse();
            ellipse.Width = 100;
            ellipse.Height = 100;
            ellipse.Fill = new ImageBrush(profilePic);
            ellipse.Stretch = Stretch.UniformToFill;
            ((ImageBrush)ellipse.Fill).Stretch = Stretch.UniformToFill;
            generalità = new TextBlock();
            generalità.Text = $"{nome} {cognome}";
            generalità.HorizontalAlignment = HorizontalAlignment.Center;
            sp.Children.Add(ellipse);
            sp.Children.Add(generalità);
            this.Content = sp;
            this.BorderBrush = Brushes.Transparent;
            this.Background = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
            this.Margin = new Thickness(5);
        }







        /*private Button MainButton;
        public bool Selected = false;

        public event EventHandler<RoutedEventArgs> UserClicked;
        static ConnectedUser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectedUser), new FrameworkPropertyMetadata(typeof(ConnectedUser)));
        }

        public override void OnApplyTemplate()
        {
            MainButton = GetTemplateChild(nameof(MainButton)) as Button;
            MainButton.Click += (s, e) => UserClicked?.Invoke(this, e);
        }


        public Brush UserBackground
        {
            get { return (Brush)GetValue(UserBackgroundProperty); }
            set { SetValue(UserBackgroundProperty, value); }
        }
        public static readonly DependencyProperty UserBackgroundProperty =
            DependencyProperty.Register("UserBackground", typeof(Brush), typeof(ConnectedUser), new PropertyMetadata(Brushes.Transparent));

        public string Nome
        {
            get { return (string)GetValue(NomeProperty); }
            set { SetValue(NomeProperty, value); }
        }
        public static readonly DependencyProperty NomeProperty =
            DependencyProperty.Register("Nome", typeof(string), typeof(ConnectedUser), new PropertyMetadata(""));
        

        public string Cognome
        {
            get { return (string)GetValue(CognomeProperty); }
            set { SetValue(CognomeProperty, value); }
        }
        public static readonly DependencyProperty CognomeProperty =
            DependencyProperty.Register("Cognome", typeof(string), typeof(ConnectedUser), new PropertyMetadata(""));

        public ImageSource UserPicSource
        {
            get { return (ImageSource)GetValue(UserPicSourceProperty); }
            set { SetValue(UserPicSourceProperty, value); }
        }
        public static readonly DependencyProperty UserPicSourceProperty =
            DependencyProperty.Register("UserPicSource", typeof(ImageSource), typeof(ConnectedUser), null);

    */
        





    }
}
