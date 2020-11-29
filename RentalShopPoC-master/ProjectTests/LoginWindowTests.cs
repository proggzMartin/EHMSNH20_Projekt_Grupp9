using Store;
using System;
using System.Reflection;
using Xunit;

namespace ProjectTests
{
    public class LoginWindowTests// : LoginWindow
    {
        [Fact]
        public void GetEnteredCustomer_ShouldThrowException()
        {
            //Arrange
            //TestHelper.CreateInMemDb();

            //MethodInfo[] methodInfos = typeof(LoginWindow).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            //FieldInfo[] fieldInfos= typeof(LoginWindow).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            var y = typeof(LoginWindow).GetMethod("GetEnteredCustomer", BindingFlags.NonPublic  | BindingFlags.Instance) 
                        ?? throw new Exception("Couldn't fins method to test; check that it's not changed name or is removed.");

            //var x = methodInfos[0];
            //Act
            //Assert
        }



        //[Fact]
        //public void Skeleton()
        //{
        //    //Arrange
        //    //Act
        //    //Assert
        //}
    }
}
