// using System;
// using System.Linq;
// using System.Reflection;
// using System.Text;
// using Domain.Response;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Microsoft.CodeAnalysis.Text;
// using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;
//
// namespace Application.Infrastructure.Injection;
//
// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
// public class ServiceLifeTimeAttribute : Attribute
// {
//     public readonly ENUM_LIFE_TYPE LifeType;
//     public readonly Type TypeOfInterface;
//     public ServiceLifeTimeAttribute(ENUM_LIFE_TYPE lifeType, Type typeOfInterface = null)
//     {
//         LifeType = lifeType;
//         TypeOfInterface = typeOfInterface;
//     }
// }
//
// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
// public class ServiceLifeTimeFactoryAttribute : Attribute
// {
//     public readonly ENUM_LIFE_TYPE LifeType;
//     public readonly Type Resolver;
//     public readonly Type TypeOfInterface;
//     public readonly Type ImplementType;
//     public readonly MethodInfo MethodInfo; 
//
//     public ServiceLifeTimeFactoryAttribute(ENUM_LIFE_TYPE lifeType, Type typeOfInterface, Type implementType, MethodInfo methodInfo)
//     {
//         LifeType = lifeType;
//         TypeOfInterface = typeOfInterface;
//         ImplementType = implementType;
//         MethodInfo = methodInfo;
//     }
// }
//
// public enum ENUM_LIFE_TYPE
// {
//     Singleton,
//     Transient,
//     Scope,
// }
//
// public class InjectionGenerator : ISourceGenerator
// {
//     public void Initialize(InitializationContext context)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Execute(SourceGeneratorContext context)
//     {
//         var attributeSymbol = context.Compilation.GetTypeByMetadataName(typeof(ServiceLifeTimeFactoryAttribute).FullName);
//         var classWithAttributes = context.Compilation.SyntaxTrees.Where(st => st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
//             .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));
//         
//         foreach (SyntaxTree tree in classWithAttributes)
//         {
//             var semanticModel = context.Compilation.GetSemanticModel(tree);
//                 
//             foreach(var declaredClass in tree
//                         .GetRoot()
//                         .DescendantNodes()
//                         .OfType<ClassDeclarationSyntax>()
//                         .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
//             {
//                 var nodes = declaredClass
//                     .DescendantNodes()
//                     .OfType<AttributeSyntax>()
//                     .FirstOrDefault(a => a.DescendantTokens().Any(dt => dt.IsKind(SyntaxKind.IdentifierToken) && semanticModel.GetTypeInfo(dt.Parent).Type.Name == attributeSymbol.Name))
//                     ?.DescendantTokens()
//                     ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
//                     ?.ToList();
//
//                 if(nodes == null)
//                 {
//                     continue;
//                 }
//
//                 var relatedClass = semanticModel.GetTypeInfo(nodes.Last().Parent);
//
//                 var generatedClass = this.GenerateClass(relatedClass);
//
//                 foreach(MethodDeclarationSyntax classMethod in declaredClass.Members.Where(m => m.IsKind(SyntaxKind.MethodDeclaration)).OfType<MethodDeclarationSyntax>())
//                 {
//                     this.GenerateMethod(declaredClass.Identifier, relatedClass, classMethod, ref generatedClass);
//                 }
//
//                 this.CloseClass(generatedClass);
//
//                 context.AddSource($"{declaredClass.Identifier}_{relatedClass.Type.Name}", SourceText.From(generatedClass.ToString(), Encoding.UTF8));
//             }
//         }
//     }
//     
//     private void GenerateMethod(SyntaxToken moduleName, TypeInfo relatedClass, MethodDeclarationSyntax methodDeclaration, ref StringBuilder builder)
//     {
//         var signature = $"{methodDeclaration.Modifiers} {relatedClass.Type.Name} {methodDeclaration.Identifier}(";
//
//         var parameters = methodDeclaration.ParameterList.Parameters.Skip(1);
//
//         signature += string.Join(", ", parameters.Select(p => p.ToString())) + ")";
//
//         var methodCall = $"return this._wrapper.{moduleName}.{methodDeclaration.Identifier}(this, {string.Join(", ", parameters.Select(p => p.Identifier.ToString()))});";
//
//         builder.AppendLine(@"
//         " + signature + @"
//         {
//             " + methodCall + @"
//         }");
//     }
//
//     private StringBuilder GenerateClass(TypeInfo relatedClass)
//     {
//         var sb = new StringBuilder();
//
//         sb.Append(@"
// using System;
// using System.Collections.Generic;
// using SpeedifyCliWrapper.Common;
//
// namespace SpeedifyCliWrapper.ReturnTypes
// {
//     public partial class " + relatedClass.Type.Name);
//
//         sb.Append(@"{");
//
//         return sb;
//     }
//
//     private void CloseClass(StringBuilder generatedClass)
//     {
//         generatedClass.Append(@"}}");
//     }
// }