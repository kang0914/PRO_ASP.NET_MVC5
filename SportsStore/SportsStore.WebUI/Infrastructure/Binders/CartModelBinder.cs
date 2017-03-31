using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionkey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        { 
            // 세션에서 Cart 개체 가져오기
            Cart cart = null;
            if(controllerContext.HttpContext.Session != null)
            {
                cart = (Cart)controllerContext.HttpContext.Session[sessionkey];
            }

            // 세션 데이터에 Cart 개체가 없다면 새로 생성한다.
            if(cart == null)
            {
                cart = new Cart();
                if(controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionkey] = cart;
                }
            }

            // Cart 개체를 반환한다.
            return cart;
        }
    }
}