namespace MiniBiggy {
    public class CreateOnAzureBlobStorageBuilder<T> : IChooseOptions<T>,
                                                      IChooseConnectionString<T>,
                                                      IChooseKeepingLatest<T>,
                                                      IChooseContainer<T> where T : new() {
        private string _connectionString;
        private int _keepingLatest = 100;

        public IChooseContainer<T> WithConnectionString(string connectionString) {
            _connectionString = connectionString;
            return this;
        }
        public IChooseConnectionString<T> KeepingLatest(int versionsToKeep) {
            _keepingLatest = versionsToKeep;
            return this;
        }

        public IChooseSerializer<T> SavingOnContainerAndFile(string container, string filename) {
            var dataStore = new AzureBlobStorage(_connectionString, container, filename, _keepingLatest);
            return new CreateListOfBuilder<T>(dataStore);
        }
    }

    public interface IChooseOptions<T> where T : new() {
        IChooseContainer<T> WithConnectionString(string connectionString);
        IChooseConnectionString<T> KeepingLatest(int versionsToKeep = 100);
    }

    public interface IChooseConnectionString<T> where T : new() {
        IChooseContainer<T> WithConnectionString(string connectionString);
    }

    public interface IChooseContainer<T> where T : new() {
        IChooseSerializer<T> SavingOnContainerAndFile(string container, string filename);
    }

    public interface IChooseKeepingLatest<T> where T : new() {
        IChooseConnectionString<T> KeepingLatest(int versionsToKeep = 100);
    }
}
