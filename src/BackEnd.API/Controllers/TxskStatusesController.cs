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
    public class TxskStatusesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TxskStatusesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteContact(int id)
        {
            var contact = await _context.TxskStatuses.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new GeneralResponse(false, $"Not Found TxskStatus Id: {id} to delete."));
            }

            _context.TxskStatuses.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"TxskStatus Id: {id} was deleted!"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TxskStatusDTO>> GetTxskStatus(int id)
        {
            var oneItem = await _context.TxskStatuses.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"TxskStatus Id: {id} not found."));
            }

            return Ok(oneItem.Adapt<TxskStatusDTO>());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TxskStatusDTO>>> GetTxskStatuses()
        {
            var allItems = await _context.TxskStatuses.ToListAsync();
            return Ok(allItems.Adapt<List<TxskStatusDTO>>());
        }
        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostTxskStatus(TxskStatusDTO toAdd)
        {
            var newItem = _context.TxskStatuses.Add(toAdd.Adapt<TxskStatus>());
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"TxskStatus Id: {newItem.Entity.Id} was created!"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> PutTxskStatus(int id, TxskStatusDTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"TxskStatus Id: {id} mismatch."));
            }

            _context.Entry(toUpdateDTO.Adapt<TxskStatus>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TxskStatusExists(id))
                {
                    return NotFound(new GeneralResponse(false, $"Not Found TxskStatus Id: {id} to save."));
                }
                else
                {
                    return NotFound(new GeneralResponse(false, $"An error occurred while updating the TxskStatus Id: {id}."));
                }
            }

            return Ok(new GeneralResponse(true, $"TxskStatus Id: {id} was updated!"));
        }
        private bool TxskStatusExists(int id)
        {
            return _context.TxskStatuses.Any(e => e.Id == id);
        }
    }
}