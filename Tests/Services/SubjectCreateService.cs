using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using Tests.Models;

namespace Tests.Services
{
    public class SubjectCreateService
    {
        private readonly Aplicationdb _context;

        public SubjectCreateService(Aplicationdb context)
        {
            _context = context;
        }

        public bool CreateSubject(SubjectViewModel model)
        {
            if (model != null)
            {
                _context.Subjects.Add(model);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<SubjectViewModel> GetSubjects()
        {
            return _context.Subjects.Include(s => s.Tests).ToList();
        }
    }
}