using System.Collections.Generic;

namespace Shaiya.Origin.Common.Networking.Packets
{
    public class PacketBuilder
    {
        // The opcode of the packet
        private int _opcode;

        // The payload of the packet
        private List<byte> _payload = new List<byte>();

        public PacketBuilder(int opcode)
        {
            _opcode = opcode;
        }

        /// <summary>
        /// Gets the opcode for this <see cref="PacketBuilder"/>
        /// </summary>
        /// <returns>The opcode</returns>
        public int GetOpcode()
        {
            return _opcode;
        }

        /// <summary>
        /// Gets the length of the packet builder
        /// </summary>
        /// <returns>The current length</returns>
        public int GetLength()
        {
            return _payload.Count;
        }

        /// <summary>
        /// Writes a single byte to the payload
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>This instance, for method chaining</returns>
        public PacketBuilder WriteByte(byte value)
        {
            _payload.Add(value);
            return this;
        }

        /// <summary>
        /// Writes an array of bytes to the payload
        /// </summary>
        /// <param name="values">The values to write</param>
        /// <returns>This instance, for method chaining</returns>
        public PacketBuilder WriteBytes(byte[] values)
        {
            foreach (var value in values)
            {
                WriteByte(value);
            }

            return this;
        }

        /// <summary>
        /// Writes a short to the payload
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>This instance, for method chaining</returns>
        public PacketBuilder WriteShort(int value)
        {
            WriteByte((byte)value);
            WriteByte((byte)((value >> 8) & 0xFF));
            return this;
        }

        /// <summary>
        /// Writes an int to the payload
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>This instance, for method chaining</returns>
        public PacketBuilder WriteInt(int value)
        {
            WriteByte((byte)(value & 0xFF));
            WriteByte((byte)((value >> 8) & 0xFF));
            WriteByte((byte)((value >> 16) & 0xFF));
            WriteByte((byte)((value >> 24) & 0xFF));
            return this;
        }

        /// <summary>
        /// Converts this packet builder instance, into a packet opcode
        /// </summary>
        /// <returns>The packet</returns>
        public Packet ToPacket()
        {
            // The packet instance
            Packet packet = new Packet
            {
                // Define the opcode
                opcode = _opcode,

                // Define the packet length
                length = GetLength() + 4,

                // Define the payload
                payload = _payload
            };

            // Return the packet instance
            return packet;
        }
    }
}