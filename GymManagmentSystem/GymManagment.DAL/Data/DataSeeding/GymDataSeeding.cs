using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.DataSeeding
{
   public class GymDataSeeding
    {
        public static async Task SeedDataAsync(string folderPath,GymDbContext dbContext,ILogger logger ,CancellationToken ct=default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync())
                {
                    var plans = LoadDataFromFileJson<Plan>(folderPath, "Plans.json");
                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Plans seedes with counts ={plans.Count}");

                    }
                    if (dbContext.ChangeTracker.HasChanges())
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    else
                        logger.LogInformation("Plan Already seeded");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Gym Data seeding Faild");
                throw;
            }
        }
        private static List<T> LoadDataFromFileJson<T>(string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{fileName}' not found in folder '{folderPath}'.");
            }
           var jsonData = File.ReadAllText(filePath);
           var options = new JsonSerializerOptions
           {
               PropertyNameCaseInsensitive = true
           };
            var data =JsonSerializer.Deserialize<List<T>>(jsonData, options);
            return data ?? new List<T>();
        }   
    }
}
