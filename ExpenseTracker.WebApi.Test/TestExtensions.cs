using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace ExpenseTracker.WebApi.Test
{
    public static class TestExtensions
    {
        public static T SetupController<T>(this T controller) where T : ControllerBase
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpContextMock.SetupGet(x => x.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(x => x.Response).Returns(httpResponseMock.Object);

            controller.ControllerContext = new ControllerContext(new ActionContext
            {
                HttpContext = httpContextMock.Object,
                RouteData = new RouteData(),
                ActionDescriptor = new Mock<ControllerActionDescriptor>().Object
            });

            controller.MetadataProvider = new Mock<IModelMetadataProvider>().Object;
            controller.ModelBinderFactory = new Mock<IModelBinderFactory>().Object;
            controller.ObjectValidator = new Mock<IObjectModelValidator>().Object;
            controller.Url = new Mock<IUrlHelper>().Object;
            return controller;
        }

    }
}