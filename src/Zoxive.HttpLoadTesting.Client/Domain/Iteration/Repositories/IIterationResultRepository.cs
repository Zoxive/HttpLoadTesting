using System;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Domain.Iteration.Repositories
{
    public interface IIterationResultRepository : IDisposable
    {
        Task Save(UserIterationResult iterationResult);
    }
}