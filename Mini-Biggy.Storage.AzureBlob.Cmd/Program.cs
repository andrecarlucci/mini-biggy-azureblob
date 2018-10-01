using MiniBiggy;
using MiniBiggy.SaveStrategies;
using MiniBiggy.Serializers;
using System;
using System.Threading.Tasks;

namespace Mini_Biggy.Storage.AzureBlob.Cmd {
    class Program {
        static async Task Main(string[] args) {
            var conn = args.Length > 0 ? args[0] : "";

            if (conn == "") {
                Console.WriteLine("Please, specify the connection string as the first argument");
                return;
            }

            Console.WriteLine("Hello MiniBiggy on SqlServer!");
            var list = Create.ListOf<Tweet>()
                          .SavingOnAzureBlob()
                          .KeepingLatest(100)
                          .WithConnectionString(conn)
                          .SavingOnContainerAndFile("container", "tweets.json")
                          .UsingPrettyJsonSerializer()
                          .BackgroundSavingEveryTwoSeconds();

            list.Saved += (sender, e) => {
                Console.WriteLine($"Saved: {e.Success} Elapsed: {e.TimeToSave}");
            };


            Console.WriteLine("List size: " + list.Count);
            Console.WriteLine("Hit enter to create and save a tweet");
            while (true) {
                var line = Console.ReadLine();
                if (line == "exit") {
                    break;
                }
                list.Add(new Tweet {
                    DateTime = DateTime.Now,
                    Message = line,
                    Id = DateTime.Now.Second
                });
            }
            Console.WriteLine("End");
        }

        public class Tweet {
            public int Id { get; set; }
            public string Message { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
