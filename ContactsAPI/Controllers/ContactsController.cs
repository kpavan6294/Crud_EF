using ContactsAPI.Data;
using ContactsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactAPIDbContext dbContext;

        public ContactsController(ContactAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
      
        [HttpGet]
        public IActionResult GetContacts()
        {
            return Ok(dbContext.Contacts.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactRequest addContactRequest)
        {
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FullName = addContactRequest.FullName,
                Email = addContactRequest.Email,
                Phone = addContactRequest.Phone,
                Address = addContactRequest.Address
            };

            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();

            return Ok(contact);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] Guid id, UpdateContactRequest updateContactRequest)
        {
            //check if db has a value for this contact to update
            var contact = await dbContext.Contacts.FindAsync(id);

            if (contact != null)
            {
                contact.FullName = updateContactRequest.FullName;
                contact.Email = updateContactRequest.Email;
                contact.Phone = updateContactRequest.Phone;
                contact.Address = updateContactRequest.Address;

               await  dbContext.SaveChangesAsync();

                return Ok(contact);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute]Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);

            if (contact != null)
            {
                return Ok(contact);
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);

            if (contact !=null)
            {
               dbContext.Remove(contact);
               await dbContext.SaveChangesAsync();

                return Ok("Contact deleted");
            }
            return NotFound();
        }
    }
}
