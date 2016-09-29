using System;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core.Schedules
{
    public class Duration : ISchedule
    {
        public Duration(Minutes length)
        {
            Length = length;
        }

        public Minutes Length { get; }

        public ScheduleType Type => ScheduleType.Duration;


        private double? _endDuration;

        public ScheduleResult Execute(ITestExecutionContext context)
        {
            if (!_endDuration.HasValue)
            {
                _endDuration = context.TotalMinutes + Length;
                Console.WriteLine($"Starting Duration of {Length} Minutes.");
            }

            var durationDone = _endDuration.Value < context.TotalMinutes;
            if (durationDone)
            {
                Console.WriteLine($"Duration of {Length} Minutes Finished.");
                Console.WriteLine("Actual: {0}", context.TotalMinutes - _endDuration.Value + Length);
            }

            return new ScheduleResult(durationDone);
        }
    }
}