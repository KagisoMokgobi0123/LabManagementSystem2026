using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NMB_HLabSys.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;


        public OtpService(
            ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<string> GenerateOtpAsync(
            string userId)
        {
            /*
             * Generate a 6 digit OTP
             */
            var otp =
                Random.Shared
                .Next(100000, 999999)
                .ToString();



            var otpRecord = new PasswordResetOtp
            {
                ApplicationUserId = userId,

                OTP = otp,

                ExpiryDate =
                    DateTime.UtcNow
                    .AddMinutes(10),

                IsUsed = false,

                CreatedDate =
                    DateTime.UtcNow
            };



            await _context.PasswordResetOtps
                .AddAsync(otpRecord);



            await _context.SaveChangesAsync();



            return otp;
        }





        public async Task<bool> ValidateOtpAsync(
            string userId,
            string otp)
        {
            var otpRecord =
                await _context.PasswordResetOtps
                .Where(x =>
                    x.ApplicationUserId == userId &&
                    x.OTP == otp &&
                    !x.IsUsed)
                .OrderByDescending(x =>
                    x.CreatedDate)
                .FirstOrDefaultAsync();



            if (otpRecord == null)
            {
                return false;
            }



            if (otpRecord.ExpiryDate <
                DateTime.UtcNow)
            {
                return false;
            }



            return true;
        }


        public async Task MarkOtpAsUsedAsync(
            string userId,
            string otp)
        {
            var otpRecord =
                await _context.PasswordResetOtps
                .Where(x =>
                    x.ApplicationUserId == userId &&
                    x.OTP == otp)
                .OrderByDescending(x =>
                    x.CreatedDate)
                .FirstOrDefaultAsync();



            if (otpRecord == null)
            {
                return;
            }



            otpRecord.IsUsed = true;


            await _context.SaveChangesAsync();
        }
    }
}