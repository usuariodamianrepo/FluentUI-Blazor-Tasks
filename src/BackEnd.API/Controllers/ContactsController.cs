using AutoMapper;
using BackEnd.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ContactsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new GeneralResponse(false, $"Not Found Contact Id: {id} to delete."));
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"Contact Id: {id} was deleted!"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDTO>> GetContact(int id)
        {
            var oneItem = await _context.Contacts.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"Contact Id: {id} not found."));
            }

            return Ok(_mapper.Map<Contact, ContactDTO>(oneItem));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContacts()
        {
            var allItems = await _context.Contacts.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<Contact>, IEnumerable<ContactDTO>>(allItems));
        }

        [HttpGet("byName")]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContactsByFirstName([FromQuery] string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new GeneralResponse(false, $"The parameter name is null or empty."));
            }

            var allItems = await _context.Contacts.Where(c => c.FirstName.StartsWith(name) || c.LastName.StartsWith(name)) 
                                  .OrderBy(i => i.FirstName)
                                  .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<Contact>, IEnumerable<ContactDTO>>(allItems));
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostContact(ContactDTO toAdd)
        {
            var newItem = _context.Contacts.Add(_mapper.Map<ContactDTO, Contact>(toAdd));
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"Contact Id: {newItem.Entity.Id} was created!"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> PutContact(int id, ContactDTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"Contact Id: {id} mismatch."));
            }

            _context.Entry(_mapper.Map<ContactDTO, Contact>(toUpdateDTO)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound(new GeneralResponse(false, $"Not Found Contact Id: {id} to save."));
                }
                else
                {
                    return NotFound(new GeneralResponse(false, $"An error occurred while updating the Contact Id: {id}."));
                }
            }

            return Ok(new GeneralResponse(true, $"Contact Id: {id} was updated!"));
        }
        
        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}