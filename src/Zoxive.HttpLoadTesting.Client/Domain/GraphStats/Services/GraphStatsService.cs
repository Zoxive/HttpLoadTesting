using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Dtos;
using Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Repositories;
using Zoxive.HttpLoadTesting.Client.Pages;

namespace Zoxive.HttpLoadTesting.Client.Domain.GraphStats.Services
{
    public class GraphStatsService : IGraphStatsService
    {
        private readonly IRequestGraphRepository _requestGraphRepository;
        private readonly ITestGraphRepository _testGraphRepository;
        private readonly IStatusCodeGraphRepository _statusCodeGraphRepository;

        public GraphStatsService(
            IRequestGraphRepository requestGraphRepository,
            ITestGraphRepository testGraphRepository,
            IStatusCodeGraphRepository statusCodeGraphRepository)
        {
            _requestGraphRepository = requestGraphRepository;
            _testGraphRepository = testGraphRepository;
            _statusCodeGraphRepository = statusCodeGraphRepository;
        }

        public async Task<IEnumerable<GraphStatDto>> Get(Filters filters)
        {
            var minuteMilliseconds = GetPeriod(filters);

            IEnumerable<GraphStatDto> result;

            switch (filters.CollationType)
            {
                case CollationType.Requests:
                    result = await _requestGraphRepository.Get(minuteMilliseconds, filters);
                    break;

                case CollationType.Tests:
                    result = await _testGraphRepository.Get(minuteMilliseconds, filters);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public async Task<IEnumerable<StatusCodeStatDto>> GetStatusCodes(Filters filters)
        {
            var minuteMilliseconds = GetPeriod(filters);

            var result = await _statusCodeGraphRepository.Get(minuteMilliseconds, filters);

            return result;
        }

        private static decimal GetPeriod(Filters filters)
        {
            if (!filters.Period.HasValue) throw new ArgumentNullException(nameof(filters), "Filter.Period must have a value");

            var minuteMilliseconds = Math.Round(filters.Period.Value * 60000);
            return minuteMilliseconds;
        }
    }
}