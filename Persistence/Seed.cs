using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if(context.Activities.Any()) return;
            var activities = new List<Activity>
            {
                new Activity{
                    Title = "Past Activity 1",
                    Date = DateTime.UtcNow.AddMonths(-2),
                    Description = "Activity 2 month ago",
                    Category = "drinks",
                    City = "London",
                    Venue = "Pub"
                },
                new Activity{
                    Title = "Past Activity 2",
                    Date = DateTime.UtcNow.AddMonths(-1),
                    Description = "Activity 1 month ago",
                    Category = "culture",
                    City = "Paris",
                    Venue = "Louvre"
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
                    Venue = "02 Arena"
                },
                new Activity{
                    Title = "Future Activity 3",
                    Date = DateTime.UtcNow.AddMonths(3),
                    Description = "Activity 3 month in future",
                    Category = "drinks",
                    City = "London",
                    Venue = "Another pub"
                },
                new Activity{
                    Title = "Future Activity 4",
                    Date = DateTime.UtcNow.AddMonths(4),
                    Description = "Activity 4 month in future",
                    Category = "culture",
                    City = "London",
                    Venue = "Yet Another Pub"
                },
                 new Activity{
                    Title = "Future Activity 5",
                    Date = DateTime.UtcNow.AddMonths(5),
                    Description = "Activity 5 month in future",
                    Category = "culture",
                    City = "London",
                    Venue = "Just Another Pub"
                },
                 new Activity{
                    Title = "Future Activity 6",
                    Date = DateTime.UtcNow.AddMonths(6),
                    Description = "Activity 6 month in future",
                    Category = "culture",
                    City = "London",
                    Venue = "Roundhouse Camden"
                },
                 new Activity{
                    Title = "Future Activity 7",
                    Date = DateTime.UtcNow.AddMonths(7),
                    Description = "Activity 7 month in future",
                    Category = "culture",
                    City = "London",
                    Venue = "Somewhere on the Thames"
                },
                 new Activity{
                    Title = "Future Activity 8",
                    Date = DateTime.UtcNow.AddMonths(8),
                    Description = "Activity 8 month in future",
                    Category = "film",
                    City = "London",
                    Venue = "cinema"
                },
            };
            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();
        }
    }
}