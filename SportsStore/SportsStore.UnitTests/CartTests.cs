using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using SportsStore.Domain.Abstract;
using Moq;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            // Arrange - 테스트할 상품들을 생성한다..
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - 새로운 카트를 생성한다.
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // Arrange - 테스트할 상품들을 생성한다..
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            // Arrange - 새로운 카트를 생성한다.
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // Arrange - 테스트할 상품들을 생성한다..
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            // Arrange - 새로운 카트를 생성한다.
            Cart target = new Cart();

            // Arrange - 카트에 상품들을 추가한다.
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Act
            target.RemoveLine(p2);

            // Assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            // Arrange - 테스트할 상품들을 생성한다..
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - 새로운 카트를 생성한다.
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            // Assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // Arrange - 테스트할 상품들을 생성한다..
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            // Arrange - 새로운 카트를 생성한다.
            Cart target = new Cart();

            // Arrange - 카트에 상품들을 추가한다.
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            // Act - 카트를 초기화시킨다.
            target.Clear();

            // Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
            }.AsQueryable());

            // Arrange - Cart 개체를 생성한다.
            Cart cart = new Cart();

            // Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object, null);

            // Act - 카트에 상품을 추가한다.
            target.AddToCart(cart, 1, null);

            // Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // Arrange - Mock 리파지토리를 생성한다.
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "P1", Category = "Apples" },
            }.AsQueryable());

            // Arrange - Cart 개체를 생성한다.
            Cart cart = new Cart();

            // Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(mock.Object, null);

            // Act - 카트에 상품을 추가한다.
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            // Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            // Arrange - Cart 개체를 생성한다.
            Cart cart = new Cart();

            // Arrange - 컨트롤러를 생성한다.
            CartController target = new CartController(null, null);

            // Act - Index 액션 메서드를 호출한다.
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange - Mock  주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - 빈 Cart 개체를 생성한다.
            Cart cart = new Cart();
            // Arrange - 배송 정보를 생성한다.
            ShippingDetails shippingDetails = new ShippingDetails();
            // Arrange = 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);

            // Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // Assert - 주문이 주문 처리기에 전달되지 않은 것을 확인한다.
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - 메서드가 기본 뷰를 반환했는지 확인한다.
            Assert.AreEqual("", result.ViewName);
            // Assert - 유효하지 않은 모델을 뷰에 전달하는지 확인한다.
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - Mock 주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - 하나의 상품이 담긴 Cart 개체를 생성한다.
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);
            // Arrange - 모델에 오류를 추가한다.
            target.ModelState.AddModelError("error", "error");

            // Act - 지불 처리를 시도한다.
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - 주문이 주문 처리기에 전달되지 않은 것을 확인한다.
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - 메서드가 기본 뷰를 반환했는지 확인한다.
            Assert.AreEqual("", result.ViewName);
            // Assert - 유효하지 않은 모델을 뷰에 전달하는지 확인한다.
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - Mock 주문 처리기를 생성한다.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            // Arrange - 하나의 상품이 담긴 Cart 개체를 생성한다.
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Arrange - 컨트롤러의 인스턴스를 생성한다.
            CartController target = new CartController(null, mock.Object);

            // Act - 지불 처리를 시도한다.
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert - 주문 처리기에 주문이 전달된 것을 확인한다.
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            // Assert - 메서드가 Completed 뷰를 반환하는지 확인한다.
            Assert.AreEqual("Completed", result.ViewName);
            // Assert - 유효한 모델을 뷰에 전달하는지 확인한다.
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
