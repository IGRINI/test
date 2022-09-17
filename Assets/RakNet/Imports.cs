using System;
using System.Runtime.InteropServices;
using System.Security;

/// <summary>
/// Imported functions from RakNet
/// </summary>
[SuppressUnmanagedCodeSecurity] /* Increase performance by suppressing unmanaged code security checks */
public static class Imports
{
    private const string _dllName = "RakNet";

    /* CLIENT */
    [DllImport(_dllName)]
    public static extern IntPtr Client_Init();

    [DllImport(_dllName)]
    public static extern void Client_Destroy();

    [DllImport(_dllName)]
    public static extern bool Client_IsActive(IntPtr p);

    [DllImport(_dllName)]
    public static extern ClientConnectResult Client_Connect(IntPtr p, string address, ushort port, string password, int attepmts);

    [DllImport(_dllName)]
    public static extern void Client_Disconnect(IntPtr p, string message = "");

    [DllImport(_dllName)]
    public static extern IntPtr Client_GetPacket(IntPtr p, out uint packet_size, out ulong local_time);

    [DllImport(_dllName)]
    public static extern void Client_DeallocPacket(IntPtr p, IntPtr p2);

    [DllImport(_dllName)]
    public static extern byte Client_Send(IntPtr p, IntPtr b, PacketPriority priority, PacketReliability reliability, byte channel);

    [DllImport(_dllName)]
    public static extern int Client_GetPing(IntPtr p);

    [DllImport(_dllName)]
    public static extern int Client_GetAveragePing(IntPtr p);

    [DllImport(_dllName)]
    public static extern int Client_GetLowestPing(IntPtr p);
	
    [DllImport(_dllName)]
    public static extern ulong Client_Guid(IntPtr p);

    /* SERVER */
    [DllImport(_dllName)]
    public static extern IntPtr Server_Init();

    [DllImport(_dllName)]
    public static extern void Server_Destroy();

    [DllImport(_dllName)]
    public static extern ServerStartResult Server_Start(IntPtr p, string address, ushort port, ushort max_connections, bool insecure);

    [DllImport(_dllName)]
    public static extern void Server_Stop(IntPtr p, string message="");

    [DllImport(_dllName)]
    public static extern IntPtr Server_IP(IntPtr p);

    [DllImport(_dllName)]
    public static extern IntPtr Server_GetPacket(IntPtr p, out ushort connectionIndex, out ulong guid, out uint packet_size, out ulong local_time);

    [DllImport(_dllName)]
    public static extern void Server_DeallocPacket(IntPtr p, IntPtr p2);

    [DllImport(_dllName)]
    public static extern void Server_SetMaxConnections(IntPtr p, ushort max_connections);

    [DllImport(_dllName)]
    public static extern void Server_SetPassword(IntPtr p, string password);

    [DllImport(_dllName)]
    public static extern bool Server_HasPassword(IntPtr p);

    [DllImport(_dllName)]
    public static extern void Server_AddBanIP(IntPtr p, string address, int seconds);

    [DllImport(_dllName)]
    public static extern void Server_RemoveBanIP(IntPtr p, string address);

    [DllImport(_dllName)]
    public static extern bool Server_IsBannedIP(IntPtr p, string address);

    [DllImport(_dllName)]
    public static extern void Server_LimitBandwidth(IntPtr p, uint bytes_per_sec);

    [DllImport(_dllName)]
    public static extern void Server_SetLimitIPConnectionFrequency(IntPtr p, bool enabled);

    [DllImport(_dllName)]
    public static extern ushort Server_NumberOfConnections(IntPtr p);

    [DllImport(_dllName)]
    public static extern ushort Server_GetMaximumConnections(IntPtr p);

    [DllImport(_dllName)]
    public static extern void Server_CloseConnection(IntPtr p, ulong guid, bool send_disconnect_notify, string disconnect_message);

