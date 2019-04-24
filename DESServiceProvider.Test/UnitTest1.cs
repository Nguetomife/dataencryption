using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DESServiceProvider.Test
{
    using DataEncryptionStandardServiceProvider;

    /// <summary>The test des service provider.</summary>
    [TestClass]
    public class TestDesServiceProvider
    {
        /// <summary>The original string_ is_the_ same_ after_encrypted and decripted.</summary>
        [TestMethod]
        public void OriginalStringIsTheSameAfterEncryptedAndDecripted()
        {
            var encryptedString = DataEncryptionServiceProvider.EncryptString("Nguenkam");
            var decryptedString = DataEncryptionServiceProvider.DecryptString(encryptedString);
            Assert.AreEqual("Nguenkam", decryptedString);
        }
    }
}
