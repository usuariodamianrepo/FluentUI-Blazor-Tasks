
using AutoMapper;
using BackEnd.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace BackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TxskTypesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TxskTypesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TxskTypeDTO>>> GetTxskTypes()
        {
            var allItems = await _context.TxskTypes.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TxskType>, IEnumerable<TxskTypeDTO>>(allItems));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TxskTypeDTO>> GetTxskType(int id)
        {
            var oneItem = await _context.TxskTypes.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"TxskType Id: {id} not found."));
            }

            return Ok(_mapper.Map<TxskType, TxskTypeDTO>(oneItem));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> PutTxskType(int id, TxskTypeDTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"TxskType Id: {id} mismatch."));
            }

            _context.Entry(_mapper.Map<TxskTypeDTO, TxskType>(toUpdateDTO)).State = EntityState.Modified;

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

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostTxskType(TxskTypeDTO toAdd)
        {
            var newItem = _context.TxskTypes.Add(_mapper.Map<TxskTypeDTO, TxskType>(toAdd));
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"TxskType Id: {newItem.Entity.Id} was created!"));
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

        private bool TxskTypeExists(int id)
        {
            return _context.TxskTypes.Any(e => e.Id == id);
        }
    }
}