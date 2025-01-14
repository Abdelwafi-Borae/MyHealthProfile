using Microsoft.AspNetCore.Identity;


using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyHealthProfile.Models;
using MyHealthProfile.Models.Dtos;
using Data.ModelViews;
using Validators;
using MyHealthProfile.Common.Exceptions;
using MyHealthProfile.Common.Extensions;
using Microsoft.AspNetCore.Identity.Data;
using System.Net.Mail;
using Azure.Core;
using System;
using MyHealthProfile.Services.Interfaces;
using static System.Net.WebRequestMethods;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MyHealthProfile.Persistence;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using AutoMapper;


namespace MyHealthProfile.Repositories.Account
{
    public class IdentityService : IIdentityService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<Patient> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IFileManager _fileManager;
          
        public IdentityService(ApplicationDbContext appDbContext, IFileManager fileManager,
        ICurrentUserService currentUserService, UserManager<Patient> userManager, IConfiguration configuration, IMapper mapper)
        {
            _fileManager = fileManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _configuration = configuration;
            _currentUserService = currentUserService;
        }


        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto register, IFormFile file)
        {

            var validator = new RegisterVAlidato().Validate(register);
            if (!validator.IsValid) throw new ValidationException(validator.Errors);
            string otp = GenerateOtp();

            Patient applicationUser = new()
            {
                UserName = register.Email,
                Email = register.Email,
                Address = register.Address,
                PhoneNumber = register.PhoneNumber,
                Name = register.Name,
                Gender = register.Gender,
                Nationality = register.Nationality,
                DateOfBirth = register.DateOfBirth,
                EmailOtp = otp,
                EmailOtpExpiration = DateTime.UtcNow.AddMinutes(5)
            };
            string LogoURL = _fileManager.CreateUpdateFile(file,"");
            applicationUser.PhotoUrl = LogoURL;

            var existingUser = await GetExistingUser(applicationUser);
            if (existingUser is not null)
            {
                throw new AlreadyExistException("Email", "Email already exist");

            }
            //image url logic not yet

            IdentityResult result = await _userManager.CreateAsync(applicationUser, register.Password);
            if (!result.Succeeded) throw result.ToValidationException();

            await SendOtpEmailAsync(applicationUser.Email, otp);

            return new RegisterResponseDto
            {
                UserId = applicationUser.Id,
                Message = "accoun created seccessfely ,confirm your account"

            }
            ;



        }

        public async Task<string> AccountVerivicationAsync(string userId, string otp)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.EmailOtp != otp || user.EmailOtpExpiration < DateTime.UtcNow)
            {
                return "Invalid or expired OTP.";
            }

            user.IsEmailVerified = true;
            user.EmailConfirmed = true;
            user.EmailOtp = null; // Clear OTP
            user.EmailOtpExpiration = null;
            await _userManager.UpdateAsync(user);

            return "Email verified successfully.";

        }
        public async Task<TokenResponse> LoginAsync(LoginDto model)
        {

            var validator = new LoginVAlidator().Validate(model);
            if (!validator.IsValid) throw new ValidationException(validator.Errors);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim().Normalize());
            _ = user ?? throw new ForbiddenAccessException("Invalid Credentials");
            if (!user.IsEmailVerified) throw new ForbiddenAccessException("Account Not Verfied");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                throw new ForbiddenAccessException("Invalid Credentials");


            var tokenString = GenerateJWTToken(model.Email);



            return await Task.FromResult(new TokenResponse(tokenString, DateTime.UtcNow.AddHours(1), user.Id));
        }



        public async Task<PatientDto> GetUserProfile()
        {

            var xx = _currentUserService.Email;
            var x = await _userManager.FindByEmailAsync(_currentUserService.Email);
            return _mapper.Map<PatientDto>(x);

        }

        public async Task<PatientDto> UpdateUserProfile(PatientUpdateDTO request, IFormFile? file)
        {
            var user = await _userManager.FindByEmailAsync(_currentUserService.Email);
            _ = user ?? throw new NotFoundException("Patient Not exist");
            user.PhoneNumber = request.Phone ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;
            if (file != null && file.Length > 0)
            {
                string LogoURL =_fileManager.CreateUpdateFile(file, user.PhotoUrl);
                user.PhotoUrl = LogoURL;
            }
            var result = await _userManager.UpdateAsync(user);
            _ = result ?? throw new NotFoundException("update failed");

            return _mapper.Map<PatientDto>(user);

            throw new NotImplementedException();
        }
        public async Task<string> ForgetPasswordAsync(string Email)
        {
            if (Email == null) throw new BadRequestException("email shoud be provide");
            var user = await _userManager.FindByEmailAsync(Email.Trim().Normalize());
            _ = user ?? throw new NotFoundException("Email not found");
            //if (user.Id != _currentUserService.UserId) throw new BadRequestException("Not Authorized");
            //await _userManager.RemovePasswordAsync(user);
            string otp = GenerateOtp();
            user.PasswordOtpExpiration = DateTime.UtcNow.AddMinutes(10);
            user.PasswordOtp = otp;
            user.IsPaswordSet = false;
            await _userManager.UpdateAsync(user);
            await SendOtpEmailAsync(Email, otp);
            return "OPT Sent To your email To Set New Password";
        }
        public async Task<string> setNewPasswordAsync(ResetPasswordRequestDto request)
        {
            var validator = new ResetPasswordRequestValidator().Validate(request);
            if (!validator.IsValid) throw new ValidationException(validator.Errors);
            return await setPasswordAsync(request);

        }






        private async Task<string> setPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
            _ = user ?? throw new NotFoundException("Email not found");

            //if (!await _userManager.HasPasswordAsync(user)) throw new ForbiddenAccessException("No password to be chagned");
            if (user.IsPaswordSet) throw new ForbiddenAccessException("Password  Already Set ");
            if (user == null || user.PasswordOtp != request.OPT || user.PasswordOtpExpiration < DateTime.UtcNow) throw new ValidationException("OTP", "Code is not valid");
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, request.Password);
            if (!result.Succeeded) throw result.ToValidationException();
            user.PasswordOtp = string.Empty;
            user.PasswordOtpExpiration = null;
            user.IsPaswordSet = true;

            var Updateresult = await _userManager.UpdateAsync(user);
            if (!Updateresult.Succeeded) throw result.ToValidationException();
            return "New Password Set Successfuley";



        }

        private string GenerateOtp()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[4]; // 4 bytes = 32 bits
                rng.GetBytes(randomBytes);
                int randomValue = BitConverter.ToInt32(randomBytes, 0);

                // Ensure the number is positive and within the 6-digit range
                return (Math.Abs(randomValue) % 900000 + 100000).ToString();
            }
        }



        private async Task SendOtpEmailAsync(string email, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["SmtpSettings:FromName"], _configuration["SmtpSettings:Username"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify Your Email";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP is: {otp}"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Accept all certificates (useful for development only, remove in production)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // Connect with STARTTLS
                await client.ConnectAsync(_configuration["SmtpSettings:Host"], int.Parse(_configuration["SmtpSettings:Port"]), SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);

                // Send email
                await client.SendAsync(message);

                // Disconnect
                await client.DisconnectAsync(true);
            }
        }


        private string GenerateJWTToken(string Email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:TokenKey").Value!);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, Email));


            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration.GetSection("JWT:Issuer").Value,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
        private async Task<Patient> GetExistingUser(Patient user)
        {
            var exitingUser = await _userManager.FindByEmailAsync(user.Email.Normalize());


            return exitingUser;
        }


    }
}

