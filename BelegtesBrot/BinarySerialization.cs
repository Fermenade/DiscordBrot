using System.Runtime.InteropServices;

namespace BelegtesBrot;

public static class BinarySerialization
{
    public static T ByteToType<T>(BinaryReader reader) // TODO: Look at me
    {
        var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        var theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();

        return theStructure;
    }

    public static T ReadStructFromBinaryFile<T>(string filePath) where T : struct // TODO: Look at me
    {
        var bytes = File.ReadAllBytes(filePath);

        var ptr = Marshal.AllocHGlobal(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    private static void WriteStructToBinaryFile<T>(T structure, string filePath) where T : struct // TODO: Look at me
    {
        var size = Marshal.SizeOf(structure);
        var bytes = new byte[size];

        var ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        File.WriteAllBytes(filePath, bytes);
    }
}