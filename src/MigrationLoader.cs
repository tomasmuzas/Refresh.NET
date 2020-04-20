using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace LibAdapter
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
