namespace Code1.Contacts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using global::Code1.Common.Persistance;
    using Newtonsoft.Json;

    // next step: replace the file operations with a FileOperationsClass, there is no need to go more abstract than that
    public class Repo
    {
        private readonly ContactList contactList;
        private readonly string repoPath;
        private readonly IObjectSerializer objectSerializer;

        private bool readOnly = false;
        private DateTime lastChange;

        public bool ReadOnly { get => this.readOnly; }

        public ContactList ContactList => this.contactList;

        public static Repo Get(string repoPath, IObjectSerializer objectSerializer)
        {
            if (!File.Exists(repoPath))
            {
                File.WriteAllText(repoPath, string.Empty);
            }

            return new Repo(repoPath, objectSerializer);
        }

        private Repo(string repoPath, IObjectSerializer objectSerializer)
        {
            this.objectSerializer = objectSerializer;

            this.contactList = new ContactList();

            this.repoPath = repoPath;
            this.ThrowIfRepoDoesNotExists();

        }

        public void Open()
        {
            this.ThrowIfRepoDoesNotExists();

            string text = File.ReadAllText(this.repoPath);
            var collection = this.objectSerializer.DeserializeObject<ObservableCollection<Contact>>(text);

            this.contactList.ClearAndFill(collection);

            FileInfo fileInfo = new FileInfo(this.repoPath);
            if (fileInfo.IsReadOnly)
            {
                this.readOnly = true;
            }

            this.lastChange = fileInfo.LastWriteTime;
            this.contactList.AcceptChanges();
        }

        public void Save()
        {
            this.ThrowIfRepoDoesNotExists();

            FileInfo fileInfo = new FileInfo(this.repoPath);

            if (fileInfo.IsReadOnly)
            {
                throw new InvalidOperationException("The file can not be saved because is read only.");
            }

            if (this.lastChange < fileInfo.LastWriteTime)
            {
                throw new InvalidOperationException("The file can not be saved because it was modified since it was read.");
            }

            string text = JsonConvert.SerializeObject(this.contactList, Formatting.Indented);

            File.WriteAllText(this.repoPath, text);

            this.lastChange = DateTime.Now;
            this.contactList.AcceptChanges();
        }

        private void ThrowIfRepoDoesNotExists()
        {
            if (!File.Exists(this.repoPath))
            {
                throw new FileNotFoundException("The repo file doesn't exist.", this.repoPath);
            }
        }
    }
}
