using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MlsExclusive.Models;
using MlsExclusive.Utilites;

namespace TestMlsExclusive
{
    [TestClass]
    public class CryptTest
    {
        [TestMethod]
        public void TestCryptEncryptString()
        {
            string password = "vile goblin";
            string test_string = "Test me";
            string test_encrypt = Crypt.EncryptString(test_string, password);
            
            string test_decrypt = Crypt.DecryptString(test_encrypt, password);
            Assert.AreEqual(test_string, test_decrypt);
        }
    }
}
