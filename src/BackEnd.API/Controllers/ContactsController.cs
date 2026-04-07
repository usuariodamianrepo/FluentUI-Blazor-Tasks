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
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
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

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetByFilterContacts(
            [FromQuery] string? email,
            [FromQuery] string? company,
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] string? phone)
        {
            var query = _context.Contacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(t => t.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(company))
                query = query.Where(t => t.Company != null && t.Company.Contains(company));
            if (!string.IsNullOrWhiteSpace(firstName))
                query = query.Where(t => t.FirstName.Contains(firstName));
            if (!string.IsNullOrWhiteSpace(lastName))
                query = query.Where(t => t.LastName.Contains(lastName));
            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(t => t.Phone != null && t.Phone.Contains(phone));

            var filteredItems = await query.Take(100).ToListAsync();

            return Ok(filteredItems.Adapt<IEnumerable<ContactDTO>>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDTO>> GetContact(int id)
        {
            var oneItem = await _context.Contacts.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"Contact Id: {id} not found."));
            }

            return Ok(oneItem.Adapt<ContactDTO>());
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContacts()
        {
            var allItems = await _context.Contacts.ToListAsync();
            return Ok(allItems.Adapt<IEnumerable<ContactDTO>>());
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

            return Ok(allItems.Adapt<IEnumerable<ContactDTO>>());
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> PostContact(ContactDTO toAdd)
        {
            var newItem = _context.Contacts.Add(toAdd.Adapt<Contact>());
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

            _context.Entry(toUpdateDTO.Adapt<Contact>()).State = EntityState.Modified;

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