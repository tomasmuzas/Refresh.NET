using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Refresh.Components.Migrations;

namespace Refresh.Tool
{
    public class MigrationLoader
    {
        public static IMigration FromPath(string path)
        {
            var migrationCode = File.ReadAllText(path);

            var sourceText = SourceText.From(migrationCode);
            var migrationTree = SyntaxFactory.ParseSyntaxTree(sourceText);

            var refs = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Threading.Tasks, Version=4.0.10.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
                MetadataReference.CreateFromFile(typeof(IMigration).Assembly.Location)
            };

            var referencedAssemblies =typeof(IMigration).Assembly.GetReferencedAssemblies();
            refs.AddRange(referencedAssemblies
                .Select(assembly => 
                    MetadataReference.CreateFromFile(Assembly.Load(assembly).Location)));

            var assemblyPath = "migration.dll";
            var result = CSharpCompilation
                .Create(
                    "assembly",
                    new[] { migrationTree },
                    references: refs,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                        optimizationLevel: OptimizationLevel.Release,
                        assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default))
                .Emit(assemblyPath);

            var migrationAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath)); ;

            var migrationType = migrationAssembly.GetTypes()
                .First(t => t.GetInterfaces().Contains(typeof(IMigration)));

            return (IMigration)Activator.CreateInstance(migrationType);
        }
    }
}
