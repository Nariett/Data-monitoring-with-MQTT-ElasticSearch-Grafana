using Nest;
namespace CRUD
{
    class Program//create/read/update/delete data in ElasticSearch
    {
        static async Task Main(string[] args)
        {   
            int c = 0;
            var setting = new ConnectionSettings(new Uri("http://localhost:9200/"));
            var client = new ElasticClient(setting);
            while (c == 0)
            {
                Console.WriteLine("Select item:\n1 - Create index\n2 - Read index\n3 - Update index\n4 - Delete index\n5 - Exit ");
                int select = Convert.ToInt32(Console.ReadLine());
                switch (select)
                {
                    case 1://create index
                        {
                          
                            var createIndexResponse = client.Indices.Create("testindex", c => c//create a "testindex" index for the class "engine"
                                .Settings(s => s
                                    .NumberOfShards(1)
                                    .NumberOfReplicas(0)
                                )
                                 .Map<Engine>(m => m.AutoMap())
                            );
                            break;
                        }
                    case 2://Read index
                        {
                            var response = await client.GetAsync<Engine>(1, idx => idx.Index("testindex"));
                            var engine = response.Source;
                            Console.WriteLine(engine.ToString());
                            break;
                        }
                    case 3://update index
                        {
                            Engine test = new Engine(1);
                            var response = await client.UpdateAsync<Engine, object>(1, u => u
                                                                            .Index("testindex")
                                                                            .Doc(test));
                            if (response.IsValid)
                            {
                                Console.WriteLine("Update document succeeded.");
                            }
                            break;
                        }
                    case 4://delete index
                        {
                            var deleteIndexResponse = client.Indices.Delete("detector");
                            if (!deleteIndexResponse.IsValid)
                            {
                                Console.WriteLine("Error deleting index: " + deleteIndexResponse.DebugInformation);
                            }
                            else
                            {
                                Console.WriteLine("Index deleted successfully");
                            }
                            break;
                        }
                    case 5://exit
                        {
                            c = 1;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Input Error");
                            break;
                        }
                }
            }
        }
    }
}