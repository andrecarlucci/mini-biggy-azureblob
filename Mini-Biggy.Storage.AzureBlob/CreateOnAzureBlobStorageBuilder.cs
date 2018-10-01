namespace MiniBiggy {
    public class CreateOnAzureBlobStorageBuilder<T> : IChooseOptions<T>,
                                                      IChooseConnectionString<T>,
                                                      IChooseKeepingLatest<T>,
                                                      IChooseTable<T> where T : new() {
        private string _connectionString;
        private int _keepingLatest = 100;

        public IChooseTable<T> WithConnectionString(string connectionString) {
            _connectionString = connectionString;
            return this;
        }
        public IChooseConnectionString<T> KeepingLatest(int versionsToKeep) {
            _keepingLatest = versionsToKeep;
            return this;
        }

        public IChooseSerializer<T> SavingOnTable(string tableName) {
            var dataStore = new AzureBlobStorage(_connectionString, tableName, _keepingLatest);
            return new CreateListOfBuilder<T>(dataStore);
        }
    }

    public interface IChooseOptions<T> where T : new() {
        IChooseTable<T> WithConnectionString(string connectionString);
        IChooseConnectionString<T> KeepingLatest(int versionsToKeep = 100);
    }

    public interface IChooseConnectionString<T> where T : new() {
        IChooseTable<T> WithConnectionString(string connectionString);
    }

    public interface IChooseTable<T> where T : new() {
        IChooseSerializer<T> SavingOnTable(string tableName);
    }

    public interface IChooseKeepingLatest<T> where T : new() {
        IChooseConnectionString<T> KeepingLatest(int versionsToKeep = 100);
    }
}
