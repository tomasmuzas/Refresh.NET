using System;
using System.IO;
using System.Linq;
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

            var refs = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IMigration).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(SyntaxTree).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            };

            var result = CSharpCompilation
                .Create(
                    "assembly",
                    new[] { migrationTree },
                    references: refs,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                        optimizationLevel: OptimizationLevel.Release,
                        assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default))
                .Emit("assembly.dll");

            var a = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath("assembly.dll")); ;

            var migrationType = a.GetTypes()
                .First(t => t.GetInterfaces().Contains(typeof(IMigration)));

            return (IMigration)Activator.CreateInstance(migrationType);
        }
    }
}
