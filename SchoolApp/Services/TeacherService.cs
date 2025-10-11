using AutoMapper;
using SchoolApp.Core.Enums;
using SchoolApp.Data;
using SchoolApp.Dto;
using SchoolApp.Exceptions;
using SchoolApp.Repositories;
using SchoolApp.Security;
using Serilog;

namespace SchoolApp.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<TeacherService> logger = new LoggerFactory().AddSerilog().CreateLogger<TeacherService>();

        public TeacherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task SignUpUserAsync(TeacherSignupDto request)
        {
            Teacher teacher = ExtractTeacher(request);
            User user = ExtractUser(request); ;

            try
            {
                //user = ExtractUser(request);
                User? existingUsername = await unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);

                if (existingUsername != null)
                {
                    throw new EntityAlreadyExistsException("User", "User with username " +
                        existingUsername.Username + " already exists");
                }

                User? existingUserEmail = await unitOfWork.UserRepository.GetUserByEmailAsync(user.Email);
                if (existingUserEmail != null)
                {
                    throw new EntityAlreadyExistsException("User", "User with email " +
                        existingUserEmail.Email + " already exists");
                }

                user.Password = EncryptionUtil.Encrypt(user.Password);
                await unitOfWork.UserRepository.AddAsync(user);

    
                //if (await unitOfWork.TeacherRepository.GetByPhoneNumberAsync(teacher.PhoneNumber) is not null)
                //{
                //    throw new EntityAlreadyExistsException("Teacher", "Teacher with phone number " +
                //        teacher.PhoneNumber + " already exists");
                //}

                await unitOfWork.TeacherRepository.AddAsync(teacher);
                user.Teacher = teacher;
                // teacher.User = user; EF manages the other-end of the relationship since both entities are attached

                await unitOfWork.SaveAsync();
                logger.LogInformation("Teacher {Teacher} signed up successfully.", teacher);        // ToDo toString in Teacher
            }
            catch (EntityAlreadyExistsException ex)
            {
                logger.LogError("Error signing up tecaher {Teacher}. {Message}", teacher, ex.Message);
                throw;
            }
        }

        private User ExtractUser(TeacherSignupDto signupDTO)
        {
            return new User()
            {
                Username = signupDTO.Username!,
                Password = signupDTO.Password!,
                Email = signupDTO.Email!,
                Firstname = signupDTO.Firstname!,
                Lastname = signupDTO.Lastname!,
                //UserRole = signupDTO.UserRole
                UserRole = UserRole.Teacher
            };
        }

        private Teacher ExtractTeacher(TeacherSignupDto signupDTO)
        {
            return new Teacher()
            {
                PhoneNumber = signupDTO.PhoneNumber,
                Institution = signupDTO.Institution!
            };
        }
    }
}