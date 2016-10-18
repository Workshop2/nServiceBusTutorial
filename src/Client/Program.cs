using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Client
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            // This makes it easier to tell console windows apart
            Console.Title = "Samples.StepByStep.Client";

            // The endpoint name will be used to determine queue names and serves as the address, or identity, of the endpoint
            var endpointConfiguration = new EndpointConfiguration(endpointName: "Samples.StepByStep.Client");
            
            endpointConfiguration.SendFailedMessagesTo("error");

            // Use JSON to serialize and deserialize messages (which are just plain classes) to and from message queues
            endpointConfiguration.UseSerialization<JsonSerializer>();

            // Ask NServiceBus to automatically create message queues
            endpointConfiguration.EnableInstallers();

            // Store information in memory for this example, rather than in a database. In this sample, only subscription information is stored
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            // Initialize the endpoint with the finished configuration
            var endpointInstance = await Endpoint
                                            .Start(endpointConfiguration)
                                            .ConfigureAwait(false);
            try
            {
                await SendOrder(endpointInstance);
            }
            finally
            {
                await endpointInstance
                                    .Stop()
                                    .ConfigureAwait(false);
            }
        }
    }
}
