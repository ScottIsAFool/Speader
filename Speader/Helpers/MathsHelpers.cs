using System;

namespace Speader.Helpers
{
    public static class MathsHelpers
    {
        public static TimeSpan TimeToReadArticle(double milliSeconds, int wordcount, int wordsAtATime)
        {
            var totalTime = (wordcount * milliSeconds) / wordsAtATime;
            var time = TimeSpan.FromMilliseconds(totalTime);
            return time;
        }

        public static double DisplayTimePerWordGroup(int wordcount, int wordsPerMin, int wordsAtATime)
        {
            var milliseconds = (60 / (double)wordsPerMin);
            return (milliseconds * 1000) * wordsAtATime;
        }

        public static int RoundedWordsPerMin(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
