#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Models;
using Microsoft.AspNetCore.Authorization;

namespace TrainTickets.Pages.Coaches
{
    [Authorize(Roles = "ADMIN")]
    public class EditModel : PageModel
    {
        private readonly TrainTickets.Data.TrainTicketsContext _context;

        public EditModel(TrainTickets.Data.TrainTicketsContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Coach Coach { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Coach = await _context.Coach
                .Include(c => c.Train).FirstOrDefaultAsync(m => m.ID == id);

            if (Coach == null)
            {
                return NotFound();
            }
           ViewData["TrainID"] = new SelectList(_context.Train, "ID", "ID");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Coach).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoachExists(Coach.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CoachExists(int id)
        {
            return _context.Coach.Any(e => e.ID == id);
        }
    }
}
