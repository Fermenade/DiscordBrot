using System.Runtime.InteropServices;

namespace BelegtesBrot;

public static class BinarySerialization
{
    public static T ByteToType<T>(BinaryReader reader) // TODO: Look at me
    {
        byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
        handle.Free();

        return theStructure;
    }

    public static T ReadStructFromBinaryFile<T>(string filePath) where T : struct // TODO: Look at me
    {
        byte[] bytes = File.ReadAllBytes(filePath);

        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
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

    static void WriteStructToBinaryFile<T>(T structure, string filePath) where T : struct // TODO: Look at me
    {
        int size = Marshal.SizeOf(structure);
        byte[] bytes = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
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