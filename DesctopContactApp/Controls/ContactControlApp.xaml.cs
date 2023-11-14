using DesctopContactApp.Classes;
using System.Windows;
using System.Windows.Controls;

namespace DesctopContactApp.Controls
{
    /// <summary>
    /// Interaction logic for ContactControlApp.xaml
    /// </summary>
    public partial class ContactControlApp : UserControl
    {




        public Contact Contact
        {
            get { return (Contact)GetValue(ContactProperty); }
            set { SetValue(ContactProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contact.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContactProperty =
            DependencyProperty.Register("Contact", typeof(Contact), typeof(ContactControlApp), new PropertyMetadata(new Contact() { Name = "Name ", Phone = "(123) 456 7980", SurName = "Lastname", Patronymic = "Bohdanovich", Address = "st. Shevchenka " }, SetText));

        private static void SetText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContactControlApp control = d as ContactControlApp;
            if (control != null)
            {
                control.nameTextBlock.Text = (e.NewValue as Contact).Name;
                control.surnameTextBlock.Text = (e.NewValue as Contact).SurName;
                control.PatronymicTextBlock.Text = (e.NewValue as Contact).Patronymic;
                control.phoneTextBlock.Text = (e.NewValue as Contact).Phone;
                control.AddressTextBlock.Text = (e.NewValue as Contact).Address;

            }
        }

        public ContactControlApp()
        {
            InitializeComponent();
        }
    }
}
