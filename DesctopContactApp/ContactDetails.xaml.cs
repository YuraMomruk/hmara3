using DesctopContactApp.Classes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Windows;

namespace DesctopContactApp
{
    /// <summary>
    /// Interaction logic for ContactDetails.xaml
    /// </summary>
    public partial class ContactDetails : Window
    {
        Contact contact;
        private static readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=khrystynacontacts;AccountKey=2DoXiUK4hs507MexxgkOYZDz5bLXSvbH85Vr+PDkqM9xJ/ZlD0DUc++vOXtUu6600p0mbdqAlU8A+AStiu4O2g==;EndpointSuffix=core.windows.net";
        public ContactDetails(Contact contact)
        {

            InitializeComponent();

            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.contact = contact;
            nameTextBox.Text = contact.Name;
            phoneTextBox.Text = contact.Phone;
            addressTextBox.Text = contact.Address;
            SurNameTextBox.Text = contact.SurName;
            PatronymicTextBox.Text = contact.Patronymic;

        }

        private bool checkInput()
        {
            return (nameTextBox.Text.Length > 0) && (SurNameTextBox.Text.Length > 0) &&
                (PatronymicTextBox.Text.Length > 0) && (addressTextBox.Text.Length > 0) && (phoneTextBox.Text.Length > 0);
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkInput())
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("contact");

                await table.CreateIfNotExistsAsync();

                TableOperation retrieveOperation = TableOperation.Retrieve<Contact>(contact.PartitionKey, contact.RowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                Contact existingContact = (Contact)result.Result;


                existingContact.Name = nameTextBox.Text;
                existingContact.SurName = SurNameTextBox.Text;
                existingContact.Patronymic = PatronymicTextBox.Text;
                existingContact.Address = addressTextBox.Text;
                existingContact.Phone = phoneTextBox.Text;


                string contactJson = JsonConvert.SerializeObject(existingContact);


                var contactEntity = new DynamicTableEntity(existingContact.PartitionKey, existingContact.RowKey);
                contactEntity.Properties.Add("Contact", EntityProperty.GeneratePropertyForString(contactJson));


                TableOperation updateOperation = TableOperation.InsertOrReplace(contactEntity);
                await table.ExecuteAsync(updateOperation);


                if (Owner is MainWindow mainWindow)
                {
                    mainWindow.ReadDatabase();
                }
            }
            Close();


        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("contact");

            await table.CreateIfNotExistsAsync();

            TableOperation retrieveOperation =
                TableOperation.Retrieve<Contact>(contact.PartitionKey, contact.RowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            Contact existingContact = (Contact)result.Result;



            TableOperation deleteOperation = TableOperation.Delete(existingContact);
            await table.ExecuteAsync(deleteOperation);


            if (Owner is MainWindow mainWindow)
            {
                mainWindow.ReadDatabase();
            }
            Close();

        }
    }
}
