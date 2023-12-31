using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{
	C_PlayerLogin = 1,
	S_PlayerLogin = 2,
	S_PlayerInfo = 3,
	C_SavePlayer = 4,
	S_SavePlayer = 5,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class C_PlayerLogin : IPacket
{
	public string username;
	public string password;

	public ushort Protocol { get { return (ushort)PacketID.C_PlayerLogin; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort usernameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.username = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, usernameLen);
		count += usernameLen;
		ushort passwordLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.password = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, passwordLen);
		count += passwordLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_PlayerLogin), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort usernameLen = (ushort)Encoding.Unicode.GetBytes(this.username, 0, this.username.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(usernameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += usernameLen;
		ushort passwordLen = (ushort)Encoding.Unicode.GetBytes(this.password, 0, this.password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(passwordLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += passwordLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_PlayerLogin : IPacket
{
	public int loginOk;

	public ushort Protocol { get { return (ushort)PacketID.S_PlayerLogin; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.loginOk = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerLogin), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.loginOk), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_PlayerInfo : IPacket
{
	public int health;
	public float speed;
	public int attack;
	public int def;
	public float evasion;
	public int gold;
	public class Item
	{
		public int id;
		public string name;
		public int itemType;
		public int price;
		public string prefab;
	
		public void Read(ArraySegment<byte> segment, ref ushort count)
		{
			this.id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
			count += nameLen;
			this.itemType = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.price = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			ushort prefabLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
			count += sizeof(ushort);
			this.prefab = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, prefabLen);
			count += prefabLen;
		}
	
		public bool Write(ArraySegment<byte> segment, ref ushort count)
		{
			bool success = true;
			Array.Copy(BitConverter.GetBytes(this.id), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += nameLen;
			Array.Copy(BitConverter.GetBytes(this.itemType), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.price), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			ushort prefabLen = (ushort)Encoding.Unicode.GetBytes(this.prefab, 0, this.prefab.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			Array.Copy(BitConverter.GetBytes(prefabLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
			count += sizeof(ushort);
			count += prefabLen;
			return success;
		}	
	}
	public List<Item> items = new List<Item>();

	public ushort Protocol { get { return (ushort)PacketID.S_PlayerInfo; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.health = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.speed = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.attack = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.def = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.evasion = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.gold = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.items.Clear();
		ushort itemLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for (int i = 0; i < itemLen; i++)
		{
			Item item = new Item();
			item.Read(segment, ref count);
			items.Add(item);
		}
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerInfo), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.health), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.speed), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.attack), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.def), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.evasion), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.gold), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes((ushort)this.items.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		foreach (Item item in this.items)
			item.Write(segment, ref count);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class C_SavePlayer : IPacket
{
	public string username;
	public string password;

	public ushort Protocol { get { return (ushort)PacketID.C_SavePlayer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		ushort usernameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.username = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, usernameLen);
		count += usernameLen;
		ushort passwordLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.password = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, passwordLen);
		count += passwordLen;
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_SavePlayer), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		ushort usernameLen = (ushort)Encoding.Unicode.GetBytes(this.username, 0, this.username.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(usernameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += usernameLen;
		ushort passwordLen = (ushort)Encoding.Unicode.GetBytes(this.password, 0, this.password.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(passwordLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		count += passwordLen;

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

public class S_SavePlayer : IPacket
{
	public int saveOk;

	public ushort Protocol { get { return (ushort)PacketID.S_SavePlayer; } }

	public void Read(ArraySegment<byte> segment)
	{
		ushort count = 0;
		count += sizeof(ushort);
		count += sizeof(ushort);
		this.saveOk = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
	}

	public ArraySegment<byte> Write()
	{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort count = 0;

		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_SavePlayer), 0, segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes(this.saveOk), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);

		Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(count);
	}
}

