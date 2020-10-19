using System.Collections.Generic;

namespace Playground.Mvc.Models
{
    public class Person
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool LikesMusic { get; set; }
        public ICollection<string> Skills { get; set; }
    }
}