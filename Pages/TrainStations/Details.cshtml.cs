#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Models;

namespace TrainTickets.Pages.TrainStations
{
    [Authorize(Roles = "ADMIN")]
    public class DetailsModel : PageModel
    {
        private readonly TrainTickets.Data.TrainTicketsContext _context;

        public DetailsModel(TrainTickets.Data.TrainTicketsContext context)
        {
            _context = context;
        }

        public TrainStation TrainStation { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TrainStation = await _context.TrainStation
                .Include(t => t.Station)
                .Include(t => t.Train).FirstOrDefaultAsync(m => m.ID == id);

            if (TrainStation == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
