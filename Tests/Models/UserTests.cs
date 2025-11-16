using System;
using System.Text.Json;
using RecruitmentSystem.API.Models;
using Xunit;

namespace RecruitmentSystem.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void NewUser_HasExpectedDefaultValues()
        {
            var before = DateTime.UtcNow;
            var user = new User();
            var after = DateTime.UtcNow;

            Assert.Equal(0, user.Id);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.PasswordHash);

            // Enum default is 0 (no defined named value in the enum has 0)
            Assert.Equal(default(UserRole), user.Role);

            // CreatedAt should be set to a recent UTC time (allow small slack)
            Assert.InRange(user.CreatedAt, before.AddSeconds(-2), after.AddSeconds(2));
        }

        [Fact]
        public void JsonSerialization_RoundTrips_User()
        {
            var original = new User
            {
                Id = 42,
                Email = "candidate@example.com",
                PasswordHash = "hashed",
                Role = UserRole.Candidate,
                CreatedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<User>(json);

            Assert.NotNull(deserialized);
            Assert.Equal(original.Id, deserialized.Id);
            Assert.Equal(original.Email, deserialized.Email);
            Assert.Equal(original.PasswordHash, deserialized.PasswordHash);
            Assert.Equal(original.Role, deserialized.Role);

            // DateTime round-trip should preserve Ticks with System.Text.Json; allow small tolerance to be robust
            Assert.InRange(deserialized.CreatedAt, original.CreatedAt.AddSeconds(-1), original.CreatedAt.AddSeconds(1));
        }
    }
}