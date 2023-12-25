using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        // This class is used to add the initial data to the database.
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            // Add initialisation data if there are no users and no events
            if (!userManager.Users.Any() && !context.Activities.Any())
            {
                // Define users
                var users = new List<AppUser>
                {
                    new AppUser {DisplayName = "Bob", UserName = "bob", Email = "bob@test.com", EmailConfirmed = true},
                    new AppUser {DisplayName = "Tom", UserName = "tom", Email = "tom@test.com"},
                    new AppUser {DisplayName = "Jane", UserName = "jane", Email = "jane@test.com", EmailConfirmed = true},
                };
                foreach (var user in users)
                {
                    // Encrypt and add the user by adding
                    await userManager.CreateAsync(user, password: "Pa$$w0rd");
                }
                // Define activities
                var activities = new List<Activity>
                {
                    new Activity{
                        Title = "Past Activity 1",
                        Date = DateTime.UtcNow.AddMonths(-2),
                        Description = "Activity 2 month ago",
                        Category = "drinks",
                        City = "London",
                        Venue = "Pub",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true}
                        }
                    },
                    new Activity{
                        Title = "Past Activity 2",
                        Date = DateTime.UtcNow.AddMonths(-1),
                        Description = "Activity 1 month ago",
                        Category = "culture",
                        City = "Paris",
                        Venue = "Louvre",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true},
                            new ActivityAttendee{ AppUser = users[1], IsHost = false}
                        }
                    },
                    new Activity{
                    Title = "Future Activity 1",
                    Date = DateTime.UtcNow.AddMonths(1),
                    Description = "Activity 1 month in future",
                    Category = "culture",
                    City = "London",
                    Venue = "Natural History Museum"
                    },
                    new Activity{
                        Title = "Future Activity 2",
                        Date = DateTime.UtcNow.AddMonths(2),
                        Description = "Activity 2 month in future",
                        Category = "musin",
                        City = "London",
                        Venue = "02 Arena",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[2], IsHost = true},
                            new ActivityAttendee{ AppUser = users[1], IsHost = false}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 3",
                        Date = DateTime.UtcNow.AddMonths(3),
                        Description = "Activity 3 month in future",
                        Category = "drinks",
                        City = "London",
                        Venue = "Another pub",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true},
                            new ActivityAttendee{ AppUser = users[2], IsHost = false}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 4",
                        Date = DateTime.UtcNow.AddMonths(4),
                        Description = "Activity 4 month in future",
                        Category = "culture",
                        City = "London",
                        Venue = "Yet Another Pub",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[1], IsHost = true},
                            new ActivityAttendee{ AppUser = users[0], IsHost = false}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 5",
                        Date = DateTime.UtcNow.AddMonths(5),
                        Description = "Activity 5 month in future",
                        Category = "culture",
                        City = "London",
                        Venue = "Just Another Pub",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[1], IsHost = true}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 6",
                        Date = DateTime.UtcNow.AddMonths(6),
                        Description = "Activity 6 month in future",
                        Category = "culture",
                        City = "London",
                        Venue = "Roundhouse Camden",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 7",
                        Date = DateTime.UtcNow.AddMonths(7),
                        Description = "Activity 7 month in future",
                        Category = "culture",
                        City = "London",
                        Venue = "Somewhere on the Thames",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true}
                        }
                    },
                    new Activity{
                        Title = "Future Activity 8",
                        Date = DateTime.UtcNow.AddMonths(8),
                        Description = "Activity 8 month in future",
                        Category = "film",
                        City = "London",
                        Venue = "cinema",
                        Attendees = new List<ActivityAttendee>
                        {
                            new ActivityAttendee{ AppUser = users[0], IsHost = true}
                        }
                    },
                };
                // add the activities
                await context.Activities.AddRangeAsync(activities);
                // save the changes
                await context.SaveChangesAsync();
            }
        }
    }
}