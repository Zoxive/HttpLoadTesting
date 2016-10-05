# HttpLoadTesting

HTTP Load testing when single HTTP endpoint is not enough.

Allows you to write test scenarios.

```csharp
// Test
public class ReadAPost : ILoadTest
{
    public string Name => "ReadAPost";

    public Task Initialize(ILoadTestHttpClient loadTestHttpClient)
    {
    	// nothing to initialize
        return Task.FromResult(0);
    }

	public async Task Execute(ILoadTestHttpClient loadTestHttpClient)
	{
        // Simulate user delay in clicking (User think time)
		await loadTestHttpClient.DelayUserClick();

		// User had to open the post
		await loadTestHttpClient.Get("posts/1");

		// User thinking and typing
		await loadTestHttpClient.DelayUserThink()

		var comment = new Dictionary<string, object>
		{
			{"name", "HttpLoadTesting"},
			{"email", "vel+minus+molestias+voluptatum@omnis.com"},
			{"body", "Comment body"}
		};

		await loadTestHttpClient.Post("posts/1/comments", comment);
	}
}

// Console Application
public class Program
{
    public static void Main(string[] args)
    {
        // Specify schedules. Add a few users run for a while and remove them. You can run any schedule in any order.
        // As long as you have active users!
        var schedule = new List<ISchedule>
        {    
            // Add Users over a period of time
            new AddUsers(totalUsers: 10, usersEvery: 2, seconds: 5),
            // Run for a duration of time
            new Duration(0.25m),
            // Remove Users over a period of time
            new RemoveUsers(usersToRemove: 10, usersEvery:2, seconds: 1)
        };

        // Create as many tests as you want to run
        // These are the tests each User will run round robin style
        var tests = new List<ILoadTest>
        {
            new ReadAPost()
        };

        // Create one or more "HttpUsers".
        // This is essentually the HTTP Connection each simulated user will use. Round robin style
        var users = new List<IHttpUser>
        {
            new HttpUser("https://jsonplaceholder.typicode.com/")
            {
                // Specify any properties i need on HttpClient. Like header values!
                AlterHttpClient = SetHttpClientProperties,
                // Specify any properties i need on HttpClientHandler. Like Cookies!
                AlterHttpClientHandler = SetHttpClientHandlerProperties
            }
        };

        var testExecution = new LoadTestExecution(users, tests);
        
        // Run the Tests!
        WebClient.Run(testExecution, schedule);
    }
}
```

TODO list
 - Display what "Schedule" is currently running (Time started, time left?)
 - Display current user count
 - Finish web client
 - Save output from cli that can later open in web client
