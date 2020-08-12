using System;
using System.Threading;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class ClientOptions
    {
        private string _databaseFile;
        public string DatabaseFile
        {
            get => _databaseFile;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    var now = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                    value = $"Test_{now}.db";
                }
                else
                {
                    Viewing = true;
                }

                _databaseFile = value;
            }
        }

        public bool StopApplicationWhenComplete { get; }

        public bool Viewing { get; private set; }

        public CancellationTokenSource CancelTokenSource { get; }

        public ClientOptions(string databaseFile = null, CancellationTokenSource cancelToken = null, bool? stopAppWhenComplete = false)
        {
            DatabaseFile = databaseFile;
            CancelTokenSource = cancelToken ?? new CancellationTokenSource();
            StopApplicationWhenComplete = stopAppWhenComplete ?? false;
        }
    }
}