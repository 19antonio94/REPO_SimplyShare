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

namespace SimplyShare.CustomControls
{
    public class ConnectedUser : Control
    {
        private Button MainButton;
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








    }
}
