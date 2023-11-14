using DesctopContactApp.Classes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DesctopContactApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=khrystynacontacts;AccountKey=2DoXiUK4hs507MexxgkOYZDz5bLXSvbH85Vr+PDkqM9xJ/ZlD0DUc++vOXtUu6600p0mbdqAlU8A+AStiu4O2g==;EndpointSuffix=core.windows.net";

        List<Contact> contacts;

        public MainWindow()
        {
            InitializeComponent();
            contacts = new List<Contact>();
            ReadDatabase();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewContactWindow newContactWindow = new NewContactWindow();
            newContactWindow.ShowDialog();
            ReadDatabase();
        }
        public async void ReadDatabase()
        {
            ContactList.ItemsSource = null;

            await Task.Run(async () =>
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("contact");

                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
                TableContinuationToken token = null;

                var newContacts = new List<Contact>();

                do
                {
                    var segment = await table.ExecuteQuerySegmentedAsync(query, token);
                    token = segment.ContinuationToken;

                    foreach (var entity in segment.Results)
                    {
                        if (entity.Properties.TryGetValue("Contact", out var property))
                        {
                            var contactJson = property.StringValue;
                            var contact = JsonConvert.DeserializeObject<Contact>(contactJson);
                            newContacts.Add(contact);
                        }
                    }
                } while (token != null);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    contacts = newContacts;
                    ContactList.ItemsSource = contacts;
                });
            });
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchTextBox = sender as TextBox;
            var filteredList = contacts.Where(c => c.Name.ToLower().Contains(searchTextBox.Text.ToLower())).ToList();
            ContactList.ItemsSource = filteredList;

        }

        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Contact selectedContact = (Contact)ContactList.SelectedItem;

            if (selectedContact != null)
            {
                ContactDetails newContactDetails = new ContactDetails(selectedContact);
                newContactDetails.ShowDialog();
                ReadDatabase();

            }

        }

    }
}
