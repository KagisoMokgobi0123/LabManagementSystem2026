using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMB_HLabSys.Data
{
    public class PasswordResetOtp
    {
        [Key]
        public int Id { get; set; }

        // ==============================
        // USER
        // ==============================

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;


        // ==============================
        // OTP
        // ==============================

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; } = string.Empty;


        // ==============================
        // EXPIRATION
        // ==============================

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }


        // ==============================
        // OTP STATE
        // ==============================

        public bool IsUsed { get; set; } = false;


        // ==============================
        // SECURITY / ATTEMPTS
        // ==============================

        public int FailedAttempts { get; set; } = 0;


        // ==============================
        // HELPER PROPERTIES
        // ==============================

        [NotMapped]
        public bool IsExpired =>
            DateTime.UtcNow > ExpiresAt;

        [NotMapped]
        public bool IsValid =>
            !IsUsed && !IsExpired;
    }
}