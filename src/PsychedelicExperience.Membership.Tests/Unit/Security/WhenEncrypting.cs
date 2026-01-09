using Newtonsoft.Json;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.Users.Domain;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Unit.Security
{
    public class WhenEncrypting
    {
        [Fact]
        public void ThenEncryptionShouldSucceed()
        {
            var original = "dqs654sdq65qs4d65sqd465sdq46";

            var key = EncryptionKey.New();
            var data = key.Encrypt(original);
            var result = key.Decrypt(data);

            result.ShouldBe(original);
        }

        [Fact]
        public void ThenEncryptionShouldSucceedWithOriginal()
        {
            var original = "dqs654sdq65qs4d65sqd465sdq46";

            var key = EncryptionKey.New();
            var data = key.Encrypt(original);

            var serialized = JsonConvert.SerializeObject(key);

            var key2 = JsonConvert.DeserializeObject<EncryptionKey>(serialized);
            var result = key2.Decrypt(data);

            result.ShouldBe(original);
        }
    }
}
