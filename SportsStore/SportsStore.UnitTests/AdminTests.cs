using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
            });

            // Arrage - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);

            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
            });

            // Arrange - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);

            // Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            // Assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Can_Edit_Nonexistent_Product()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1"},
                new Product { ProductID = 2, Name = "P2"},
                new Product { ProductID = 3, Name = "P3"},
            });

            // Arrange - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);

            // Act
            Product result = (Product)target.Edit(4).ViewData.Model;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);
            // Arrange - 상품을 생성한다.
            Product product = new Product { Name = "Test" };

            // Act - 상품 저장을 시도한다.
            ActionResult result = target.Edit(product);

            // Assert - 리파지토리가 호출되었는지 확인한다.
            mock.Verify(m => m.SaveProduct(product));
            // Assert - 메서드의 반환 형식을 확인한다.
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);
            // Arrange - 상품을 생성한다.
            Product product = new Product { Name = "Test" };
            // Arrange - 모델 상태에 오류를 추가한다.
            target.ModelState.AddModelError("error", "error");

            // Act - 상품 저장을 시도한다.
            ActionResult result = target.Edit(product);

            // Assert - 리파지토리가 호출되지 않았는지 확인한다.
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // Assert - 메서드의 반환 형식을 확인한다.
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            // Arrange - 상품을 생성한다.
            Product prod = new Product { ProductID = 2, Name = "Test" };

            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1" },
                prod,
                new Product { ProductID = 3, Name = "P3" },
            });

            // Arrange - 컨트롤러를 생성한다.
            AdminController target = new AdminController(mock.Object);

            // Act - 상품을 삭제한다.
            target.Delete(prod.ProductID);

            // Assert - 리파지토리의 DeleteProduct 메서드가
            // 올바른 상품과 함께 호출되었는지 확인한다.
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
