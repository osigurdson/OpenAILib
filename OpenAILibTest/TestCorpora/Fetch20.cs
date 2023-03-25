using OpenAILib.FineTuning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAILib.Tests.TestCorpora
{
    internal class Fetch20
    {
        public static List<FineTunePair> CreateHockeyBaseballBlogTitles()
        {
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

            return trainingData;
        }
    }
}
