// GodotStubs.cs — Minimal Godot API surface needed so game source compiles in a plain
// .NET test host without the Godot engine. Only stub what is actually used by game code.
// DO NOT add game logic here; stubs only.

using System;
using System.Collections.Generic;

// ReSharper disable all
#pragma warning disable CS8618
#pragma warning disable CA1050

// ── Godot namespace surface ────────────────────────────────────────────────────

namespace Godot
{
    // Attribute used by Godot to expose properties in the editor
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ExportAttribute : Attribute { }

    public static class Mathf
    {
        public const float Pi = MathF.PI;
        public static float Abs(float v)                     => MathF.Abs(v);
        public static float Clamp(float v, float lo, float hi) => Math.Clamp(v, lo, hi);
        public static float Max(float a, float b)            => MathF.Max(a, b);
        public static float Min(float a, float b)            => MathF.Min(a, b);
        public static int   FloorToInt(float v)              => (int)MathF.Floor(v);
        public static bool  IsEqualApprox(float a, float b)  => MathF.Abs(a - b) < 1e-5f;
        public static float Lerp(float a, float b, float t)  => a + (b - a) * t;
    }

    public static class GD
    {
        public static void Print(object? msg)     => Console.WriteLine($"[GD] {msg}");
        public static void PrintErr(object? msg)  => Console.Error.WriteLine($"[GD.ERR] {msg}");
        public static void PushError(string msg)  => Console.Error.WriteLine($"[GD.ERR] {msg}");
    }

    public static class Time
    {
        public static double GetUnixTimeFromSystem() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
    }

    public static class FileAccess
    {
        public enum ModeFlags { Read = 1, Write = 2, ReadWrite = 3 }

        public static bool FileExists(string path) => System.IO.File.Exists(MapPath(path));

        public static FileAccessHandle? Open(string path, ModeFlags mode)
        {
            try
            {
                string real = MapPath(path);
                if (mode == ModeFlags.Write || mode == ModeFlags.ReadWrite)
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(real)!);
                return new FileAccessHandle(real, mode);
            }
            catch { return null; }
        }

        // Overload used by DataRegistry: FileAccess.Open(path, FileAccess.ModeFlags.Read)
        // The compiler resolves Godot.FileAccess.ModeFlags directly, no ambiguity with System.IO.FileAccess
        public static string MapPath(string path)
            => path.Replace("user://", System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(), "cityzero_test") + "/")
                   .Replace("res://", "./");
    }

    public class FileAccessHandle : IDisposable
    {
        private readonly System.IO.FileStream _stream;
        private readonly System.IO.StreamReader? _reader;
        private readonly System.IO.StreamWriter? _writer;

        public FileAccessHandle(string path, FileAccess.ModeFlags mode)
        {
            var fm = mode switch
            {
                FileAccess.ModeFlags.Write     => System.IO.FileMode.Create,
                FileAccess.ModeFlags.ReadWrite => System.IO.FileMode.OpenOrCreate,
                _                              => System.IO.FileMode.Open
            };
            var fa = mode == FileAccess.ModeFlags.Read
                ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite;

            _stream = new System.IO.FileStream(path, fm, fa);
            if (mode == FileAccess.ModeFlags.Read)
                _reader = new System.IO.StreamReader(_stream);
            else
                _writer = new System.IO.StreamWriter(_stream);
        }

        public string GetAsText()          => _reader?.ReadToEnd() ?? string.Empty;
        public void   StoreString(string s) => _writer?.Write(s);
        public void   Close()              { _writer?.Flush(); _stream.Close(); }
        public void   Dispose()            => Close();
    }

    public static class DirAccess
    {
        public static void MakeDirRecursiveAbsolute(string path)
            => System.IO.Directory.CreateDirectory(MapPath(path));

        public static bool RemoveAbsolute(string path)
        { try { System.IO.File.Delete(MapPath(path)); return true; } catch { return false; } }

        public static bool RenameAbsolute(string from, string to)
        { try { System.IO.File.Move(MapPath(from), MapPath(to)); return true; } catch { return false; } }

        public static DirAccessHandle? Open(string path)
        {
            string real = MapPath(path);
            return System.IO.Directory.Exists(real) ? new DirAccessHandle(real) : null;
        }

        private static string MapPath(string path)
            => path.Replace("user://", System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(), "cityzero_test") + "/")
                   .Replace("res://", "./");
    }

    public class DirAccessHandle : IDisposable
    {
        private readonly string _path;
        private string[]? _files;
        private int _idx;
        public DirAccessHandle(string path) => _path = path;
        public void ListDirBegin() { _files = System.IO.Directory.GetFiles(_path); _idx = 0; }
        public string GetNext() => _idx < (_files?.Length ?? 0) ? System.IO.Path.GetFileName(_files![_idx++]) : string.Empty;
        public void ListDirEnd() { }
        public void Dispose() { }
    }

    // Minimal Node base — game systems extend this; test doubles override relevant virtuals
    public class Node
    {
        public string Name { get; set; } = string.Empty;
        public virtual void _Ready()   { }
        public virtual void _Process(double delta) { }
        public virtual void _ExitTree() { }
        public void QueueFree()        { }
        public void AddChild(Node n)   { }
    }

    public class Node2D : Node { }
    public class CharacterBody2D : Node2D { }
    public class RigidBody2D : Node2D { }

    public struct Vector2
    {
        public float X, Y;
        public Vector2(float x, float y) { X = x; Y = y; }
        public static Vector2 Zero => new(0, 0);
        public float Length() => MathF.Sqrt(X * X + Y * Y);
    }
}
