---
name: api-rest
description: For one specific class create its API REST, this include the controller, the DTO and the configurations necessary to run the proyect. For all examples code we will use T4 text anotation. T4 text template is a mixture of text blocks and control logic that can generate a text file.
---

# API Create

## Set the DbContext into the AppDbContext class
Open the AppDbContext file and add the DbSet line. 

```csharp   
    public DbSet<<#=ClassName#>> <#=PluralClassName#> { get; set; }
```    

## Create the DTO
Create the DTO file into the DTOs folder with the name of the class and suffix "DTO". Copy the properties from the model class and remove any navigation properties. You can also add data annotations if needed.

```csharp
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public sealed class <#=ClassName#>DTO
    {
        // Copy the properties class
    }
}
```

## Create the Controller
Create the controller file with the name of the class in plural.

```csharp
using AutoMapper;
using BackEnd.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace BackEnd.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class <#=PluralClassName#>Controller : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public <#=PluralClassName#>Controller(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<<#=ClassName#>DTO>>> Get<#=PluralClassName#>()
        {
            var allItems = await _context.<#=PluralClassName#>.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<<#=ClassName#>>, IEnumerable<<#=ClassName#>DTO>>(allItems));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<<#=ClassName#>DTO>> Get<#=ClassName#>(int id)
        {
            var oneItem = await _context.<#=PluralClassName#>.FindAsync(id);

            if (oneItem == null)
            {
                return NotFound(new GeneralResponse(false, $"<#=ClassName#> Id: {id} not found."));
            }

            return Ok(_mapper.Map<<#=ClassName#>, <#=ClassName#>DTO>(oneItem));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GeneralResponse>> Put<#=ClassName#>(int id, <#=ClassName#>DTO toUpdateDTO)
        {
            if (id != toUpdateDTO.Id)
            {
                return BadRequest(new GeneralResponse(false, $"<#=ClassName#> Id: {id} mismatch."));
            }

            _context.Entry(_mapper.Map<<#=ClassName#>DTO, <#=ClassName#>>(toUpdateDTO)).State = EntityState.Modified;

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

        private bool <#=ClassName#>Exists(int id)
        {
            return _context.<#=PluralClassName#>.Any(e => e.Id == id);
        }
    }
}
```
## Set the DTO into the Mapping Profile
Open the Mapping Profile file and add the mapping between the model and the DTO. 

```csharp   
    CreateMap<<#=ClassName#>, <#=ClassName#>DTO>();
    CreateMap<<#=ClassName#>DTO, <#=ClassName#>>();
```        