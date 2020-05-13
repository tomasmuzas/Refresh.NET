using Refresh.Tool.SampleMigrations;
using Xunit;

namespace Refresh.Components.Test
{
    public class CSharp7To8MigrationTests : MigrationTestBase
    {
        [Fact]
        public void It_Migrates_UsingDeclaration()
        {
            var source = @"
            namespace Test
            {
                using System.IO;
                public class Program
                {
                    public void Main()
                    {
                        var b = new object();
                        using (var something = new System.IO.MemoryStream())
                        {
                            var a = true;
                            var str = """";
                            
                            something.Flush();
                        }
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                using System.IO;
                public class Program
                {
                    public void Main()
                    {
                        var b = new object();
                        using var something = new System.IO.MemoryStream();
                            var a = true;
                            var str = """";
                            
                            something.Flush();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new CSharp7to8(),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }


        [Fact]
        public void It_Migrates_SwitchStatement_ToExpression()
        {
            var source = @"
             namespace Test
            {
                public enum Rainbow
                {
                    Red,
                    Orange,
                    Yellow,
                    Green,
                    Blue,
                    Indigo,
                    Violet
                }
                
                public class Program
                {
                    public static string FromRainbowClassic(Rainbow colorBand)
                    {
                        switch (colorBand)
                        {
                            case Rainbow.Red:
                                return ""red"";
                            case Rainbow.Orange:
                                return ""orange"";
                            case Rainbow.Yellow:
                                return ""yellow"";
                            default:
                                return null;
                        }
                    }
                }
            }";

            var expected = @"
             namespace Test
            {
                public enum Rainbow
                {
                    Red,
                    Orange,
                    Yellow,
                    Green,
                    Blue,
                    Indigo,
                    Violet
                }
                
                public class Program
                {
                    public static string FromRainbowClassic(Rainbow colorBand)
                    {
                        colorBand switch
                        {
                            Rainbow.Red => ""red"",
                            Rainbow.Orange => ""orange"",
                            Rainbow.Yellow => ""yellow"",
                            _ => null
                        };
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new CSharp7to8(), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}