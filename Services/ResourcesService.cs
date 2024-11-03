using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test_menu.Data;
using Test_menu.Models;

namespace Test_menu.Services
{
    public class ResourcesService
    {
        private readonly ApplicationDbContext _context;

        public ResourcesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Resource>> GetAllResources()
        {
            return await _context.Resources.ToListAsync();
        }
        public async Task<Resource> GetResourceById(int id)
        {
            return await _context.Resources.FindAsync(id);
        }
        public async Task<bool> Create(Resource resource)
        {
            try
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Edit(int id, Resource resource)
        {
            if (id != resource.Id)
            {
                return false;
            }

            try
            {
                _context.Update(resource);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(resource.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.Id == id);
        }
    }
}
