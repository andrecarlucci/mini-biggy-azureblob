namespace MiniBiggy {
    public static class CreatListOfAzureBlobExt {
        public static IChooseOptions<T> SavingOnAzureBlob<T>(this CreateListOf<T> list) where T : new() {
            return new CreateOnAzureBlobStorageBuilder<T>();
        }
    }
}
