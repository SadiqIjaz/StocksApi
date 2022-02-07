using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stocks.Data;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly StocksContext _context;

        public StocksController(StocksContext context)
        {
            _context = context;
        }

        // GET: api/Stocks & api/Stocks?Name=
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocks([FromQuery] string name)
        {
            IEnumerable<Stock> stocks = null;

            try
            {
                stocks = await _context.Stocks.ToListAsync();
                if (stocks != null && name != null)
                {
                    stocks = stocks.Where(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch
            {
                stocks = Array.Empty<Stock>();
            }

            return CreatedAtAction(nameof(GetStocks), stocks.ToList());
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        //[Authorize]
        public async Task<ActionResult<Stock>> GetStocks(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return stock;
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutStocks(int id, Stock stock)
        {
            if (id != stock.StockId)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stocks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Stock>> PostStocks(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStock", new { id = stock.StockId }, stock);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Stock>> DeleteStocks(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
