using System;
using System.Collections.Generic;
using System.Text;

namespace Fixi.Tests.IntegrationTest.Helpers
{
    public static class HttpClientExtensions
    {
        public static void SetAdminToken(this HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("X-Test-Role");
            client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");
        }

        public static void SetUserToken(this HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("X-Test-Role");
            client.DefaultRequestHeaders.Add("X-Test-Role", "User");
        }

        public static void SetManagerToken(this HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("X-Test-Role");
            client.DefaultRequestHeaders.Add("X-Test-Role", "Manager");
        }

        public static void SetTechnicianToken(this HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("X-Test-Role");
            client.DefaultRequestHeaders.Add("X-Test-Role", "Technician");
        }

        public static void SetDepartmentIdClaim(this HttpClient client, string departmentId)
        {
            client.DefaultRequestHeaders.Remove("X-Test-DeptId");
            client.DefaultRequestHeaders.Add("X-Test-DeptId", departmentId);
        }

    }
}
