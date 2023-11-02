// using api.Data;
// using api.Interfaces;
// using Microsoft.EntityFrameworkCore;
// using Models;

// namespace api.Repository;
// public class EventRepository : IEventRepository
// {
//     private readonly BookCircleContext _context;
//     public EventRepository(BookCircleContext context)
//     {
//         _context = context;
//     }

//     public async Task<bool> AddAsync(Event e)
//     {
//         try
//         {
//             await _context.Events.AddAsync(e);
//             return true;
//         }
//         catch
//         {
//             return false;
//         }
//     }

//     public Task<bool> DeleteAsync(Event e)
//     {
//         try
//         {
//             _context.Events.Remove(e);
//             return Task.FromResult(true);
//         }
//         catch
//         {
//             return Task.FromResult(false);
//         }
//     }

//     public async Task<Event?> FindByIdAsync(string id)
//     {
//         return await _context.Events
//         .Where(e => e.Id == id)
//         .Include(e => e.Books)
//         .SingleOrDefaultAsync();
//     }

//     public async Task<IList<Event>> ListAllAsync()
//     {
//         return await _context.Events.ToListAsync();
//     }

//     public async Task<bool> SaveAsync()
//     {
//         try
//         {
//             if (await _context.SaveChangesAsync() > 0) return true;
//             return false;
//         }
//         catch
//         {
//             return false;
//         }
//     }

//     public Task<bool> UpdateAsync(Event e)
//     {
//         try
//         {
//             _context.Events.Update(e);
//             return Task.FromResult(true);
//         }
//         catch
//         {
//             return Task.FromResult(false);
//         }
//     }
// }