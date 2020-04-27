# Refresh.NET
A simple CLI tool to easily migrate .NET projects by writing custom migrations.

This tool is built with intention to create shareable migrations to be able to easily adapt to library, framework or even language updates.

# Supported migrations:
* Rename class, interface, method, member, namespace
* Add, remove, change, reorder arguments
* Replace class, method
* Change method return type

⚠️Disclaimer! At this point, only references are refactored! Support for end-to-end refactoring (with declarations) is coming soon.

# Usage

Install the tool by using:

`dotnet tool install -g refresh.tool`

Use the tool:

`refresh -p Path/To/Project.csproj -m Path/To/Migration.cs`

# Migration file

Install Refresh.Components Nuget package in order to get Intelli-Sense support:

`Install-Package Refresh.Components`

`Migration.cs` can be any C\# class, implementing [IMigration](blob/master/src/Refresh.Components/Migrations/IMigration.cs) and it can have any name (Migration, MyMigration, CustomMigration etc.).

## IMigration interface
```csharp
SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context);
```

`SyntaxTree` is just `Microsoft.CodeAnalysis.SyntaxTree` without any alterations. This is an AST of a file being migrated.

`MigrationContext` - context that stores types of AST nodes. Needs to be updated after every refactoring.

## MigrationBuilder

In order to simplify standard migrations, `MigrationBuilder` provides fluent API for buidling a `CompositeMigration`.

MigrationBuilder API:
```csharp
RenameClass(string type, string newName);
RenameInterface(string type, string newName);
RenameMethod(Method method, string newName);
RenameMember(string type, string memberName, string newName);
RenameNamespace(string namespaceName, string newName);

ReplaceClass(string oldType, string newType);
ReplaceMethod(Method method, string newMethodName);

AddArguments(Method method, List<PositionalArgument> arguments);
ChangeArguments(Method method, string argumentName, List<PositionalArgument> newArgument);
RemoveArguments(Method method, List<int> positions);
ReorderArguments(Method method, List<int> newArgumentOrder);

ChangeMethodReturnType(Method method, string newReturnType);
ChangeMemberType(string type, string memberName, string newType);

SplitMethod(Method method, Method newMethod1, Method newMethod2);
```

Overloads are provided in order to build `Method`, `Argument` and `List<PositionalArgument>` types.

Usage:
```csharp
new MigrationBuilder()
      .RenameClass("Namespace.TestClass", "NewClass")
      .RenameMethod(m => m
          .OfClass("Namespace.NewClass")
          .WithName("TestMethod"), 
          "NewMethod")
      .AddArguments(m => m
          .OfClass("Namespace.NewClass")
          .WithName("NewMethod"),
          a => a
              .OfType("string")
              .WithDefaultValueExpression("\"this is a new string value\"")
              .AtPosition(1))
      .Build()
```
