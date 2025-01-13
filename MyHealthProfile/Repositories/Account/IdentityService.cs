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


namespace MyHealthProfile.Repositories.Account
{
    public class IdentityService : IIdentityService
    {
        private readonly ICurrentUserService _currentUserService;
        //private readonly AppDbContext _appDbContext;
        private readonly UserManager<Patient> _userManager;
        private readonly IConfiguration _configuration;
        public IdentityService(/*AppDbContext appDbContext, */
            ICurrentUserService currentUserService, UserManager<Patient> userManager, IConfiguration configuration)
        {
            // _appDbContext = appDbContext;
            _userManager = userManager;
            _configuration = configuration;
            _currentUserService = currentUserService;
        }


        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto register)
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


            var tokenString = GenerateJWTToken(model.Email/*, user.Role*/);



            return await Task.FromResult(new TokenResponse(tokenString, DateTime.UtcNow.AddHours(1), user.Id));
        }



        public Task<TokenResponse> GetUserProfile(LoginDto register)
        {
            throw new NotImplementedException();
        }

        public Task<TokenResponse> UpdateUserProfile(LoginDto register)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ForgetPasswordAsync(string Email)
        {
            if (Email == null) throw new BadRequestException("email shoud be provide");
            var user = await _userManager.FindByEmailAsync(Email.Trim().Normalize());
            _ = user ?? throw new NotFoundException("Email not found");
            if (user.Id != _currentUserService.UserId) throw new BadRequestException("Not Authorized");
            await _userManager.RemovePasswordAsync(user);
            string otp = GenerateOtp();
            user.PasswordOtpExpiration = DateTime.UtcNow.AddMinutes(10);
            user.PasswordOtp = otp;
            user.IsPaswordSet = false;
            await _userManager.UpdateAsync(user);

            return true;
        }
        public async Task<bool> setNewPasswordAsync(ResetPasswordRequestDto request)
        {
            var validator = new ResetPasswordRequestValidator().Validate(request);
            if (!validator.IsValid) throw new ValidationException(validator.Errors);
            return await setPasswordAsync(request);

        }






        private async Task<bool> setPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
            _ = user ?? throw new NotFoundException("Email not found");

            //if (!await _userManager.HasPasswordAsync(user)) throw new ForbiddenAccessException("No password to be chagned");
            if (!user.IsEmailVerified) throw new ForbiddenAccessException("Email is not  Verified");
            if (user == null || user.PasswordOtp != request.OPT || user.PasswordOtpExpiration < DateTime.UtcNow) throw new ValidationException("OTP", "Code is not valid");
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, request.Password);
            if (!result.Succeeded) throw result.ToValidationException();
            user.PasswordOtp = string.Empty;
            user.PasswordOtpExpiration = null;
            user.IsPaswordSet = true;

            await _userManager.UpdateAsync(user);
            return true;



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


        private string GenerateJWTToken(string Email /*,string Role*/)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("7KGXiGLRklf5Fa3jEo2ZS7HOKs1YurR0YRPcZVgspzg"/*_configuration.GetSection("JWT:TokenKey").Value!*/);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, Email));
            ///claims.Add(new Claim(ClaimTypes.Role, Role));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "MyHealthProfile",//_configuration.GetSection("JWT:Issuer").Value,
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

