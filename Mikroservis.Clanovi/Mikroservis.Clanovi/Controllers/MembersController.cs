using Microsoft.AspNetCore.Mvc;
using FitnessCentar.Members.Models;
using FitnessCentar.Members.Persistence;
using Microsoft.EntityFrameworkCore;
using FitnessCentar.Members.Interface;
using FitnessCentar.Members.Services;
using Newtonsoft.Json;

namespace FitnessCentar.Members.Controllers
{
    [ApiController]
    [Route("api/members")]
    public class MembersController : ControllerBase
    {
        private readonly MembersDbContext _context;
        private readonly IMessageBroker _messageBroker;
        private readonly IAuthService _authService;

        public MembersController(MembersDbContext context, IAuthService authService, IMessageBroker messageBroker)
        {
            _context = context;
            _authService = authService;
            _messageBroker = messageBroker;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _context.Members.ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            return Ok(member);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MemberRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Provera da li već postoji korisnik sa istim e-mailom
            var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == request.Email);
            if (existingMember != null)
                return Conflict("Korisnik sa ovim emailom već postoji.");

            // Kreiranje novog člana i heširanje lozinku
            var member = new Member
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                DateOfBirth = request.DateOfBirth
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Generisanje JWT token
            var token = _authService.GenerateJwtToken(member.Id);

            // Priprema email poruke za slanje
            var emailMessage = JsonConvert.SerializeObject(new
            {
                Recipient = member.Email,
                Subject = "Dobrodošli!",
                Body = $"Zdravo {member.FirstName}, hvala što ste se registrovali!"
            });

            // Objavljivanje poruke na RabbitMQ
            bool messageSent = _messageBroker.Publish("email_exchange", "email_key", emailMessage);

            // Provera da li je poruka uspešno poslata na RabbitMQ
            if (messageSent)
            {
                Console.WriteLine("Poruka je uspešno poslata na RabbitMQ.");
            }
            else
            {
                Console.WriteLine("Došlo je do greške prilikom slanja poruke na RabbitMQ.");
            }

            return Ok(new { Token = token });
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] Member updatedMember)
        {
            if (id != updatedMember.Id)
                return BadRequest();

            _context.Entry(updatedMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}


