using Unity.Netcode;
using Unity.Collections;

public class ConnectionData
{
    public static byte[] Write(ServerManager.Config config, ClientMode mode)
    {
        FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp);

        writer.WriteValueSafe(config.password.Length);
        writer.WriteBytesSafe(System.Text.Encoding.ASCII.GetBytes(config.password));
        writer.WriteByteSafe((byte)mode);

        byte[] data = writer.ToArray();
        writer.Dispose();

        return data;
    }

    public static void Read(byte[] data, out string password, out ClientMode mode)
    {
        FastBufferReader reader = new FastBufferReader(data, Allocator.Temp);

        reader.ReadValueSafe(out int passwordSize);
        byte[] passwordBytes = new byte[passwordSize];
        for (int i = 0; i < passwordSize; i++)
        {
            reader.ReadByteSafe(out byte nextByte);
            passwordBytes[i] = nextByte;
        }
        password = System.Text.Encoding.ASCII.GetString(passwordBytes);

        reader.ReadByteSafe(out byte modeByte);
        mode = (ClientMode)modeByte;

        reader.Dispose();
    }

    public static bool TryRead(byte[] data, out string password, out ClientMode mode)
    {
        try
        {
            Read(data, out password, out mode);
            return true;
        }
        catch(System.Exception e)
        {
            password = "";
            mode = ClientMode.PLAYER;
            return false;
        }
    }
}
