using Moq;
using NBench;
using ProjectManagement.DataLayer;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProjectManagement.PerformanceTest
{
    public class DataLayerPerformanceTests
    {
        private List<User> output = null;
        Mock<DbSet<User>> mockUsers;
        List<User> users;
        ProjectManagementDataConnector connector;
        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            users = new List<User>
            {
                new User { ID=1, FirstName="First-Name-01", LastName="Last-Name-01", EmployeeID=1},
                new User { ID=2, FirstName="First-Name-02", LastName="Last-Name-02", EmployeeID=2},
                new User { ID=3, FirstName="First-Name-03", LastName="Last-Name-03", EmployeeID=3}
            };
            var source = users.AsQueryable();
            mockUsers = new Mock<DbSet<User>>();
            mockUsers.As<IQueryable<User>>().Setup(m => m.Expression).Returns(source.Expression);
            mockUsers.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mockUsers.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
            mockUsers.As<IQueryable<User>>().Setup(m => m.Provider).Returns(source.Provider);

            var model = new Mock<ProjectManagementDataModel>();
            model.Setup(x => x.Users).Returns(mockUsers.Object);
            model.Setup(x => x.Database.Initialize(It.IsAny<bool>()));
            connector = new ProjectManagementDataConnector(model.Object);
        }

        [PerfBenchmark(Description = "Project Manager Performance Test", NumberOfIterations = 5,
            RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000, TestMode = TestMode.Measurement)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void GetUsers_MemoryMesaurement()
        {
            output = connector.GetAllUsers().ToList();
        }

        [PerfBenchmark(Description = "Project Manager Performance Test", NumberOfIterations = 5,
            RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000, TestMode = TestMode.Measurement)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        public void GetUsers_GcMesaurement()
        {
            output = connector.GetAllUsers().ToList();
        }

        [PerfBenchmark(Description = "Project Manager Performance Test", NumberOfIterations = 1,
           RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000, TestMode = TestMode.Measurement)]
        [ElapsedTimeAssertion(MaxTimeMilliseconds = 2000)]
        public void GetUsers_ElapsedTimeAssertion()
        {
            output = connector.GetAllUsers().ToList();
        }
        [PerfCleanup]
        public void Cleanup()
        {
            if (output != null)
                output = null;
        }
    }
}
