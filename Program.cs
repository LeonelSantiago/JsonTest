using JsonTest.Data;
using JsonTest.Services;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        string jsonUrl = "https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json";

        var serviceProvider = new ServiceCollection()
            .AddDbContext<JsonTestDbContext>()         
            .AddHttpClient()
            .AddScoped<UserService>()
            .BuildServiceProvider();

        var userService = serviceProvider.GetRequiredService<UserService>();

        var users = await userService.FetchUsersFromJsonAsync(jsonUrl);
        if (users.Count == 0)
        {
            Console.WriteLine("No valid data fetched.");
            return;
        }

        await userService.SaveUsersToDatabaseAsync(users);

        var sortedUsers = await userService.GetSortedUsersAsync();
        foreach (var user in sortedUsers)
        {
            Console.WriteLine($"User data:");
            Console.WriteLine($"Id: {user.Id}");
            Console.WriteLine($"Name: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Language: {user.Language}");
            Console.WriteLine($"Bio: {user.Bio}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}
