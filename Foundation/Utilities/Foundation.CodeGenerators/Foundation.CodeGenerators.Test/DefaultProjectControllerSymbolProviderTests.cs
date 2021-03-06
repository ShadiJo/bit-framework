﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation.CodeGenerators.Implementations;
using Foundation.CodeGenerators.Model;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.CodeGenerators.Test
{
    [TestClass]
    public class DefaultProjectDtoControllersProviderTests : CodeGeneratorTest
    {
        [TestMethod]
        public virtual async Task DefaultProjectDtoControllersProviderShouldReturnODataControllersAsDesired()
        {
            using (Workspace workspace = GetWorkspace())
            {
                Solution solution = workspace.CurrentSolution;

                IList<DtoController> controllers = new DefaultProjectDtoControllersProvider()
                    .GetProjectDtoControllersWithTheirOperations(solution.Projects.Single(p => p.Name == "Foundation.Api"));

                Assert.IsTrue(
                    controllers.Select(c => c.Name).SequenceEqual(new[] { "ClientsLogs", "JobsInfo", "UsersSettings" }));
            }
        }

        [TestMethod]
        public virtual async Task DefaultProjectDtoControllersProviderShouldFindODataOperationParametersCorrectly()
        {
            const string sourceCodeOfDtoControllerWithActionAndParameter = @"
using System;
using System.Threading.Tasks;
using System.Web.OData;
using Foundation.Api.ApiControllers;
using Foundation.Test.Model.DomainModels;

namespace Foundation.Api.ApiControllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ParameterAttribute : Attribute
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public bool IsOptional { get; private set; }

        public ParameterAttribute(string name, Type type, bool isOptional = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            IsOptional = isOptional;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ActionAttribute : Attribute
    {

    }
}

public interface IDto {
}

public class TestModel : IDto {
}

namespace Foundation.Test.Api.ApiControllers
{
    public class TestController : DtoController<TestModel>
    {
        [Action]
        [Parameter(""Parameter1"", typeof(string), isOptional : true )]
        [ParameterAttribute(""Parameter1"", typeof(string) , isOptional : false )]
        [Foundation.Api.ApiControllers.Parameter(""Parameter1"", typeof(string))]
        [Foundation.Api.ApiControllers.ParameterAttribute(""Parameter1"", typeof(string))]
        public virtual async Task Do(ODataActionParameters actionParameters)
        {

        }
    }
}
";

            IList<DtoController> controllers = new DefaultProjectDtoControllersProvider()
                    .GetProjectDtoControllersWithTheirOperations(CreateProjectFromSourceCodes(sourceCodeOfDtoControllerWithActionAndParameter));

            Assert.AreEqual(true, controllers.Single()
                .Operations.Single()
                .Parameters.First().IsOptional);

            Assert.IsTrue(controllers.Single()
                .Operations.Single()
                .Parameters.Skip(1)
                .All(p => p.IsOptional == false));
        }
    }
}
