using Lox.Exceptions;
using Shouldly;

namespace Lox.Test;

public class LoxEnvironmentTest {
    [Fact]
    public void Get() {
        var env = new LoxEnvironment();

        const string key = "key";
        const string value = "value";

        env.Define(key, value);

        var actual = env.Get(key);

        actual.ShouldBe(value);
    }

    [Fact]
    public void Get_Undefined() {
        var env = new LoxEnvironment();

        var get = () => env.Get("undefined");

        get.ShouldThrow<UndefinedVariableException>();
    }

    [Fact]
    public void Get_DefinedInParent() {
        var env = new LoxEnvironment();

        const string key = "key";
        const string value = "value";

        env.Define(key, value);

        env = new LoxEnvironment(env);

        var actual = env.Get(key);

        actual.ShouldBe(value);
    }

    [Fact]
    public void Assign() {
        var env = new LoxEnvironment();

        const string key = "key";
        const string value = "value";
        const string value2 = "value2";

        env.Define(key, value);
        env.Assign(key, value2);

        var actual = env.Get(key);

        actual.ShouldBe(value2);
    }

    [Fact]
    public void Assign_Undefined() {
        var env = new LoxEnvironment();

        const string key = "key";
        const string value = "value";

        var assign = () => env.Assign(key, value);

        assign.ShouldThrow<UndefinedVariableException>();
    }

    [Fact]
    public void Assign_DefinedInParent() {
        var env = new LoxEnvironment();

        const string key = "key";
        const string value = "value";
        const string value2 = "value2";

        env.Define(key, value);

        env = new LoxEnvironment(env);

        env.Assign(key, value2);

        var actual = env.Get(key);

        actual.ShouldBe(value2);
    }

    [Fact]
    public void GetAt() {
        var env = new LoxEnvironment();
        const string key = "key";
        const string value = "value";
        env.Define(key, value);

        env =  new LoxEnvironment(env);

        const string value2 = "value2";
        env.Define(key, value2);

        env.GetAt(0, key).ShouldBe(value2);
        env.GetAt(1, key).ShouldBe(value);
    }
}
