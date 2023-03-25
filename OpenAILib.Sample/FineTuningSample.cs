using OpenAILib.FineTuning;

namespace OpenAILib.Sample
{
    internal static class FineTuningSample
    {
        public static async Task Sample01Async()
        {
            // Create the client
            var organizationId = Environment.GetEnvironmentVariable("OpenAI_OrganizationId");
            var apiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey");
            var client = new OpenAIClient(organizationId: organizationId, apiKey: apiKey);

            // Create some training data
            // This is far too little training data to create a robust model but will train quickly (< 5 minutes) / cheaply (<$0.01)
            // and provide a walkthrough of how fine tuning works - see https://platform.openai.com/docs/guides/fine-tuning for more details.
            // Samples below are from fetch20_newgroups (as discussed on the openai link above)

            // The nice thing about this approach is you can source your data from anywhere (Excel file, csv, database, service, etc.). It is not necessary
            // to deal with jsonl formatting, prompt suffixes, completion suffixes and stop information. A content-addressing strategy is utilized meaning that
            // the same training data will not be uploaded more than once
            var trainingData = new List<FineTunePair>
            {
                // hockey examples
                new (prompt: "NHL: Oilers trade Anderson to Leafs", completion: "hockey"),
                new (prompt: "Hockey Equipment for Sale", completion: "hockey"),
                new (prompt: "NHL: Senators fire head coach", completion: "hockey"),
                new (prompt: "Re: NHL All-Rookie Team", completion: "hockey"),
                new (prompt: "Re: Rookie Hockey player wins tournament", completion: "hockey"),
                new (prompt: "Re: New Hockey Trivia Question", completion: "hockey"),
                new (prompt: "Re: NHL Playoff Predictions", completion: "hockey"),
                new (prompt: "Re: College Hockey: NCAA Tourney", completion: "hockey"),
                new (prompt: "Re: NHL Team to Oklahoma City?", completion: "hockey"),
                new (prompt: "Re: Hockey Pool Update", completion: "hockey"),

                // baseball examples
                new (prompt: "MLB: Angels trade for Justin Upton", completion: "baseball"),
                new (prompt: "Baseball Card Values", completion: "baseball"),
                new (prompt: "MLB: Yankees sign Gerrit Cole to record deal", completion: "baseball"),
                new (prompt: "Re: 1996 Yankees vs 2004 Red Sox", completion: "baseball"),
                new (prompt: "Re: NL East: Mets vs. Braves", completion: "baseball"),
                new (prompt: "Re: Red Sox Dynasty?", completion: "baseball"),
                new (prompt: "Re: Yankees/Diamondbacks Game 7", completion: "baseball"),
                new (prompt: "Re: Baseball Trivia Question", completion: "baseball"),
                new (prompt: "Re: MLB Playoff Predictions", completion: "baseball"),
                new (prompt: "Re: College Baseball: NCAA Tourney", completion: "baseball")
            };

            // Create the fine tune
            var fineTunedModel = await client.FineTuning.CreateFineTuneAsync(trainingData);

            // Wait for it to complete - this can take several minutes
            await foreach (var evt in client.FineTuning.GetEventStreamAsync(fineTunedModel))
            {
                Console.WriteLine(evt.Message);
            }

            Console.WriteLine("Write a blog post title about hockey or baseball, we will try to classify it - type 'done' to quit");
            string question;
            while ((question = Console.ReadLine()) != "done")
            {
                var fineTunedCompletion = await client.GetCompletionAsync(question, spec => spec.Model(fineTunedModel));
                Console.WriteLine(fineTunedCompletion);
            }

            // Delete the model and associated training data when done
            await client.FineTuning.DeleteFineTuneAsync(fineTunedModel, deleteTrainingFiles: true);

            // Notes
            // 1. The resulting object 'fineTunedModel' can be serialized to json and saved to a file/db/service and used at a later time to create completions, or check the status of the fine tune.
            // 2. It is possible to create a variant of the original fine tune using CreateFineTuneVariantAsync using a different model or parameters. Training data does not need
            //    to be uploaded again and multiple variants can be created simultaneously
        }
    }
}
