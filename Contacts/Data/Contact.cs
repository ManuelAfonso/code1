namespace Code1.Contacts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Contact
    {
        public bool PhoneList { get; set; }

        public string Name { get; set; }

        public string Reference { get; set; }

        public List<string> AddressLines { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Email { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public HashSet<string> Tags { get; set; }

        public List<string> NotesLines { get; set; }
    }
}
