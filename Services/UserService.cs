using System.Net.Http.Json;
using System.Text.Json;
using JsonTest.Data;
using JsonTest.Data.Entities;
using JsonTest.Model;
using Microsoft.EntityFrameworkCore;

namespace JsonTest.Services
{
    public class UserService
    {
        private readonly JsonTestDbContext _context;
        private readonly HttpClient _httpClient;

        public UserService(JsonTestDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<List<User>> FetchUsersFromJsonAsync(string jsonUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonUrl))
                {
                    return new List<User>();
                }

                var jsonUsers = await _httpClient.GetFromJsonAsync<List<JsonUser>>(jsonUrl);
                if (jsonUsers == null)
                {
                    return new List<User>();
                }

                var users = jsonUsers
                    .Where(x => !string.IsNullOrEmpty(x.Name) || !string.IsNullOrEmpty(x.Id))
                    .Select(j => new User
                    {
                        Id = j.Id,
                        FirstName = GetSeparatedName(j.Name).firstName,
                        LastName = GetSeparatedName(j.Name).lastName,
                        Language = j.Language,
                        Bio = j.Bio,
                        Version = j.Version
                    }).ToList();

                return users;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network error: Unable to read data from JSON file.");
                Console.WriteLine($"Details: {ex.Message}");
                return new List<User>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error parsing JSON data.");
                Console.WriteLine($"Details: {ex.Message}");
                return new List<User>();
            }
        }
        public async Task SaveUsersToDatabaseAsync(List<User> users)
        {
            try
            {
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database error: Unable to save data.");
                Console.WriteLine($"Details: {ex.Message}");
            }
        }

        public async Task<List<User>> GetSortedUsersAsync()
        {
            try
            {
                return await _context.Users
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving data.");
                Console.WriteLine($"Details: {ex.Message}");
                return new List<User>();
            }
        }

        private static (string firstName, string lastName) GetSeparatedName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return (string.Empty, string.Empty);

            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return names.Length > 0 ? (names[0], names[1]) : (string.Empty, string.Empty);
        }
    }
}