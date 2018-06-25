using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Utf8Json;

namespace AzureFunctionsIntroduction.Teams
{
    public class TeamsMessage
    {
        public string title { get; set; }
        public string text { get; set; }
        public Potentialaction[] potentialAction { get; set; }
    }

    public class Potentialaction
    {
        [DataMember(Name = "@context")]
        public string context { get; set; } = "http://schema.org";
        [DataMember(Name = "@type")]
        public string type { get; set; } = "ViewAction";
        public string name { get; set; }
        public string[] target { get; set; }
    }
}
