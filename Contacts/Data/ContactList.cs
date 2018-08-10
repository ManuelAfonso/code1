namespace Code1.Contacts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Code1.Common.Log;

    public class ContactList
    {
        private readonly ObservableCollection<Contact> contactCollection;
        private bool hasChanges = false;

        public ContactList()
        {
            this.contactCollection = new ObservableCollection<Contact>();
            this.contactCollection.CollectionChanged += this.ContactCollection_CollectionChanged;
        }

        public bool HasChanges { get => this.hasChanges; }

        public void AcceptChanges()
        {
            this.hasChanges = false;
        }

        public void ClearAndFill(IEnumerable<Contact> origin)
        {
            this.contactCollection.CollectionChanged -= this.ContactCollection_CollectionChanged;

            this.contactCollection.Clear();
            foreach (var elem in origin)
            {
                this.contactCollection.Add(elem);
            }

            this.contactCollection.CollectionChanged += this.ContactCollection_CollectionChanged;
        }

        private void ContactCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Logger.Info("Collection:" + e.Action.ToString());
            this.hasChanges = true;
        }
    }
}
