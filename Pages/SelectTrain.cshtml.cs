using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrainTickets.Models;
using TrainTickets.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

using TrainTickets.Models.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace TrainTickets.Pages
{
    [AllowAnonymous]
    public class SelectTrainModel : PageModel
    {
        private readonly TrainTicketsContext _context;

        public SelectTrainModel(TrainTicketsContext context) {
            _context = context;
        }

        public StationsAndDateVM StationsAndDateVM { get; set; }
        public IList<TrainVM> TrainVM { get; set; } = new List<TrainVM>();
        

        public async Task OnGetAsync(string originStation, string destStation, string departureDate) {
            DateTime departDatetime = DateTime.ParseExact(departureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            StationsAndDateVM = new StationsAndDateVM {
                OriginStation = originStation,
                DestStation = destStation,
                DepartDatetime = departDatetime
            };
            TempData["StationsAndDateVM"] = JsonConvert.SerializeObject(StationsAndDateVM);

            IList<Train> trains = await _context.Train
                .Include(t => t.TrainStations)
                    .ThenInclude(ts => ts.Station)
                .Include(t => t.Coaches)
                .ToListAsync();

            InitTrainVM(trains, StationsAndDateVM);
        }

        private void InitTrainVM(IList<Train> trains, StationsAndDateVM stationsAndDateVM) {
            string originStation = stationsAndDateVM.OriginStation;
            string destStation = stationsAndDateVM.DestStation;
            DateTime departDatetime = stationsAndDateVM.DepartDatetime;

            foreach (Train train in trains) {
                TrainStation originTS = null;
                TrainStation destTS = null;
                bool departDateMatches = false;
                foreach (TrainStation ts in train.TrainStations.ToList()) {
                    if (ts.Station.Name.Equals(originStation)) {
                        originTS = ts;
                        if (ts.DepartureAt.Date.Equals(departDatetime.Date)) {
                            departDateMatches = true;
                        }
                    }
                    if (originTS != null && departDateMatches && ts.Station.Name.Equals(destStation)) {
                        destTS = ts;
                    }
                }
                if (originTS != null && destTS != null && departDateMatches) {
                    TimeSpan tripTimespan = destTS.ArrivalAt - originTS.DepartureAt;
                    TrainVM.Add(new Models.ViewModels.TrainVM {
                        ID = train.ID,
                        Name = train.Name,
                        FirstStation = train.TrainStations.First().Station.Name,
                        LastStation = train.TrainStations.Last().Station.Name,
                        DepartDatetime = originTS.DepartureAt,
                        ArrivalDatetime = destTS.ArrivalAt,
                        TripDuration = ToReadableString(tripTimespan),
                        MinPriceTenge = MinPriceAmongCoaches(train.Coaches.ToList())
                    });

                    // For saving Ticket with origin_train_station_id and dest_train_station_id in PurchaseTicket.OnPost()
                    TempData["OriginTrainStationID"] = originTS.ID;
                    TempData["DestTrainStationID"] = destTS.ID;
                }
            }
        }

        private int MinPriceAmongCoaches(IList<Coach> coaches) {
            int minPrice = coaches.First().PriceTenge;
            foreach (var coach in coaches) {
                if (coach.PriceTenge < minPrice) {
                    minPrice = coach.PriceTenge;
                }
            }
            return minPrice;
        }

        private string ToReadableString(TimeSpan span) {
            return string.Format("{0}{1}{2}",
                        span.Duration().Days > 0 ? string.Format("{0:0}d ", span.Days) : string.Empty,
                        span.Duration().Hours > 0 ? string.Format("{0:0}h ", span.Hours) : string.Empty,
                        span.Duration().Minutes > 0 ? string.Format("{0:0}m ", span.Minutes) : string.Empty);
        }
    }
}
