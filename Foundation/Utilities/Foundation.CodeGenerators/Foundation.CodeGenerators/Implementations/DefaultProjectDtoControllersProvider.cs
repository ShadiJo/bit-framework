﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Foundation.CodeGenerators.Contracts;
using Foundation.CodeGenerators.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Foundation.CodeGenerators.Implementations
{
    public class DefaultProjectDtoControllersProvider : IProjectDtoControllersProvider
    {
        public virtual IList<DtoController> GetProjectDtoControllersWithTheirOperations(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            IList<DtoController> dtoControllers = new List<DtoController>();

            foreach (Document doc in project.Documents)
            {
                if (!doc.SupportsSemanticModel)
                    continue;

                SyntaxNode root = doc.GetSyntaxRootAsync(CancellationToken.None).Result;

                List<ClassDeclarationSyntax> dtoControllersClassDecs = new List<ClassDeclarationSyntax>();

                foreach (ClassDeclarationSyntax classDeclarationSyntax in root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>())
                {
                    if (classDeclarationSyntax.BaseList == null)
                        continue;

                    bool isController = classDeclarationSyntax.Identifier.ValueText != "DtoSetController" && classDeclarationSyntax.BaseList.Types.OfType<SimpleBaseTypeSyntax>()
                                        .Any(t =>
                                        {
                                            string baseName = (t.Type as GenericNameSyntax)?.Identifier.ValueText;
                                            return baseName == "DtoController" || baseName == "DtoSetController";
                                        });

                    if (isController == true)
                        dtoControllersClassDecs.Add(classDeclarationSyntax);
                }

                if (!dtoControllersClassDecs.Any())
                    continue;

                SemanticModel semanticModel = doc.GetSemanticModelAsync().Result;

                foreach (ClassDeclarationSyntax dtoControllerClassDec in dtoControllersClassDecs)
                {
                    INamedTypeSymbol controllerSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(dtoControllerClassDec);

                    DtoController dtoController = new DtoController
                    {
                        ControllerSymbol = controllerSymbol,
                        Name = controllerSymbol.Name.Replace("Controller", string.Empty),
                        Operations = new List<ODataOperation>(),
                        ModelSymbol = controllerSymbol.BaseType.TypeArguments.Single(t => t.IsDto())
                    };

                    dtoControllers.Add(dtoController);

                    foreach (MethodDeclarationSyntax methodDecSyntax in dtoControllerClassDec.DescendantNodes().OfType<MethodDeclarationSyntax>())
                    {
                        IMethodSymbol methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(methodDecSyntax);

                        ImmutableArray<AttributeData> attrs = methodSymbol.GetAttributes();

                        AttributeData actionAttribute = attrs.SingleOrDefault(att => att.AttributeClass.Name == "ActionAttribute");

                        AttributeData functionAttribute = attrs.SingleOrDefault(att => att.AttributeClass.Name == "FunctionAttribute");

                        if (actionAttribute == null && functionAttribute == null)
                            continue;

                        ODataOperation operation = new ODataOperation
                        {
                            Method = methodSymbol,
                            Kind = actionAttribute != null ? ODataOperationKind.Action : ODataOperationKind.Function,
                            ReturnType = methodSymbol.ReturnType
                        };

                        operation.Parameters = operation.Method.GetAttributes()
                            .Where(att => att.AttributeClass.Name == "ParameterAttribute")
                            .Select(parameterAttribute => new ODataOperationParameter
                            {
                                Name = parameterAttribute.ConstructorArguments[0].Value.ToString(),
                                Type = ((INamedTypeSymbol)parameterAttribute.ConstructorArguments[1].Value),
                                IsOptional = parameterAttribute.ConstructorArguments.Count() == 3 && object.Equals(parameterAttribute.ConstructorArguments[2].Value, true)
                            }).ToList();

                        dtoController.Operations.Add(operation);
                    }
                }
            }

            return dtoControllers;
        }
    }
}
