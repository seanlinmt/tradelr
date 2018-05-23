#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace tradelr.Tests.Helpers
{
    public class TestHelpers
    {
        public long sessionValue;
        public Mock<HttpContextBase> httpContext;
        public Mock<HttpRequestBase> request;
        public Mock<HttpResponseBase> response;
        public Mock<HttpSessionStateBase> session;
        public Mock<HttpServerUtilityBase> server;
        public HttpCookieCollection cookies;

        public TestHelpers()
        {
            sessionValue = -1;
            httpContext = new Mock<HttpContextBase>();
            request = new Mock<HttpRequestBase>();
            response = new Mock<HttpResponseBase>();
            session = new Mock<HttpSessionStateBase>();
            server = new Mock<HttpServerUtilityBase>();
            cookies = new HttpCookieCollection();
        }
        public void FakeHttpContext()
        {
            FakeHttpContext(new NameValueCollection(), null, null);
        }

        public void FakeHttpContext(string[] cookieNames)
        {
            FakeHttpContext(new NameValueCollection(), null, cookieNames);
        }

        public void FakeHttpContext(long? pid)
        {
             FakeHttpContext(new NameValueCollection(), pid, null);
        }

        public void FakeHttpContext(NameValueCollection queryString, long? pid, string[] cookieNames)
        {
            response.Setup(x => x.Cookies).Returns(cookies);
            
            httpContext.Setup(x => x.Server).Returns(server.Object);
            httpContext.Setup(x => x.Session).Returns(session.Object);
            httpContext.Setup(x => x.Request).Returns(request.Object);
            httpContext.Setup(x => x.Response).Returns(response.Object);

            httpContext.SetupGet(x => x.Request.QueryString).Returns(queryString);
            httpContext.SetupGet(x => x.Request.Url).Returns(new Uri("http://127.0.0.1:22222/"));
            var writer = new StringWriter();
            var wr = new SimpleWorkerRequest("", "","","", writer);
            HttpContext.Current = new HttpContext(wr);

            // session
            if (pid.HasValue)
            {
                httpContext.SetupGet(x => x.Session["id"]).Returns(pid);
            }
            else
            {
                httpContext.SetupGet(x => x.Session["id"]).Returns(sessionValue);
                httpContext.SetupSet(x => x.Session["id"] = It.IsAny<long>())
                    .Callback((string name, object val) =>
                                  {
                                      sessionValue = (long)val;
                                      httpContext.SetupGet(x => x.Session["id"]).Returns(sessionValue);
                                  });
            }

            // cookie
            if (cookieNames != null)
            {
                foreach (var name in cookieNames)
                {
                    cookies.Add(new HttpCookie(name,""));
                }
            }

            // httpcontext
            httpContext.SetupGet(x => x.Request.HttpMethod).Returns("GET");

        }

        public void SetHttpMethod(string httpMethod)
        {
            httpContext.SetupGet(x => x.Request.HttpMethod).Returns(httpMethod);
        }

        public static void AssertRoute(RouteCollection routes, string url, object expectations)
        {
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                .Returns(url);

            RouteData routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData, "Should have found the route");

            foreach (PropertyValue property in GetProperties(expectations))
            {
                Assert.IsTrue(string.Equals(property.Value.ToString(),
                                            routeData.Values[property.Name].ToString(),
                                            StringComparison.OrdinalIgnoreCase)
                              , string.Format("Expected '{0}', not '{1}' for '{2}'.",
                                              property.Value, routeData.Values[property.Name], property.Name));
            }
        }

        private static IEnumerable<PropertyValue> GetProperties(object o)
        {
            return from prop in TypeDescriptor.GetProperties(o).Cast<PropertyDescriptor>()
                   let val = prop.GetValue(o)
                   where val != null
                   select new PropertyValue { Name = prop.Name, Value = val };
        }

        private sealed class PropertyValue
        {
            public string Name {get;set;}
            public object Value {get;set;}
        }
    }
}