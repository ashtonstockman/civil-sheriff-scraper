using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace civil_sheriff_scraper
{
    class AuctionDateProcessor
    {
        public static void ProcessAuctionDates()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(@"http://civilsheriff.com/RealEstate/RealEstateFilter.asp");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                var doc = new HtmlDocument();
                HtmlNode.ElementsFlags.Remove("option");
                doc.Load(reader.BaseStream);

                var dateSelect = doc.DocumentNode.SelectNodes("//select//option");
                using (CSScraperDataContext db = new CSScraperDataContext())
                {
                    List<AuctionDate> datesToAdd = new List<AuctionDate>();
                    List<AuctionDate> existingDates = db.AuctionDates.ToList();

                    foreach (var node in dateSelect)
                    {
                        string auctionDate = node.Attributes["value"].Value;
                        if (!String.IsNullOrEmpty(auctionDate))
                        { 
                            AuctionDate date = existingDates.SingleOrDefault(x => x.StringDate == auctionDate);
                            if (date == null)
                            {
                                //add it to the database dawg
                                date = new AuctionDate() { StringDate = auctionDate, AuctionDateVal = Convert.ToDateTime(auctionDate) };
                                datesToAdd.Add(date);
                            }
                        }
                    }
                    db.AuctionDates.InsertAllOnSubmit(datesToAdd);
                    db.SubmitChanges();
                }
            }
        }

        public static List<string> GetUnscrapedAuctionDates()
        {
            List<string> returnList = new List<string>();

            using (CSScraperDataContext db = new CSScraperDataContext())
            {
                returnList = db.AuctionDates.Where(x => x.AuctionDateVal > DateTime.Now).Select(x => x.StringDate).ToList();
            }

            return returnList;
        }
    }
}
