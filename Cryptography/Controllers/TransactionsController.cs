using Cryptography.Data;
using Cryptography.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cryptography.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> Get()
        {
            var transactions = await _context.Transactions.ToListAsync();

            if (transactions is null)
            {
                return NotFound("transactions not founded...");
            }

            return Ok(transactions);
        }

        [HttpGet("{id}", Name = "GetTransaction")]
        public async Task<ActionResult<Transaction>> Get(long id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

            if (transaction is null)
            {
                return NotFound("transaction not founded...");
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post(Transaction transaction)
        {
            if (transaction is null)
            {
                return BadRequest("");    
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("GetTransaction",
                new { id = transaction.Id }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(long id, Transaction transaction) 
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

            if (transaction is null)
            {
                return NotFound("transaction not founded");
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }
    }
}
