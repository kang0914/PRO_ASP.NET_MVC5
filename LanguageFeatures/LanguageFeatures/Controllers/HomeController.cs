using LanguageFeatures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace LanguageFeatures.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Navigate to a URL to show an example";
        }

        public ViewResult AutoProperty()
        {
            // 새로운 Product 개체를 생성한다.
            Product myProduct = new Product();

            // 속성 값을 설정한다.
            myProduct.Name = "Kayak";

            // 속성 값을 가져온다.
            string productName = myProduct.Name;

            // 뷰를 생성한다.
            return View("Result", (object)String.Format("Product name: {0}", productName));
        }

        public ViewResult CreateCollection()
        {
            string[] stringArray = { "apple", "orange", "plum" };

            List<int> intList = new List<int> { 10, 20, 30, 40};

            Dictionary<string, int> myDict = new Dictionary<string, int> {
                { "apple", 10}, { "orange", 20}, { "plum", 30 }
            };

            return View("Result", (object)stringArray[1]);
        }


        //public ViewResult UseExtension()
        //{
        //    // ShoppingCart 개체를 생성하고 속성 값을 설정한다.
        //    ShoppingCart cart = new ShoppingCart
        //    {
        //        Products = new List<Product>
        //        {
        //            new Product {Name = "Kayak", Price = 275M },
        //            new Product {Name = "Lifejacket", Price = 48.95M },
        //            new Product {Name = "soccer ball", Price = 19.50M },
        //            new Product {Name = "Corner flag", Price = 34.95M },
        //        }
        //    };

        //    // 카트에 담긴 제품들의 합계 값을 구한다.
        //    decimal cartTotal = cart.TotalPrices();

        //    return View("Result", (object)String.Format("Total: {0:c}",cartTotal));
        //}

        public ViewResult UseExtensionEnumerable()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product>
                {
                    new Product { Name = "Kayak", Price = 275M },
                    new Product { Name = "Lifejacket", Price = 48.95M },
                    new Product { Name = "Soccer ball", Price = 19.50M },
                    new Product { Name = "Corner flag", Price = 34.95M },
                }
            };

            // Product 개체들의 배열을 생성하고 데이터를 채워 넣는다.
            Product[] productArray =
            {
                new Product { Name = "Kayak", Price = 275M },
                new Product { Name = "Lifejacket", Price = 48.95M },
                new Product { Name = "Soccer ball", Price = 19.50M },
                new Product { Name = "Corner flag", Price = 34.95M },
            };

            // 카트에 존재하는 제품들의 합계를 구한다.
            decimal cartTotal = products.TotalPrice();
            decimal arrayTotal = productArray.TotalPrice();

            return View("Result", (object)String.Format("Cart Total: {0}, Array Total: {1}", cartTotal, arrayTotal));
        }

        public ViewResult UseFilterExtensionMethod()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product>
                {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
                }
            };

            decimal total = 0;
            foreach(Product prod in products.FilterByCategory("Soccer"))
            {
                total += prod.Price;
            }

            return View("Result", (object)String.Format("Totla: {0}", total));
        }

        public ViewResult UseFilterExtensionMethod2()
        {
            IEnumerable<Product> products = new ShoppingCart
            {
                Products = new List<Product>
                {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
                }
            };

            // 1. 델리게이트
            //Func<Product, bool> categoryFilter = delegate (Product prod)
            //{
            //    return prod.Category == "Soccer";
            //};

            // 2. 람다식
            //Func<Product, bool> categoryFilter = prod => prod.Category == "Soccer";

            decimal total = 0;


            //foreach (Product prod in products.Filter(categoryFilter))

            // 3. 람다식
            //foreach (Product prod in products.Filter(prod => prod.Category == "Soccer"))

            // 4. 람다식 확장
            foreach (Product prod in products.Filter(prod => prod.Category == "Soccer" || prod.Price > 20))
            {
                total += prod.Price;
            }

            return View("Result", (object)String.Format("Totla: {0}", total));
        }

        // 익명 형식 개체
        public ViewResult CreateAnonArray()
        {
            var oddsAndEnds = new[]
            {
                new {Name = "MVC", Category = "Pattern" },
                new {Name = "Hat", Category = "Clothing" },
                new {Name = "Apple", Category = "Fruit" },
            };

            StringBuilder result = new StringBuilder();
            foreach(var item in oddsAndEnds)
            {
                result.Append(item.Name).Append(" ");
            }

            return View("Result", (object)result.ToString());
        }

        // LINQ
        public ViewResult FindProductsNotLinq()
        {
            Product[] products =
            {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
            };

            // 결과를 담아둘 배열을 정의한다.
            Product[] foundProducts = new Product[3];

            // 배열의 내용을 정리한다.
            Array.Sort(products, (item1, item2) => {
                return Comparer<decimal>.Default.Compare(item1.Price, item2.Price);
            });

            // 배열에서 첫 번째 세 개의 항목들을 결과로 얻는다.
            Array.Copy(products, foundProducts, 3);

            // 결과를 작성한다.
            StringBuilder result = new StringBuilder();
            foreach(Product p in foundProducts)
            {
                result.AppendFormat("Price: {0} ", p.Price);
            }

            return View("Result", (object)result.ToString());
        }

        public ViewResult FindProducts()
        {
            Product[] products =
            {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
            };

            // 1. 질의 구문(Query Syntax)
            //var foundProducts = from match in products
            //                    orderby match.Price descending
            //                    select new { match.Name, match.Price };

            // 2. 마침표 표기법(Dot Notation)
            var foundProducts = products.OrderByDescending(e => e.Price)
                                        .Take(3)
                                        .Select(e => new { e.Name, e.Price });

            // 결과를 작성한다.
            int count = 0;
            StringBuilder result = new StringBuilder();
            foreach (var p in foundProducts)
            {
                result.AppendFormat("Price: {0} ", p.Price);
                if (++count == 3)
                {
                    break;
                }
            }

            return View("Result", (object)result.ToString());
        }

        // 지연된 LINQ 질의
        public ViewResult FindProducts2()
        {
            Product[] products =
            {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
            };

            var foundProducts = products.OrderByDescending(e => e.Price)
                                        .Take(3)
                                        .Select(e => new { e.Name, e.Price });

            products[2] = new Product { Name = "Stadium", Price = 79600M };

            StringBuilder result = new StringBuilder();
            foreach (var p in foundProducts)
            {
                result.AppendFormat("Price: {0} ", p.Price);
            }

            return View("Result", (object)result.ToString());
        }

        // 즉시 수행되는 LINQ 질의
        public ViewResult FindProducts3()
        {
            Product[] products =
            {
                    new Product { Name = "Kayak", Category ="Watersports", Price = 275M },
                    new Product { Name = "Lifejacket", Category ="Watersports", Price = 48.95M },
                    new Product { Name = "Soccer ball", Category ="Soccer", Price = 19.50M },
                    new Product { Name = "Corner flag", Category ="Soccer", Price = 34.95M },
            };

            var foundProducts = products.OrderByDescending(e => e.Price)
                                        .Take(3)
                                        .Select(e => new { e.Name, e.Price });

            var results = products.Sum(e => e.Price);

            products[2] = new Product { Name = "Stadium", Price = 79600M };

            return View("Result", (object)string.Format("Sum: {0:c}", results));
        }
    }
}