    [DllImport(_dllName)]
    public static extern uint Server_SendToClient(IntPtr p, IntPtr b, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

    [DllImport(_dllName)]
    public static extern uint Server_SendToAll(IntPtr p, IntPtr b, PacketPriority priority, PacketReliability reliability, byte channel);

    [DllImport(_dllName)]
    public static extern uint Server_SendToAllIgnore(IntPtr p, IntPtr b, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

    [DllImport(_dllName)]
    public static extern ulong Server_GetGuidFromIndex(IntPtr p, int index);

    [DllImport(_dllName)]
    public static extern int Server_GetIndexFromGuid(IntPtr p, ulong guid);

    [DllImport(_dllName)]
    public static extern int Server_GetPing(IntPtr p, ulong guid);

    [DllImport(_dllName)]
    public static extern int Server_GetAveragePing(IntPtr p, ulong guid);

    [DllImport(_dllName)]
    public static extern int Server_GetLowestPing(IntPtr p, ulong guid);
	
    [DllImport(_dllName)]
    public static extern void Server_AllowQuery(IntPtr p, bool enabled);

    [DllImport(_dllName)]
    public static extern bool Server_IsQueryAllowed(IntPtr p);

    [DllImport(_dllName)]
    public static extern void Server_SetQueryResponce(IntPtr p, byte[] data);

    /* SHARED */
    [DllImport(_dllName)]
    public static extern IntPtr Shared_GetAddress(IntPtr p, ulong guid, bool with_port);

    [DllImport(_dllName)]
    public static extern ushort Shared_GetPort(IntPtr p, ulong guid);

    [DllImport(_dllName)]
    public static extern ulong Shared_GetStatisticsLastSeconds(IntPtr p, uint index, RNSPerSecondMetrics metrics);

    [DllImport(_dllName)]
    public static extern ulong Shared_GetStatisticsTotal(IntPtr p, uint index, RNSPerSecondMetrics metrics);

    [DllImport(_dllName)]
    public static extern bool Shared_Statistics(IntPtr p, uint index, ref RakNetStatistics statistics);

    [DllImport(_dllName)]
    public static extern bool Shared_IsAllowSending(IntPtr p);

	[DllImport(_dllName)]
    public static extern void Shared_AllowSending(IntPtr p, bool allow);
	
	[DllImport(_dllName)]
    public static extern bool Shared_IsAllowReceiving(IntPtr p);

	[DllImport(_dllName)]
    public static extern void Shared_AllowReceiving(IntPtr p, bool allow);

    /* BITSTREAM */
    [DllImport(_dllName)]
    public static extern IntPtr BitStream_Create1();

    [DllImport(_dllName)]
    public static extern IntPtr BitStream_Create2(IntPtr packet_ptr);

    [DllImport(_dllName)]
    public static extern void BitStream_ReadPacket(IntPtr bitstream_pointer, IntPtr packet_ptr);

    [DllImport(_dllName)]
    public static extern void BitStream_Close(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_IgnoreBytes(IntPtr bitstream_pointer, int numberOfBytes);

    [DllImport(_dllName)]
    public static extern void BitStream_ResetWritePointer(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_SetWriteOffset(IntPtr bitstream_pointer, uint offset);

    [DllImport(_dllName)]
    public static extern uint BitStream_GetWriteOffset(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_ResetReadPointer(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_SetReadOffset(IntPtr bitstream_pointer, uint offset);

    [DllImport(_dllName)]
    public static extern uint BitStream_GetReadOffset(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_Reset(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern void BitStream_GetData(IntPtr bitstream_pointer, out IntPtr data, out int data_size);

    [DllImport(_dllName)]
    public static extern void BitStream_SetData(IntPtr bitstream_pointer, byte[] data, int data_size);

    /* BYTE */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteByte(IntPtr bitstream_pointer, byte value);

    [DllImport(_dllName)]
    public static extern void BitStream_WriteByteCompressed(IntPtr bitstream_pointer, byte value);

    [DllImport(_dllName)]
    public static extern byte BitStream_ReadByte(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern byte BitStream_ReadByteCompressed(IntPtr bitstream_pointer);

    /* SHORT */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteShort(IntPtr bitstream_pointer, short value);

    [DllImport(_dllName)]
    public static extern void BitStream_WriteShortCompressed(IntPtr bitstream_pointer, short value);

    [DllImport(_dllName)]
    public static extern short BitStream_ReadShort(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern short BitStream_ReadShortCompressed(IntPtr bitstream_pointer);

    /* INT */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteInt(IntPtr bitstream_pointer, int value);

    [DllImport(_dllName)]
    public static extern void BitStream_WriteIntCompressed(IntPtr bitstream_pointer, int value);

    [DllImport(_dllName)]
    public static extern int BitStream_ReadInt(IntPtr bitstream_pointer);
	
    [DllImport(_dllName)]
    public static extern int BitStream_ReadIntCompressed(IntPtr bitstream_pointer);

    /* FLOAT */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteFloat(IntPtr bitstream_pointer, float value);

    [DllImport(_dllName)]
    public static extern void BitStream_WriteFloatCompressed(IntPtr bitstream_pointer, float value);

    [DllImport(_dllName)]
    public static extern float BitStream_ReadFloat(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern float BitStream_ReadFloatCompressed(IntPtr bitstream_pointer);

    /* FLOAT16 */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteFloat16(IntPtr bitstream_pointer, float value, float min, float max);

    [DllImport(_dllName)]
    public static extern float BitStream_ReadFloat16(IntPtr bitstream_pointer, float min, float max);

    /* LONG */
    [DllImport(_dllName)]
    public static extern void BitStream_WriteLong(IntPtr bitstream_pointer, long value);

    [DllImport(_dllName)]
    public static extern void BitStream_WriteLongCompressed(IntPtr bitstream_pointer, long value);
	
    [DllImport(_dllName)]
    public static extern long BitStream_ReadLong(IntPtr bitstream_pointer);

    [DllImport(_dllName)]
    public static extern long BitStream_ReadLongCompressed(IntPtr bitstream_pointer);
	
	/* STRING */
	[DllImport(_dllName)]
    public static extern void BitStream_WriteString(IntPtr bitstream_pointer, string value);
	
	[DllImport(_dllName)]
    public static extern IntPtr BitStream_ReadString(IntPtr bitstream_pointer);
	
	[DllImport(_dllName)]
    public static extern void BitStream_WriteCompressedString(IntPtr bitstream_pointer, string value, ushort languageId, bool writelanguageId);
	
	[DllImport(_dllName)]
    public static extern IntPtr BitStream_ReadCompressedString(IntPtr bitstream_pointer, bool readLanguageId);
	
	[DllImport(_dllName)]
    public static extern void BitStream_WriteArray(IntPtr bitstream_pointer, byte[] array);
	
	[DllImport(_dllName)]
    public static extern void BitStream_ReadArray(IntPtr bitstream_pointer, ref byte[] array);
	
	/* Data Compressor */
    [DllImport(_dllName)]
    public static extern void DataCompressor_Compress(byte[] data, int data_size, IntPtr bitstream_pointer);
	
	[DllImport(_dllName)]
    public static extern int DataCompressor_Decompress(IntPtr bitstream_pointer, ref byte[] data);

	/* String From IntPtr */
    public static string IntPtrToStringAnsi(IntPtr p)
    {
        return Marshal.PtrToStringAnsi(p);
    }

    public static string IntPtrToStringUnicode(IntPtr p)
    {
        return Marshal.PtrToStringUni(p);
    }
}
