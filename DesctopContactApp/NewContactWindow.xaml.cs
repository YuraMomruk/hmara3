using DesctopContactApp.Classes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Windows;
using Azure.Storage.Blobs;
namespace DesctopContactApp
{
    /// <summary>
    /// Interaction logic for NewContactWindow.xaml
    /// </summary>
    public partial class NewContactWindow : Window
    {
        private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=khrystynacontacts;AccountKey=2DoXiUK4hs507MexxgkOYZDz5bLXSvbH85Vr+PDkqM9xJ/ZlD0DUc++vOXtUu6600p0mbdqAlU8A+AStiu4O2g==;EndpointSuffix=core.windows.net";
        private static string _imagescontainer = "images";

        public NewContactWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private bool checkInput()
        {
            return (nameTextBox.Text.Length > 0) && (surnameTextBox.Text.Length > 0) &&
                (PatronymicTextBox.Text.Length > 0) && (addressTextBox.Text.Length > 0) && (phoneTextBox.Text.Length > 0);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            if (checkInput())
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);

                var tableClient = storageAccount.CreateCloudTableClient();

                var table = tableClient.GetTableReference("contact");

                await table.CreateIfNotExistsAsync();


                Contact contact = new Contact()
                {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = Guid.NewGuid().ToString(),
                    Name = nameTextBox.Text,
                    SurName = surnameTextBox.Text,
                    Patronymic = PatronymicTextBox.Text,
                    Address = addressTextBox.Text,
                    Phone = phoneTextBox.Text

                };



                string contactJson = JsonConvert.SerializeObject(contact);


                var contactEntity = new DynamicTableEntity(contact.PartitionKey, contact.RowKey);
                contactEntity.Properties.Add("Contact", EntityProperty.GeneratePropertyForString(contactJson));



                var insertOperation = TableOperation.InsertOrReplace(contactEntity);
                await table.ExecuteAsync(insertOperation);

                if (Owner is MainWindow mainWindow)
                {
                    mainWindow.ReadDatabase();
                }
            }
            Close();
        }

    }
}
