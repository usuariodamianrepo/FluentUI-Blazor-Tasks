
using BackEnd.API.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TxskTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TxskTypesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteContact(int id)
        {
            var contact = await _context.TxskTypes.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new GeneralResponse(false, $"Not Found TxskType Id: {id} to delete."));
            }

            _context.TxskTypes.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"TxskType Id: {id} was deleted!"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TxskTypeDTO>> GetTxskType(int id)
        {
            var oneItem = await _context.TxskTypes.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"TxskType Id: {id} not found."));
            }

            return Ok(oneItem.Adapt<TxskTypeDTO>());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TxskTypeDTO>>> GetTxskTypes()
        {
            var allItems = await _context.TxskTypes.ToListAsync();
            return Ok(allItems.Adapt<IEnumerable<TxskTypeDTO>>());
        }
        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostTxskType(TxskTypeDTO toAdd)
        {
            var newItem = _context.TxskTypes.Add(toAdd.Adapt<TxskType>());
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"TxskType Id: {newItem.Entity.Id} was created!"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> PutTxskType(int id, TxskTypeDTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"TxskType Id: {id} mismatch."));
            }

            _context.Entry(toUpdateDTO.Adapt<TxskType>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TxskTypeExists(id))
                {
                    return NotFound(new GeneralResponse(false, $"Not Found TxskType Id: {id} to save."));
                }
                else
                {
                    return NotFound(new GeneralResponse(false, $"An error occurred while updating the TxskType Id: {id}."));
                }
            }

            return Ok(new GeneralResponse(true, $"TxskType Id: {id} was updated!"));
        }
        private bool TxskTypeExists(int id)
        {
            return _context.TxskTypes.Any(e => e.Id == id);
        }
    }
}