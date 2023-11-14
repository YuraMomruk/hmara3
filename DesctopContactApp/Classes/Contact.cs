using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DesctopContactApp.Classes
{
    [Serializable]
    public class Contact : TableEntity
    {

        public string Name { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public string PhotoURl { get; set; }
    }

}
