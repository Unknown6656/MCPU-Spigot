using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Collections;
using System;

using MCPUCompiler.Core.Testing;
using MCPUCompiler.Core;
using Piglet.Parser;

namespace MCPUCompilerUnitTests
{
    using static SyntaxTree;
    using static Compiler;
    using static Parser;
    using static Lexer;

    using Program = FSharpList<SyntaxTree.Declaration>;


    [TestClass]
    public class TestCases
    {
        private const ASMCommentLevel clvl = ASMCommentLevel.CommentAll;
        private static readonly IParser<object> p = Parser;


        private static T Try<T>(Func<T> f, bool shouldfail)
        {
            try
            {
                T res = f();

                return shouldfail ? throw new AssertFailedException("The test should fail - but it did not") : res;
            }
            catch (Exception ex)
            when (!(ex is AssertFailedException))
            {
                if (shouldfail)
                    return default(T);
                else
                    throw;
            }
        }

        private static (string, string) TestCompileCode(string code, bool shouldfail = false) => (Try(() =>
        {
            var prog = p.Parse(code) as Program;
            var sares = Analyze(prog);
            var compl = new ASMBuilder(sares);
            var res = compl.BuildClass(prog);
            var emt = new Emitter(res);
            var asm = emt.Merge();
            var outp = emt.Generate(clvl);

            return string.Join("\n", outp);
        }, shouldfail), code);


        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_00() => TestCompileCode("", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_01() => TestCompileCode(@"
void main(void)
{
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_02() => TestCompileCode(@"
int g1;
bool g2;

void main(int p1, bool p2)
{
    bool l1;
}

void helper(bool p1)
{
    int l1;
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_03() => TestCompileCode(@"
int g1;
bool g2;

void main(int p1, bool p2)
{
    bool g2;
}

void helper(bool p1)
{
    int l1;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_04() => TestCompileCode(@"
void main(int p1)
{
    int p1;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_05() => TestCompileCode(@"
void main(int p1)
{
    int i;
    {
        int i;

        printi(i);
    }

    printi(i);
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_06() => TestCompileCode(@"
void main(void)
{
    {
        int i;

        printi(i);
    }
    {
        int i;

        printi(i);
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_07() => TestCompileCode(@"
int g1;
bool g1;

main : void -> void
{
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_08() => TestCompileCode(@"
main : int p1, int p1 -> void
{
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Trivial_09() => TestCompileCode(@"
main : void -> void
{
    bool l1;
    bool l1;
}
", true);

        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_01() => TestCompileCode(@"
void main(void)
{
    __asm ""nop"";
    __asm ""test:"";
    __asm ""abk"";
    __asm ""br test"";
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_02() => TestCompileCode(@"
void main(void)
{
    __asm ""
        halt
    "";
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_03() => TestCompileCode(@"
void main(void)
{
    int i;

    for (i = 0; i < 10; ++i)
        break;
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_04() => TestCompileCode(@"
void main(void)
{
    int i;

    for (i = 0; i < 10; ++i)
        continue;
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_05() => TestCompileCode(@"
void main(void)
{
    int i;

    while (i < 10)
        break;
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_06() => TestCompileCode(@"
void main(void)
{
    int i;

    while (i < 10)
    {
        ++i;

        continue;
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_07() => TestCompileCode(@"
void main(void)
{
    break;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_08() => TestCompileCode(@"
void main(void)
{
    continue;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Statements_09() => TestCompileCode(@"
void main(void)
{
    halt;
    abk;
}
");

        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_01() => TestCompileCode(@"
void main(void)
{
    int i;

    i = 0;

    while (i < 10)
    {
        printi(i);

        ++i;
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_02() => TestCompileCode(@"
void main(void)
{
    bool b;
    int i;

    b = false;
    i = 0;

    while (i < 20)
    {
        if (b)
            i = i + 1;
        else
            i = i > 2 ? i >> 1 : i;

        b = !b;
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_03() => TestCompileCode(@"
void main(void)
{
    int i;

    for (i = 0; i < 9; ++i)
        printi(i);
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_04() => TestCompileCode(@"
void main(void)
{
    for each (int k -> int v in io)
    {
        printi(k);
        printi(v);
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_05() => TestCompileCode(@"
void main(void)
{
    for each (k -> v in io)
    {
        printi(k);
        printi(v);
    }
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_06() => TestCompileCode(@"
void main(void)
{
    for each (int v in io)
        printi(v);
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Loops_07() => TestCompileCode(@"
void main(void)
{
    for each (v in io)
        printi(v);
}
");

        [TestMethod]
        public (string, string) Test_CodeCompile_Functional_01() => TestCompileCode(@"
main : void -> void
{
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Functional_02() => TestCompileCode(@"
let gl0 : bool;
let gl1 : int;

main : int i, int j, bool b -> void
{
    let test : bool;
    let loc : int;
}

helper : int k -> bool
{
    return k != 0;
}
");

        [TestMethod]
        public (string, string) Test_CodeCompile_Operators_01() => TestCompileCode(@"
main : void -> void
{
    int i;
    int j;
    bool r0;
    bool r1;
    bool r2;
    bool r3;
    bool r4;
    bool r5;
    int r6;
    int r7;
    int r8;
    int r9;
    int ra;
    int rb;
    int rc;
    int rd;
    int re;
    int rf;
    int r10;
    int r11;
    int r12;
    int r13;
    int r14;
    int r15;
    int r16;
    int r17;
    int r18;
    bool r19;

    i = 42;
    j = -315;

    r0 = i != j;
    r1 = i < j;
    r2 = i <= j;
    r3 = i > j;
    r4 = i >= j;
    r5 = i == j;
    r6 = i & j;
    r7 = i && j;
    r8 = i | j;
    r9 = i || j;
    ra = i << j;
    rb = i <<< j;
    rc = i >> j;
    rd = i >>> j;
    re = i ^ j;
    rf = ~j;
    r10 = i + j;
    r11 = i * j;
    r12 = i / j;
    r13 = i % j;
    r14 = i - j;
    r15 = i ** j;
    r16 = i % j;
    r17 = ++i;
    r18 = --j;
    r19 = !r0;
}
");

        [TestMethod]
        public (string, string) Test_CodeCompile_Types_01() => TestCompileCode(@"
main : void -> int
{
    return false;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Types_02() => TestCompileCode(@"
main : bool b -> void
{
    b + 5;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Types_03() => TestCompileCode(@"
main : int -> int
{
    return 0;
}
", true);
        [TestMethod]
        public (string, string) Test_CodeCompile_Types_04() => TestCompileCode(@"
main : void -> void
{
}

max2 : int x, int y -> int
{
    return x >= y ? x : y;
}
");
        [TestMethod]
        public (string, string) Test_CodeCompile_Types_05() => TestCompileCode(@"
main : void -> void
{
}

max2 : int x, int y -> int
{
    return x ? x : y;
}
", true);
    }
}
