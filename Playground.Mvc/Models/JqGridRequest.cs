using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Playground.Mvc.Models
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    [DataContract]
    public class JqGridRequest
    {

        public JqGridRequest()
        {
            Page = 1;
            PerPage = 25;
        }

        [DataMember]
        public int Page { get; set; }

        [DataMember]
        public int PerPage { get; set; }


        [DataMember]
        public string SearchTerm { get; set; }

        [DataMember]
        public List<KeyValuePair<string, string>> SearchPropertiesAndTerms { get; set; }

        [DataMember]
        public string SearchProperty { get; set; }

        [DataMember]
        public string SortByProperty { get; set; }

        public SortOrder SortOrder { get; set; }
    }
}