using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace civil_sheriff_scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            //go scrape the dates from the auction listing
            AuctionDateProcessor.ProcessAuctionDates();
            List<string> unscrapedDates = AuctionDateProcessor.GetUnscrapedAuctionDates();

            //start the scraping!!!
            AuctionProcessor.IndexAuctions(unscrapedDates);            
        }
    }
}
