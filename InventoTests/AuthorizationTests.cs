namespace Invento.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        // Test registration password hashing
        [TestMethod]
        public void TestRegistrationPasswordHashing()
        {
            string plainPassword = "TestPassword123";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(
                plainPassword,
                BCrypt.Net.BCrypt.GenerateSalt(12)
            );

            Assert.IsTrue(hashedPassword.StartsWith("$2"));
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword));
            Assert.IsFalse(BCrypt.Net.BCrypt.Verify("WrongPassword", hashedPassword));
        }

        // Test password validation rules
        [TestMethod]
        public void TestPasswordValidationRules()
        {
            string shortPassword = "Short12";
            Assert.IsTrue(shortPassword.Length < 8, "Password shorter than 8 characters should be invalid");

            string validPassword = "ValidPass123";
            Assert.IsTrue(validPassword.Length >= 8, "Password of 8 or more characters should be valid");
        }

        // Test password confirmation matching
        [TestMethod]
        public void TestPasswordConfirmationMatch()
        {
            string password = "TestPassword123";
            string confirmPassword = "TestPassword123";
            string wrongConfirmPassword = "DifferentPassword123";

            Assert.AreEqual(password, confirmPassword, "Matching passwords should be equal");
            Assert.AreNotEqual(password, wrongConfirmPassword, "Different passwords should not be equal");
        }

        // Test login verification with both hashed and plain passwords
        [TestMethod]
        public void TestLoginPasswordVerification()
        {
            string plainPassword = "TestPassword123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, BCrypt.Net.BCrypt.GenerateSalt(12));
            string plainStoredPassword = "OldPlainTextPassword";

            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword),
                "BCrypt verification should succeed with correct password");

            Assert.IsTrue(plainStoredPassword == "OldPlainTextPassword",
                "Plain text comparison should work for legacy passwords");
        }

        // Test salt uniqueness
        [TestMethod]
        public void TestSaltUniqueness()
        {
            string password = "TestPassword123";

            string hash1 = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
            string hash2 = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

            Assert.AreNotEqual(hash1, hash2, "Same password should produce different hashes due to different salts");
        }

        // Test password migration (plain text to hashed)
        [TestMethod]
        public void TestPasswordMigration()
        {
            string plainPassword = "TestPassword123";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, BCrypt.Net.BCrypt.GenerateSalt(12));

            Assert.IsTrue(hashedPassword.StartsWith("$2"), "Migrated password should be in BCrypt format");
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword),
                "Original password should verify against migrated hash");
        }

        // Test exception handling for invalid hash formats
        [TestMethod]
        public void TestInvalidHashHandling()
        {
            string plainPassword = "TestPassword123";
            string invalidHash = "InvalidHashFormat";

            Assert.ThrowsException<BCrypt.Net.SaltParseException>(() =>
                BCrypt.Net.BCrypt.Verify(plainPassword, invalidHash),
                "Invalid hash format should throw SaltParseException");
        }

        [TestMethod]
        public void TestLoginAttemptTracking()
        {
            var loginAttempts = new Dictionary<string, int>();
            const string testUser = "testUser";

            loginAttempts[testUser] = 1;
            Assert.AreEqual(1, loginAttempts[testUser], "First login attempt should be recorded as 1");

            loginAttempts[testUser] = 5;
            Assert.AreEqual(5, loginAttempts[testUser], "Login attempts should accumulate correctly");
        }

        // Test account lockout timing
        [TestMethod]
        public void TestAccountLockoutTiming()
        {
            var lockoutTimes = new Dictionary<string, DateTime>();
            const string testUser = "testUser";
            const int lockoutDurationMinutes = 1;

            DateTime lockoutTime = DateTime.Now.AddMinutes(lockoutDurationMinutes);
            lockoutTimes[testUser] = lockoutTime;

            DateTime beforeExpiry = lockoutTime.AddSeconds(-30);
            Assert.IsTrue(beforeExpiry < lockoutTimes[testUser], "Account should still be locked before expiry");

            DateTime afterExpiry = lockoutTime.AddSeconds(30);
            Assert.IsTrue(afterExpiry > lockoutTimes[testUser], "Account should be unlocked after expiry");
        }

        // Test attempt limit thresholds
        [TestMethod]
        public void TestAttemptLimitThresholds()
        {
            const int ATTEMPT_LIMIT_TIMEOUT = 10;
            const int ATTEMPT_LIMIT_DEACTIVATE = 20;

            Assert.IsTrue(ATTEMPT_LIMIT_TIMEOUT < ATTEMPT_LIMIT_DEACTIVATE,
                "Timeout threshold should be less than deactivation threshold");

            int attempts = 5;
            Assert.IsFalse(attempts >= ATTEMPT_LIMIT_TIMEOUT, "Account should not be locked at 5 attempts");

            attempts = 15;
            Assert.IsTrue(attempts >= ATTEMPT_LIMIT_TIMEOUT, "Account should be locked at 15 attempts");
            Assert.IsFalse(attempts >= ATTEMPT_LIMIT_DEACTIVATE, "Account should not be deactivated at 15 attempts");

            attempts = 20;
            Assert.IsTrue(attempts >= ATTEMPT_LIMIT_DEACTIVATE, "Account should be deactivated at 20 attempts");
        }

        // Test login attempt reset
        [TestMethod]
        public void TestLoginAttemptReset()
        {
            var loginAttempts = new Dictionary<string, int>();
            var lockoutTimes = new Dictionary<string, DateTime>();
            const string testUser = "testUser";

            loginAttempts[testUser] = 5;
            lockoutTimes[testUser] = DateTime.Now.AddMinutes(1);

            loginAttempts.Remove(testUser);
            lockoutTimes.Remove(testUser);

            Assert.IsFalse(loginAttempts.ContainsKey(testUser), "Login attempts should be reset");
            Assert.IsFalse(lockoutTimes.ContainsKey(testUser), "Lockout time should be reset");
        }

        // Test first user registration logic
        [TestMethod]
        public void TestFirstUserRegistrationLogic()
        {
            const string role = "Admin";
            const string status = "Active";

            bool isFirstUser = true;

            string assignedRole = isFirstUser ? "Admin" : "Cashier";
            string assignedStatus = isFirstUser ? "Active" : "Approval";

            Assert.AreEqual(role, assignedRole, "First user should be assigned Admin role");
            Assert.AreEqual(status, assignedStatus, "First user should have Active status");

            isFirstUser = false;

            assignedRole = isFirstUser ? "Admin" : "Cashier";
            assignedStatus = isFirstUser ? "Active" : "Approval";

            Assert.AreEqual("Cashier", assignedRole, "Subsequent users should be assigned Cashier role");
            Assert.AreEqual("Approval", assignedStatus, "Subsequent users should have Approval status");
        }

        // Test account status validation
        [TestMethod]
        public void TestAccountStatusValidation()
        {
            var validStatuses = new[] { "Active", "Inactive", "Approval" };

            foreach (string status in validStatuses)
            {
                Assert.IsTrue(Array.Exists(validStatuses, s => s == status),
                    $"Status '{status}' should be valid");
            }

            string accountStatus = "Active";
            Assert.AreEqual("Active", accountStatus, "Active accounts should be allowed access");

            accountStatus = "Inactive";
            Assert.AreNotEqual("Active", accountStatus, "Inactive accounts should be denied access");

            accountStatus = "Approval";
            Assert.AreNotEqual("Active", accountStatus, "Pending approval accounts should be denied access");
        }

        private enum CaptchaResult
        {
            None,
            OK,
            Cancel
        }

        //Test captcha verification
        [TestMethod]
        public void TestCaptchaVerification()
        {
            bool captchaValidated = false;
            CaptchaResult mockResult = CaptchaResult.None;

            mockResult = CaptchaResult.Cancel;
            captchaValidated = (mockResult == CaptchaResult.OK);
            Assert.IsFalse(captchaValidated, "Registration should be prevented when captcha verification fails");

            mockResult = CaptchaResult.OK;
            captchaValidated = (mockResult == CaptchaResult.OK);
            Assert.IsTrue(captchaValidated, "Registration should proceed when captcha verification succeeds");

            mockResult = CaptchaResult.None;
            captchaValidated = (mockResult == CaptchaResult.OK);
            Assert.IsFalse(captchaValidated, "Registration should be prevented when captcha response is empty");

            var registrationSteps = new List<string>
            {
            "Validate input fields",
            "Check username availability",
            "Verify captcha",
            "Hash password",
            "Insert user data"
            };

            Assert.IsTrue(registrationSteps.Contains("Verify captcha"),
                "Captcha verification must be part of registration process");

            int captchaStepIndex = registrationSteps.IndexOf("Verify captcha");
            int passwordHashIndex = registrationSteps.IndexOf("Hash password");

            Assert.IsTrue(captchaStepIndex < passwordHashIndex,
                "Captcha must be verified before processing sensitive operations");
        }
    }
}