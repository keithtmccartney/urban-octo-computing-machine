using Emitter;
using Newtonsoft.Json;
using System;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Models;

namespace urban_octo_computing_machine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Connect to emitter
            var emitter = Connection.Establish();

            // Set up your credentials (https://apps.twitter.com)
            Auth.SetUserCredentials(
                Environment.GetEnvironmentVariable("CONSUMER_KEY"),
                Environment.GetEnvironmentVariable("CONSUMER_SECRET"),
                Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
                Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")
                );

            // Setup a rate limiter, only re-publish one tweet per 350 milliseconds
            var limiter = new Throttle(1, TimeSpan.FromMilliseconds(350));

            // Using the sample stream
            var stream = Stream.CreateSampleStream();

            stream.AddTweetLanguageFilter(LanguageFilter.English);
            stream.FilterLevel = Tweetinvi.Streaming.Parameters.StreamFilterLevel.Low;
            stream.TweetReceived += (sender, t) =>
            {
                // Skip retweets
                if (t.Tweet.IsRetweet)
                    return;

                // Limiter allows us to have a steady, visually pleasing rate of tweets
                var ct = new CancellationToken();

                limiter.Enqueue(() =>
                {
                    // Publish the tweet to the broker
                    emitter.Publish(
                        "IsQ7z18uGEFpjOJpt4K6ij49klT3PGzu",
                        "tweet-stream",
                        JsonConvert.SerializeObject(new
                        {
                            avatar = t.Tweet.CreatedBy.ProfileImageUrl400x400,
                            text = t.Tweet.Text
                        }));
                }, ct);
            };

            // Start
            stream.StartStream();
        }
    }
}
