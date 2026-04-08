
using BackEnd.API.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Responses;

namespace BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TxsksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TxsksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteContact(int id)
        {
            var contact = await _context.Txsks.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new GeneralResponse(false, $"Not Found Txsk Id: {id} to delete."));
            }

            _context.Txsks.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"Txsk Id: {id} was deleted!"));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<TxskDTO>>> GetByFilterTxsks(
            [FromQuery] DateTime? dueDateFrom,
            [FromQuery] DateTime? dueDateTo)
        {
            if (!dueDateFrom.HasValue || !dueDateTo.HasValue)
            {
                return BadRequest(new GeneralResponse(false, $"Due Date From or Due Date To can not be null"));
            }

            if (dueDateFrom > dueDateTo)
            {
                return BadRequest(new GeneralResponse(false, $"Due Date From {dueDateFrom} can not be greated than Due Date To {dueDateTo}"));
            }

            var query = _context.Txsks
                .Include("Contact")
                .Include("TxskStatus")
                .Include("TxskType")
                .AsQueryable();

            if (dueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= dueDateFrom.Value);

            if (dueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= dueDateTo.Value);

            var filteredItems = await query.Take(100).ToListAsync();

            return Ok(filteredItems.Adapt<IEnumerable<TxskDTO>>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TxskDTO>> GetTxsk(int id)
        {
            var oneItem = await _context.Txsks
                .Include("Contact")
                .Include("TxskStatus")
                .Include("TxskType")
                .FirstOrDefaultAsync(x => x.Id == id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"Txsk Id: {id} not found."));
            }

            return Ok(oneItem.Adapt<TxskDTO>());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TxskDTO>>> GetTxsks()
        {
            var allItems = await _context.Txsks
                .Include("Contact")
                .Include("TxskStatus")
                .Include("TxskType")
                .ToListAsync();
            
            return Ok(allItems.Adapt<IEnumerable<TxskDTO>>());
        }
        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostTxsk(TxskDTO toAdd)
        {
            var newItem = _context.Txsks.Add(toAdd.Adapt<Txsk>());
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"Txsk Id: {newItem.Entity.Id} was created!"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> PutTxsk(int id, TxskDTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"Txsk Id: {id} mismatch."));
            }

            _context.Entry(toUpdateDTO.Adapt<Txsk>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TxskExists(id))
                {
                    return NotFound(new GeneralResponse(false, $"Not Found Txsk Id: {id} to save."));
                }
                else
                {
                    return NotFound(new GeneralResponse(false, $"An error occurred while updating the Txsk Id: {id}."));
                }
            }

            return Ok(new GeneralResponse(true, $"Txsk Id: {id} was updated!"));
        }
        private bool TxskExists(int id)
        {
            return _context.Txsks.Any(e => e.Id == id);
        }
    }
}