using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Repositories
{
    public interface IIterationResultRepository
    {
        void Save(UserIterationResult iterationResult);

        IReadOnlyDictionary<string, UserIterationResult> GetAll();
    }
}