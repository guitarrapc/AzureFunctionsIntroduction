using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionsIntroduction.Domains.Entities
{
    public class AssetEntity : TableEntity
    {
        public string AssetName { get; set; }
    }
}
