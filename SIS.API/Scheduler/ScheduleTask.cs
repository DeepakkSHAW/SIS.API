using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SIS.API.Scheduler.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        protected override string Schedule => "*/1 * * * *";

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Processing starts here");
            System.Diagnostics.Debug.WriteLine($"[{DateTime.UtcNow.ToLongTimeString()}]Processing starts here");
            //var myStocks = new StockInfo();
            //var aStock = myStocks.GetAStockLatestPrice(10);

            //if(aStock != null)
            //    System.Diagnostics.Debug.WriteLine($"The latest price for StockId: {aStock.StockID} name {aStock.StockName} is {aStock.Price} recorded as on {aStock.ValueOn}.");
            return Task.CompletedTask;
        }
    }
}
