---
name: api-rest
description: Create its API REST form specific class, this include the controller, the DTO and the configurations necessary to run the proyect. All examples code use T4 text anotation. Copy the code from the example and replace the <#=ClassName#> and <#=PluralClassName#> with the name of your class and its plural form.
---

# API Create

## Set the DbContext into the AppDbContext class
At the BackEnd.API proyect, open the AppDbContext file and add the DbSet line. 

```csharp   
    public DbSet<<#=ClassName#>> <#=PluralClassName#> { get; set; }
```    

## Create the DTO
At the Shared proyect, create the DTO file into the DTOs folder with the name of the class and suffix "DTO". Copy the properties from the model class and remove any navigation properties. You can also add data annotations if needed.

```csharp
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public sealed class <#=ClassName#>DTO
    {
        // Copy the properties class
    }
}
```

## Create the Controller
At the BackEnd.API proyect, create the controller file into the Controllers folder with the name of the class in plural and suffix "Controller".

```csharp
using BackEnd.API.Data;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Responses;

namespace BackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class <#=PluralClassName#>Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public <#=PluralClassName#>Controller(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> DeleteContact(int id)
        {
            var contact = await _context.<#=PluralClassName#>.FindAsync(id);
            if (contact == null)
            {
                return NotFound(new GeneralResponse(false, $"Not Found <#=ClassName#> Id: {id} to delete."));
            }

            _context.<#=PluralClassName#>.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"<#=ClassName#> Id: {id} was deleted!"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<<#=ClassName#>DTO>>> Get<#=PluralClassName#>()
        {
            var allItems = await _context.<#=PluralClassName#>.ToListAsync();
            return Ok(allItems.Adapt<List<<#=ClassName#>DTO>>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<<#=ClassName#>DTO>> Get<#=ClassName#>(int id)
        {
            var oneItem = await _context.<#=PluralClassName#>.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"<#=ClassName#> Id: {id} not found."));
            }

            return Ok(oneItem.Adapt<<#=ClassName#>DTO>());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> Put<#=ClassName#>(int id, <#=ClassName#>DTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"<#=ClassName#> Id: {id} mismatch."));
            }

            _context.Entry(toUpdateDTO.Adapt<<#=ClassName#>>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!<#=ClassName#>Exists(id))
                {
                    return NotFound(new GeneralResponse(false, $"Not Found <#=ClassName#> Id: {id} to save."));
                }
                else
                {
                    return NotFound(new GeneralResponse(false, $"An error occurred while updating the <#=ClassName#> Id: {id}."));
                }
            }

            return Ok(new GeneralResponse(true, $"<#=ClassName#> Id: {id} was updated!"));
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Post<#=ClassName#>(<#=ClassName#>DTO toAdd)
        {
            var newItem = _context.<#=PluralClassName#>.Add(_mapper.Map<<#=ClassName#>DTO, <#=ClassName#>>(toAdd));
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"<#=ClassName#> Id: {newItem.Entity.Id} was created!"));
        }

        private bool <#=ClassName#>Exists(int id)
        {
            return _context.<#=PluralClassName#>.Any(e => e.Id == id);
        }
    }
}
```      