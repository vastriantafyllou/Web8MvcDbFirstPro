using Microsoft.EntityFrameworkCore;
using SchoolApp.Core.Enums;
using SchoolApp.Data;
using SchoolApp.Models;
using System.Linq.Expressions;

namespace SchoolApp.Repositories;

    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(Mvc8DbProContext context) : base(context)
        {
        }

        public async Task<Teacher?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await context.Teachers
                .Where(t => t.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync(); // fetch many results, return the first or null
        }

        public async Task<List<Course>> GetTeacherCoursesAsync(int teacherId)
        {
            List<Course> courses;

            courses = await context.Teachers
                .Where(t => t.Id == teacherId)
                .SelectMany(t => t.Courses)
                .ToListAsync();

            return courses;

        }

        public async Task<List<User>> GetAllUsersTeachersAsync()
        {
            var usersWithRoleTeacher = await context.Users
                .Where(u => u.UserRole == UserRole.Teacher)
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher
                .ToListAsync();

            return usersWithRoleTeacher;
        }

        public async Task<User?> GetUserTeacherByUsernameAsync(string username)
        {
            var userTeacher = await context.Users
                .Where(u => u.Username == username && u.UserRole == UserRole.Teacher)
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher
                .SingleOrDefaultAsync(); // fetch 0 ή 1 results, otherwise throws an exception
            
            return userTeacher;
        }
        
        public async Task<PaginatedResult<User>> GetPaginatedUsersTeachersAsync(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            
            var usersWithRoleTeachers = await context.Users
                .Where(u => u.UserRole == UserRole.Teacher)
                .Include(u => u.Teacher) // Εager loading της σχετικής οντότητας Teacher
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            
            int totalRecords = await context.Users
                .Where(u => u.UserRole == UserRole.Teacher)
                .CountAsync();
            
            return new PaginatedResult<User>(usersWithRoleTeachers, totalRecords, pageNumber, pageSize);
        }
        
        public async Task<PaginatedResult<User>> GetPaginatedUsersTeachersFilteredAsync(int pageNumber, int pageSize, 
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users
                .Where(u => u.UserRole == UserRole.Teacher)
                .Include(u => u.Teacher); // Εager loading της σχετικής οντότητας Teacher

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);  // εκτελείται, υπονοείται το AND 
                }
            }
            int totalRecords = await query.CountAsync(); // εκτελειται το query για να μετρησει τα αποτελεσματα
            int skip = (pageNumber - 1) * pageSize;
            
            var data = await query
                .OrderBy(u => u.Id) // παντα να υπάρχει OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελειται το query για να φερει τα αποτελεσματα
            
            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }
    }

